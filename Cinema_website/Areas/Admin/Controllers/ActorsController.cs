using Cinema_website.Data;
using Cinema_website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cinema_website.Areas.Admin.Controllers
{

    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        [HttpGet]
        public IActionResult Index()
        {
            var actor = _context.Actors.ToList();
            return View(actor);
        }
        [HttpPost]
        public IActionResult Index(string actorQuery)
        {
            var actor = _context.Actors.Where(e => e.Name.Contains(actorQuery));
            ViewBag.ActorQuery = actorQuery;
            return View(actor.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile img)
        {

            if (img != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + img.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Actors_Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);

                }
                actor.Img = fileName;
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.Find(id);
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile img)
        {
            var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);

            if (img != null)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + img.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Actor_Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    img.CopyTo(stream);

                }
                actor.Img = fileName;
            }
            else
            {
                actor.Img = actorInDb.Img;
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);
            if (!string.IsNullOrEmpty(actor.Img))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor_Images", actor.Img);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
