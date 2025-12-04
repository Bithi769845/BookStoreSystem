using BookStore.Data;
using Microsoft.AspNetCore.Identity;
using BookStore.Models;
using BookStore.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSystem.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionService(ApplDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<Module>> GetModulesForUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Module>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<Module>();

            var roles = await _userManager.GetRolesAsync(user); // role names
            if (roles == null || roles.Count == 0)
                return new List<Module>();

            // Get role ids from names (role names may be null/empty)
            var normalizedRoleNames = roles.Where(r => !string.IsNullOrEmpty(r)).Select(r => r).ToList();
            if (normalizedRoleNames.Count == 0)
                return new List<Module>();

            var roleEntities = await _roleManager.Roles.Where(r => normalizedRoleNames.Contains(r.Name)).ToListAsync();
            var roleIds = roleEntities.Select(r => r.Id).ToList();
            if (roleIds.Count == 0)
                return new List<Module>();

            // Collect allowed modules from all roles. Defensive include and projection.
            var modules = await _context.RoleModuleAccesses
                .Where(rm => roleIds.Contains(rm.RoleId) && rm.CanView)
                .Include(rm => rm.Module)
                .Select(rm => rm.Module)
                .Where(m => m != null)
                .Select(m => m!)
                .Distinct()
                .ToListAsync();

            return modules ?? new List<Module>();
        }

        public bool HasAccess(string roleId, string moduleName, string action)
        {
            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(moduleName))
                return false;

            var access = _context.RoleModuleAccesses
                .Include(rm => rm.Module)
                .FirstOrDefault(rm =>
                    rm.RoleId == roleId &&
                    rm.Module != null && rm.Module.Name == moduleName &&
                    ((action == "View" && rm.CanView) ||
                     (action == "Edit" && rm.CanEdit) ||
                     (action == "Delete" && rm.CanDelete))
                );

            return access != null;
        }
    }
}
