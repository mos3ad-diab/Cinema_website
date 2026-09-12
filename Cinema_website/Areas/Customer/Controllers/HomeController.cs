using Cinema_website.Models;
using Cinema_website.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_website.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;

        public HomeController(IRepository<Movie> movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAllAsync();
            return View(movies);
        }
    }
}
