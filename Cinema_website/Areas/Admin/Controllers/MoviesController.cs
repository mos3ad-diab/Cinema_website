using Cinema_website.Data;
using Cinema_website.Models;
using Cinema_website.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cinema_website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly IRepository<Movie> _movieRepository; // = new Repository<Movie>();
        private readonly IRepository<Category> _categoryRepository; //= new Repository<Category>();
        private readonly IRepository<Cinema> _cinemaRepository; //= new Repository<Cinema>();
        private readonly IRepository<Actor> _actorRepository; //= new Repository<Actor>();
        private readonly IMovieActorRepository _movieActorRepository;// = new MovieActorRepository();

        public MoviesController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, IMovieActorRepository movieActorRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieActorRepository = movieActorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAllAsync();
            return View(movies.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string searchQuery)
        {
            var moviesQuery = await _movieRepository.GetAllAsync(
                  includes:  [ m => m.Category, m => m.Cinema ]);



            if (!string.IsNullOrEmpty(searchQuery))
            {
                moviesQuery = moviesQuery.Where(m =>
                    m.Name.Contains(searchQuery) ||
                    m.Cinema.Name.Contains(searchQuery));
            }

            ViewBag.SearchQuery = searchQuery;
            return View(moviesQuery.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            

            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Cinemas = new SelectList(await _cinemaRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Actors = await _actorRepository.GetAllAsync();


            return View(new Movie());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile mainImg, List<IFormFile> subImgs, List<int> selectedActors)
        {
            if (!ModelState.IsValid)
            {
                return View(movie);
            }
            // 1. Main Image
            if (mainImg != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + mainImg.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies_Images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                movie.MainImg = fileName;
            }

            // 2. Sub Images
            if (subImgs != null && subImgs.Count > 0)
            {
                movie.SubImgs = new List<SubImg>();
                foreach (var file in subImgs)
                {
                    var subFileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    var subFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies_SubImages", subFileName);

                    using (var stream = System.IO.File.Create(subFilePath))
                    {
                        file.CopyTo(stream);
                    }

                    movie.SubImgs.Add(new SubImg { Sub_Img = subFileName });
                }
            }

            // 3. Actors
            if (selectedActors != null && selectedActors.Count > 0)
            {
                movie.MovieActors = new List<MovieActor>();
                foreach (var actorId in selectedActors)
                {
                    movie.MovieActors.Add(new MovieActor { ActorId = actorId });
                }
            }

            await _movieRepository.InsertAsync(movie);
            await _movieRepository.CommitAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetOneAsync(
                  filter: e=>e.Id == id ,
                  includes: [m => m.Category, m => m.Cinema, m => m.MovieActors]);

            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", movie.CategoryId);
            ViewBag.Cinemas = new SelectList(await _cinemaRepository.GetAllAsync(), "Id", "Name", movie.CinemaId);
            ViewBag.Actors = await _actorRepository.GetAllAsync();
            ViewBag.SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

            return View(movie);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile mainImg, List<IFormFile> subImgs, List<int> selectedActors)
        {
            if (!ModelState.IsValid)
            {
                return View(movie);
            }
            //var movieInDb = _context.Movies
            //  .Include(m => m.SubImgs)
            //  .Include(m => m.MovieActors)
            //  .FirstOrDefault(m => m.Id == movie.Id);

            var movieInDb = await _movieRepository.GetOneAsync(filter: m => m.Id == movie.Id, includes: [m=>m.SubImgs , m=>m.MovieActors]);

            if (movieInDb == null) return NotFound();

            
            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Status = movie.Status;
            movieInDb.Date_Time = movie.Date_Time;
            movieInDb.CategoryId = movie.CategoryId;
            movieInDb.CinemaId = movie.CinemaId;

            
            if (mainImg != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + mainImg.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies_Images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                movieInDb.MainImg = fileName;
            }

            
            if (subImgs != null && subImgs.Count > 0)
            {
                foreach (var file in subImgs)
                {
                    var subFileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    var subFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies_SubImages", subFileName);

                    using (var stream = System.IO.File.Create(subFilePath))
                    {
                        file.CopyTo(stream);
                    }

                    movieInDb.SubImgs.Add(new SubImg { Sub_Img = subFileName });
                }
            }

            
            _movieActorRepository.DeleteRange(movieInDb.MovieActors);
            if (selectedActors != null && selectedActors.Count > 0)
            {
                movieInDb.MovieActors = selectedActors.Select(actorId => new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                }).ToList();
            }
            _movieRepository.Update(movieInDb);
            await _movieActorRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {

            var movie = await _movieRepository.GetOneAsync(filter: m => m.Id == id, includes: [m => m.SubImgs, m => m.MovieActors]);

            if (movie == null)
            {
                return NotFound();
            }

           
            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                var mainImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Movies_Images", movie.MainImg);
                if (System.IO.File.Exists(mainImgPath))
                {
                    System.IO.File.Delete(mainImgPath);
                }
            }

            if (movie.SubImgs != null && movie.SubImgs.Any())
            {
                foreach (var sub in movie.SubImgs)
                {
                    var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Movies_SubImages", sub.Sub_Img);
                    if (System.IO.File.Exists(subImgPath))
                    {
                        System.IO.File.Delete(subImgPath);
                    }
                }
            }

          
            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            return RedirectToAction(nameof(Index));

            
        }
    }
}
