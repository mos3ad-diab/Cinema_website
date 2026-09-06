using Cinema_website.Data;
using Cinema_website.Models;
using Cinema_website.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace Cinema_website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Actor> _actorRepository;// = new Repository<Actor>();

        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var actor = await _actorRepository.GetAllAsync();
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Index(string actorQuery)
        {
            var actor = await _actorRepository.GetAllAsync(e => e.Name.Contains(actorQuery));
            ViewBag.ActorQuery = actorQuery;
            return View(actor.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View(new Actor());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile img)
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

            await _actorRepository.InsertAsync(actor);
            await _actorRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            //var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(c => c.Id == actor.Id);
            var actorInDb = await _actorRepository.GetOneAsync(e => e.Id == actor.Id);
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
            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e=>e.Id == id);
            if (!string.IsNullOrEmpty(actor.Img))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Actor_Images", actor.Img);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync();
            return RedirectToAction("Index");
        }

    }
}
