using BookStore.Models.Identity;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreMVC.Attributes
{
    // Attribute used on controllers/actions - forwards to ModuleAuthorizeFilter which is resolved from DI
    public class ModuleAuthorizeAttribute : TypeFilterAttribute
    {
        public ModuleAuthorizeAttribute(string action) : base(typeof(ModuleAuthorizeFilter))
        {
            Arguments = new object[] { action };
        }
    }

    // Actual filter resolved from DI so services are injected by the framework
    public class ModuleAuthorizeFilter : IAuthorizationFilter
    {
        private readonly string _action;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;

        public ModuleAuthorizeFilter(string action, UserManager<ApplicationUser> userManager, IPermissionService permissionService)
        {
            _action = action;
            _userManager = userManager;
            _permissionService = permissionService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Defensive: if DI failed, forbid
            if (_userManager == null || _permissionService == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userId = _userManager.GetUserId(context.HttpContext.User);
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            if (roles == null || roles.Count == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Allow Admin role to bypass module-level checks
            if (roles.Contains("Admin"))
            {
                // optional: log
                Console.WriteLine($"ModuleAuthorize: Admin bypass for user {userId} on action {_action}");
                return;
            }

            var controller = context.ActionDescriptor.RouteValues["controller"] ?? string.Empty;

            // Map action names used in views/controllers to corresponding permission checks
            bool allowed = false;
            switch (_action)
            {
                case "View":
                    allowed = roles.Any(r => _permissionService.HasAccessByRoleName(r, controller, "View"));
                    break;
                case "Create":
                    allowed = roles.Any(r => _permissionService.HasAccessByRoleName(r, controller, "Create"));
                    break;
                case "Edit":
                    allowed = roles.Any(r => _permissionService.HasAccessByRoleName(r, controller, "Edit"));
                    break;
                case "Delete":
                    allowed = roles.Any(r => _permissionService.HasAccessByRoleName(r, controller, "Delete"));
                    break;
                default:
                    allowed = roles.Any(r => _permissionService.HasAccessByRoleName(r, controller, _action));
                    break;
            }

            Console.WriteLine($"ModuleAuthorize: user={userId}, roles=[{string.Join(',', roles)}], controller={controller}, action={_action}, allowed={allowed}");

            if (!allowed)
                context.Result = new ForbidResult();
        }
    }
}
