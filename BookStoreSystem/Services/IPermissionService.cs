using BookStore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreSystem.Services
{
    public interface IPermissionService
    {
        bool HasAccess(string roleId, string moduleName, string action);
        Task<List<Module>> GetModulesForUser(string userId);
    }
}
