using System.ComponentModel.DataAnnotations.Schema;
using BookStore.Models.Identity;

namespace BookStore.Models
{
 public class RoleModuleAccess
 {
 public int Id { get; set; }
 public string RoleId { get; set; } = string.Empty;
 public int ModuleId { get; set; }
 public bool CanView { get; set; }
 public bool CanCreate { get; set; }
 public bool CanEdit { get; set; }
 public bool CanDelete { get; set; }

 // Navigation
 [ForeignKey("RoleId")]
 public ApplicationRole? Role { get; set; }
 [ForeignKey("ModuleId")]
 public Module? Module { get; set; }
 }
}
