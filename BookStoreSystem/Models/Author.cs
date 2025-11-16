using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 20)]
        public string Biography { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}
