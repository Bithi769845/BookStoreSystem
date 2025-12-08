using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreSystem.Services
{
    public interface IPermissionService
    {
        Task<List<Module>> GetModulesForUser(string userId);
        bool HasAccess(string roleId, string moduleName, string action);

        // এটা add করো
        bool HasAccessByRoleName(string roleName, string moduleName, string action);
    }

}
