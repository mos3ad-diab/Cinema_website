using Cinema_website.Data;
using Cinema_website.Models;
using Cinema_website.Repositories;
using Cinema_website.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cinema_website.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.ADMIN_ROLE} ,{CD.SUPER_ADMIN_ROLE}")]
    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _cinemaRepository;// = new Repository<Cinema>();

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cinema = await _cinemaRepository.GetAllAsync();
            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string cinemaQuery)
        {

            var cinema = await _cinemaRepository.GetAllAsync(e=>e.Name.Contains(cinemaQuery));
            ViewBag.CinemaQuery = cinemaQuery;
            return View(cinema.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            
            return View(new Cinema());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile img)
        {
            if(!ModelState.IsValid)
            {
                return View(cinema);
            }
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

            await _cinemaRepository.InsertAsync(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e=>e.Id == id);
            return View(cinema);
        }
        [HttpPost]
        [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Cinema cinema , IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            //var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.Id == cinema.Id);
            var cinemaInDb = await _cinemaRepository.GetOneAsync(filter: e => e.Id == cinema.Id, isTracked: false);

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
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _cinemaRepository.GetOneAsync(e => e.Id == id);
            if (!string.IsNullOrEmpty(cinema.Img))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Cinema_Images", cinema.Img);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction("Index");
        }

    }
}
