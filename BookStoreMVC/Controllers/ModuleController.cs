using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ApplDbContext _context;

        public ModuleController(ApplDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Modules.ToList());
        }

        // CREATE
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Module model)
        {
            if (ModelState.IsValid)
            {
                _context.Modules.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            var module = _context.Modules.Find(id);
            return View(module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Module model)
        {
            if (ModelState.IsValid)
            {
                _context.Modules.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var module = _context.Modules.Find(id);
            return View(module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Module model)
        {
            var module = _context.Modules.Find(model.ModuleId);
            _context.Modules.Remove(module);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
