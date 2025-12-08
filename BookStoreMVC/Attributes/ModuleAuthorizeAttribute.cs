using BookStore.Models.Identity;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreMVC.Attributes
{
    public class ModuleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _action;

        public ModuleAuthorizeAttribute(string action)
        {
            _action = action;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();

            if (userManager == null || permissionService == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userId = userManager.GetUserId(context.HttpContext.User);
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roles = userManager.GetRolesAsync(user).Result;
            if (roles == null || roles.Count == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            var controller = context.ActionDescriptor.RouteValues["controller"] ?? string.Empty;

            bool allowed = roles.Any(r => permissionService.HasAccessByRoleName(r, controller, _action));

            if (!allowed)
                context.Result = new ForbidResult();
        }

    }

}
