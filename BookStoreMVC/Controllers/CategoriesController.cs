using BookStore.Models;
using BookStoreMVC.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookStoreMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
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


        // GET: Categories
        [ModuleAuthorize("View")]

        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var categories = await client.GetFromJsonAsync<List<Category>>("api/Category");
            return View(categories);
        }

        // GET: Categories/Create
        [ModuleAuthorize("Create")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModuleAuthorize("Create")]

        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            var client = GetClient();
            await client.PostAsJsonAsync("api/Category", category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        [ModuleAuthorize("Edit")]

        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var category = await client.GetFromJsonAsync<Category>($"api/Category/{id}");
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModuleAuthorize("Edit")]

        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (!ModelState.IsValid) return View(category);

            category.CategoryId = id;
            var client = GetClient();
            var res = await client.PutAsJsonAsync($"api/Category/{id}", category);

            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Update failed");
                return View(category);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        [ModuleAuthorize("Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var category = await client.GetFromJsonAsync<Category>($"api/Category/{id}");
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ModuleAuthorize("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/Category/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
