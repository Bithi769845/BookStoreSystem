using BookStore.Models;
using BookStoreMVC.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class BooksController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;


    public BooksController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient GetClient()
    {
        var client = _httpClientFactory.CreateClient("BookStore");
        var token = HttpContext.Session.GetString("JWToken");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

        return client;
    }


    private async Task PopulateViewBags()
    {
        var client = GetClient();
        ViewBag.Authors = await client.GetFromJsonAsync<List<Author>>("api/Author") ?? new List<Author>();
        ViewBag.Categories = await client.GetFromJsonAsync<List<Category>>("api/Category") ?? new List<Category>();
    }

    // GET: Books
    [ModuleAuthorize("View")]

    public async Task<IActionResult> Index()
    {
        var client = GetClient();
        var books = await client.GetFromJsonAsync<List<Book>>("api/Book");
        return View(books);
    }

    // GET: Books/Create
    [ModuleAuthorize("Create")]

    public async Task<IActionResult> Create()
    {
        await PopulateViewBags();
        return View();
    }

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ModuleAuthorize("Create")]

    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags();
            return View(book);
        }

        var client = GetClient();
        await client.PostAsJsonAsync("api/Book", book);
        return RedirectToAction(nameof(Index));
    }

    // GET: Books/Edit/5
    [ModuleAuthorize("Edit")]

    public async Task<IActionResult> Edit(int id)
    {
        var client = GetClient();
        var book = await client.GetFromJsonAsync<Book>($"api/Book/{id}");
        if (book == null) return NotFound();

        await PopulateViewBags();
        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ModuleAuthorize("Edit")]

    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBags();
            return View(book);
        }

        book.BookId = id;
        var client = GetClient();
        var res = await client.PutAsJsonAsync($"api/Book/{id}", book);

        if (!res.IsSuccessStatusCode)
        {
            await PopulateViewBags();
            ModelState.AddModelError("", "Update failed");
            return View(book);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Books/Delete/5
    [ModuleAuthorize("Delete")]

    public async Task<IActionResult> Delete(int id)
    {
        var client = GetClient();
        var book = await client.GetFromJsonAsync<Book>($"api/Book/{id}");
        if (book == null) return NotFound();

        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [ModuleAuthorize("Delete")]

    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = GetClient();
        await client.DeleteAsync($"api/Book/{id}");
        return RedirectToAction(nameof(Index));
    }
}
