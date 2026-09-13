using Cinema_website.Models;
using Cinema_website.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_website.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, IRepository<Movie> movieRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _movieRepository = movieRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();

            var carts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id , includes: [m => m.Movie]);
            return View(carts);
        }

        public async Task<IActionResult> AddToCart(int movieId, int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var movie = await _movieRepository.GetOneAsync(e => e.Id == movieId);
            var cartInDb = await _cartRepository.GetOneAsync( e => e.MovieId == movieId && e.ApplicationUserId == user.Id);

            if (cartInDb != null)
            {
                cartInDb.Count += count;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart()
            {
                ApplicationUserId = user.Id,
                MovieId = movieId,
                Count = count,
                Price = movie.Price,
            };

            await _cartRepository.InsertAsync(cart);
            await _cartRepository.CommitAsync();
             
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Increment(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var movie = await _movieRepository.GetOneAsync(e=>e.Id == movieId);
            if (movie == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if(carts.Count < movie.Amount)
            {
                carts.Count++;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Decrement(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var movie = await _movieRepository.GetOneAsync(e => e.Id == movieId);
            if (movie == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);
            if (carts.Count > 1)
            {
                carts.Count--;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var movie = await _movieRepository.GetOneAsync(e => e.Id == movieId);
            if (movie == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == movieId);

            _cartRepository.Delete(carts);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
