using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
 public class Module
 {
 [Key]
 public int ModuleId { get; set; }

 [Column("ModuleName")]
 public string Name { get; set; } = string.Empty;
 public string Description { get; set; } = string.Empty;

 // Navigation
 public ICollection<RoleModuleAccess>? RoleModuleAccesses { get; set; }
 }
}
