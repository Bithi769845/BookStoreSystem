using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public DateTime PublishedDate { get; set; }
        [Range(0.01, 10000)]
        public decimal Price { get; set; }
    }
}
