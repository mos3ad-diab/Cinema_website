using Cinema_website.Models;
using Cinema_website.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_website.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.InsertAsync(category);
                await _categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetOneAsync(filter: c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                await _categoryRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetOneAsync(filter: c => c.Id == id);
            if (category != null)
            {
                _categoryRepository.Delete(category);
                await _categoryRepository.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
