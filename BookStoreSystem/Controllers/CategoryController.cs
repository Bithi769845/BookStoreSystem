using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : GenericController<Category>
    {
        public CategoryController(ApplDbContext context) : base(context) { }
    }

}
