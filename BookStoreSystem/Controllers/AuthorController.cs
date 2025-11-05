using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : GenericController<Author>
    {
        public AuthorController(ApplDbContext context) : base(context) { }
    }

}
