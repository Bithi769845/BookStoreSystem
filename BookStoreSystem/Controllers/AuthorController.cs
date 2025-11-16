using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplDbContext _context;
        private readonly DbSet<Author> _dbSet;

        public AuthorController(ApplDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Author>();
        }

        // GET: api/Author
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _dbSet.ToListAsync();
            return Ok(authors);
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _dbSet.FindAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        // POST: api/Author
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Author author)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _dbSet.AddAsync(author);
            await _context.SaveChangesAsync();
            return Ok(author);
        }

        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Author updated)
        {
            if (id != updated.AuthorId) return BadRequest("ID mismatch");

            var existing = await _dbSet.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = updated.Name;
            existing.Biography = updated.Biography;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _dbSet.FindAsync(id);
            if (author == null) return NotFound();

            _dbSet.Remove(author);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Author deleted successfully" });
        }
    }
}
