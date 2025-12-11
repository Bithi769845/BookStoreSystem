using BookStore.Data;
using BookStore.Models.Identity;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplDbContext _context;

        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // List Roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // Create Role - GET
        public IActionResult Create() => View();

        // Create Role - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return View();

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }

            return RedirectToAction("Index");
        }

        // Assign Modules to Role
        public IActionResult AssignModules(string roleId)
        {
            var role = _roleManager.FindByIdAsync(roleId).Result;
            var modules = _context.Modules.ToList();

            var model = new RoleModuleViewModel
            {
                RoleId = roleId,
                RoleName = role?.Name ?? "",
                Modules = modules.Select(m =>
                {
                    var access = _context.RoleModuleAccesses.FirstOrDefault(r => r.RoleId == roleId && r.ModuleId == m.ModuleId);
                    return new ModuleAccessModel
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.Name,
                        CanView = access != null && access.CanView,
                        CanCreate = access != null && access.CanCreate,
                        CanEdit = access != null && access.CanEdit,
                        CanDelete = access != null && access.CanDelete
                    };
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignModules(RoleModuleViewModel model)
        {
            // Remove existing
            var existing = _context.RoleModuleAccesses.Where(r => r.RoleId == model.RoleId).ToList();
            _context.RoleModuleAccesses.RemoveRange(existing);

            // Add new
            foreach (var m in model.Modules)
            {
                _context.RoleModuleAccesses.Add(new BookStore.Models.RoleModuleAccess
                {
                    RoleId = model.RoleId,
                    ModuleId = m.ModuleId,
                    CanView = m.CanView,
                    CanCreate = m.CanCreate,
                    CanEdit = m.CanEdit,
                    CanDelete = m.CanDelete
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // EDIT role GET
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        // EDIT role POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationRole model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            role.Name = model.Name;

            await _roleManager.UpdateAsync(role);
            return RedirectToAction("Index");
        }

        // DELETE role GET
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        // DELETE role POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApplicationRole model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }

    }
}
