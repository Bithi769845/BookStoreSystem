using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly ApplDbContext _context;
    private readonly DbSet<Book> _dbSet;

    public BookController(ApplDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Book>();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _dbSet.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var book = await _dbSet.FindAsync(id);
        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        await _dbSet.AddAsync(book);
        await _context.SaveChangesAsync();
        return Ok(book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Book updated)
    {
        if (id != updated.BookId) return BadRequest("ID mismatch");

        var existing = await _dbSet.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = updated.Title;
        existing.Price = updated.Price;
        existing.AuthorId = updated.AuthorId;
        existing.CategoryId = updated.CategoryId;
        existing.PublishedDate = updated.PublishedDate;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _dbSet.FindAsync(id);
        if (book == null) return NotFound();

        _dbSet.Remove(book);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Book deleted successfully" });
    }
}



//public class BookController : ControllerBase
//{
//    private readonly ApplDbContext _context;
//    public BookController(ApplDbContext context)
//    {
//        _context = context;

//    }
//    [HttpGet]
//    public async Task<IActionResult> GetBooks()
//    {
//        var books = await _context.Books.Include(b => b.Author)
//                                        .Include(b => b.Category)
//                                        .ToListAsync();
//        return Ok(books);
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetBook(int id)
//    {
//        var book = await _context.Books.Include(b => b.Author)
//                                       .Include(b => b.Category)
//                                       .FirstOrDefaultAsync(b => b.BookId == id);
//        if (book == null)
//        {
//            return NotFound();
//        }
//        return Ok(book);
//    }

//    [HttpPost]
//    public async Task<IActionResult> AddBook([FromBody]Book book)
//    {
//        await _context.Books.AddAsync(book);
//        await _context.SaveChangesAsync();
//        return Ok(book);

//    }
//    [HttpPut("{id}")]
//    public async Task<IActionResult> UpdateBook(int id,  [FromBody]Book book)
//    {
//        var dbBook = await _context.Books.FindAsync(id);
//        if(dbBook == null) return NotFound();

//        dbBook.Title = book.Title;
//        dbBook.Price= book.Price;
//        dbBook.AuthorId= book.AuthorId;
//        dbBook.CategoryId= book.CategoryId;

//        await _context.SaveChangesAsync();
//        return Ok(dbBook);
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteBook(int id)
//    {
//        var dbBook = await _context.Books.FindAsync(id);
//        if (dbBook == null) return NotFound();

//        _context.Books.Remove(dbBook);
//        await _context.SaveChangesAsync();
//        return Ok(new {message="Book deleted Successfully"});
//    }
//}