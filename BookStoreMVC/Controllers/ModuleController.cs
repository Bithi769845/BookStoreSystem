using BookStore.Data;
using BookStore.Models;
using BookStoreMVC.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ModuleController : Controller
    {
        private readonly ApplDbContext _context;

        public ModuleController(ApplDbContext context)
        {
            _context = context;
        }

        [ModuleAuthorize("View")]
        public IActionResult Index()
        {
            return View(_context.Modules.ToList());
        }

        // CREATE
        [ModuleAuthorize("Create")]
        public IActionResult Create() => View();


        [HttpPost]
        [ModuleAuthorize("Create")]
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
        [ModuleAuthorize("Edit")]
        public IActionResult Edit(int id)
        {
            var module = _context.Modules.Find(id);
            return View(module);
        }

        [HttpPost]
        [ModuleAuthorize("Edit")]
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
        [ModuleAuthorize("Delete")]
        public IActionResult Delete(int id)
        {
            var module = _context.Modules.Find(id);
            return View(module);
        }

        [HttpPost]
        [ModuleAuthorize("Delete")]
        public IActionResult Delete(Module model)
        {
            var module = _context.Modules.Find(model.ModuleId);
            _context.Modules.Remove(module);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
