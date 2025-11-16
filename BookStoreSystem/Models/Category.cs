using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}
