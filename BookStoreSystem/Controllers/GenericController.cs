using BookStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GenericController<TEntity> : ControllerBase where TEntity : class
    {
        protected readonly ApplDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericController(ApplDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _dbSet.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TEntity updatedEntity)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"{typeof(TEntity).Name} deleted successfully" });
        }
    }

}
