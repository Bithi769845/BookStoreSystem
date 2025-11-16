using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace BookStoreMVC.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient() => _httpClientFactory.CreateClient("BookStoreAPI");

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var authors = await client.GetFromJsonAsync<List<Author>>("api/Author");
            return View(authors);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) return View(author);

            var client = GetClient();
            await client.PostAsJsonAsync("api/Author", author);
            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var author = await client.GetFromJsonAsync<Author>($"api/Author/{id}");
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (!ModelState.IsValid) return View(author);

            author.AuthorId = id;
            var client = GetClient();
            var res = await client.PutAsJsonAsync($"api/Author/{id}", author);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Update failed");
                return View(author);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var author = await client.GetFromJsonAsync<Author>($"api/Author/{id}");
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/Author/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
