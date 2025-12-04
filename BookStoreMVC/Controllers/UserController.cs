using BookStore.Data;
using BookStore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreMVC.Controllers
{
 [Authorize(Roles = "Admin")]
 public class UserController : Controller
 {
 private readonly UserManager<ApplicationUser> _userManager;
 private readonly RoleManager<ApplicationRole> _roleManager;
 private readonly ApplDbContext _context;

 public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplDbContext context)
 {
 _userManager = userManager;
 _roleManager = roleManager;
 _context = context;
 }

 public async Task<IActionResult> Index()
 {
 var users = await _userManager.Users.ToListAsync();
 var model = users.Select(u => new UserListItem { Id = u.Id, Email = u.Email ?? "", FullName = u.FullName }).ToList();
 return View(model);
 }

 public async Task<IActionResult> EditRoles(string id)
 {
 var user = await _userManager.FindByIdAsync(id);
 if (user == null) return NotFound();

 var roles = _roleManager.Roles.Select(r => r.Name).ToList();
 var userRoles = await _userManager.GetRolesAsync(user);
 var model = new EditRolesViewModel
 {
 UserId = id,
 Email = user.Email,
 Roles = roles.Select(r => new RoleCheckbox { Name = r ?? "", Selected = userRoles.Contains(r) }).ToList()
 };
 return View(model);
 }

 [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> EditRoles(EditRolesViewModel model)
 {
 var user = await _userManager.FindByIdAsync(model.UserId);
 if (user == null) return NotFound();

 var currentRoles = await _userManager.GetRolesAsync(user);
 var selected = model.Roles.Where(r => r.Selected).Select(r => r.Name).ToList();

 var toAdd = selected.Except(currentRoles).ToArray();
 var toRemove = currentRoles.Except(selected).ToArray();

 if (toAdd.Any()) await _userManager.AddToRolesAsync(user, toAdd);
 if (toRemove.Any()) await _userManager.RemoveFromRolesAsync(user, toRemove);

 return RedirectToAction("Index");
 }
 }

 public class UserListItem
 {
 public string Id { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string FullName { get; set; } = string.Empty;
 }

 public class EditRolesViewModel
 {
 public string UserId { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public List<RoleCheckbox> Roles { get; set; } = new List<RoleCheckbox>();
 }

 public class RoleCheckbox
 {
 public string Name { get; set; } = string.Empty;
 public bool Selected { get; set; }
 }
}
