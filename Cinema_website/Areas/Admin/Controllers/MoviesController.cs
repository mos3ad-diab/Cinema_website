using Cinema_website.Data;
using Cinema_website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cinema_website.Areas.Admin.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        [HttpGet]
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }
        [HttpPost]
        public IActionResult Index(string searchQuery)
        {
            var moviesQuery = _context.Movies
              .Include(m => m.Category)
              .Include(m => m.Cinema)
              .AsQueryable();

         
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
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Cinemas = new SelectList(_context.Cinemas.ToList(), "Id", "Name");
            ViewBag.Actors = _context.Actors.ToList();


            return View();
        }
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile mainImg, List<IFormFile> subImgs, List<int> selectedActors)
        {
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

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.SubImgs)
                .Include(m => m.MovieActors)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", movie.CategoryId);
            ViewBag.Cinemas = new SelectList(_context.Cinemas, "Id", "Name", movie.CinemaId);
            ViewBag.Actors = _context.Actors.ToList();
            ViewBag.SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

            return View(movie);
        }

        
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile mainImg, List<IFormFile> subImgs, List<int> selectedActors)
        {
            var movieInDb = _context.Movies
                .Include(m => m.SubImgs)
                .Include(m => m.MovieActors)
                .FirstOrDefault(m => m.Id == movie.Id);

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

            
            _context.MovieActors.RemoveRange(movieInDb.MovieActors);
            if (selectedActors != null && selectedActors.Count > 0)
            {
                movieInDb.MovieActors = selectedActors.Select(actorId => new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                }).ToList();
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {

            var movie = _context.Movies
                  .Include(m => m.SubImgs)
                  .Include(m => m.MovieActors)
                  .FirstOrDefault(m => m.Id == id);

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

          
            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

            
        }
    }
}
