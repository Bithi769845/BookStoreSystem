using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ApplDbContext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryController(ApplDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Category>();
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _dbSet.ToListAsync();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _dbSet.FindAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _dbSet.AddAsync(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updated)
        {
            if (id != updated.CategoryId) return BadRequest("ID mismatch");

            var existing = await _dbSet.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = updated.Name;
            existing.Description = updated.Description;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dbSet.FindAsync(id);
            if (category == null) return NotFound();

            _dbSet.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
