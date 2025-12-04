using System.Collections.Generic;

namespace BookStoreMVC.Models
{
 public class ModuleAccessModel
 {
 public int ModuleId { get; set; }
 public string ModuleName { get; set; } = string.Empty;
 public bool CanView { get; set; }
 public bool CanCreate { get; set; }
 public bool CanEdit { get; set; }
 public bool CanDelete { get; set; }
 }

 public class RoleModuleViewModel
 {
 public string RoleId { get; set; } = string.Empty;
 public string RoleName { get; set; } = string.Empty;
 public List<ModuleAccessModel> Modules { get; set; } = new List<ModuleAccessModel>();
 }
}
