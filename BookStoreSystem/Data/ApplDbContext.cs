using BookStore.Models;
using BookStore.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Data
{
    public class ApplDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplDbContext(DbContextOptions<ApplDbContext>options) : base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
