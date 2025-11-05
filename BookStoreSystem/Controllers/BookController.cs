using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//Endpoints:
    //GET / api / books → Get all books
    //GET /api/books/{id} → Get single book by ID
    //POST /api/books → Add new book
    //PUT / api / books /{ id} → Update book
    //DELETE /api/books/{id} → Delete book

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : GenericController<Book>
    {
        public BookController(ApplDbContext context) : base(context) { 
        
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
}
