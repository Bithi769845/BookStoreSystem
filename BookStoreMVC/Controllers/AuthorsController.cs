using BookStore.Models;
using BookStoreMVC.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
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

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("BookStore");
            var token = HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

            return client;
        }


        // GET: Authors
        [ModuleAuthorize("View")]

        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var authors = await client.GetFromJsonAsync<List<Author>>("api/Author");
            return View(authors);
        }

        // GET: Authors/Create
        [ModuleAuthorize("Create")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModuleAuthorize("Create")]

        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) return View(author);

            var client = GetClient();
            await client.PostAsJsonAsync("api/Author", author);
            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Edit/5
        [ModuleAuthorize("Edit")]

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
        [ModuleAuthorize("Edit")]
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
        [ModuleAuthorize("Delete")]
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
        [ModuleAuthorize("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/Author/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
