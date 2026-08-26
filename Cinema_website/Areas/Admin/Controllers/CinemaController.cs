using Cinema_website.Data;
using Cinema_website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cinema_website.Areas.Admin.Controllers
{
    
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        [HttpGet]
        public IActionResult Index()
        {
            var cinema = _context.Cinemas.ToList();
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Index(string cinemaQuery)
        {
            var cinema = _context.Cinemas.Where(e=>e.Name.Contains(cinemaQuery));
            ViewBag.CinemaQuery = cinemaQuery;
            return View(cinema.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile img)
        {

            if (img != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + img.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Cinema_Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);

                }
                cinema.Img = fileName;
            }

            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.Cinemas.Find(id);
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema , IFormFile img)
        {
            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);

            if (img != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + img.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Cinema_Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);

                }
                cinema.Img = fileName;
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.Find(id);
            if (!string.IsNullOrEmpty(cinema.Img))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema_Images", cinema.Img);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
