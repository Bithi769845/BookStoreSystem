namespace BookStore.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}
