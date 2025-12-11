using BookStore.Models;
using BookStore.Models.Identity;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Json;

namespace BookStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BookStore");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var model = new DashboardViewModel();

            try
            {
                var books = await client.GetFromJsonAsync<List<Book>>("api/Book") ?? new List<Book>();
                var authors = await client.GetFromJsonAsync<List<Author>>("api/Author") ?? new List<Author>();
                var categories = await client.GetFromJsonAsync<List<Category>>("api/Category") ?? new List<Category>();

                model.TotalBooks = books.Count;
                model.TotalAuthors = authors.Count;
                model.TotalCategories = categories.Count;
                model.TotalUsers = _userManager.Users.Count(); ;

                model.RecentBooks = books
                    .OrderByDescending(b => b.BookId)
                    .Take(5)
                    .Select(b => new RecentBookItem
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        AuthorName = authors.FirstOrDefault(a => a.AuthorId == b.AuthorId)?.Name ?? "",
                        CategoryName = categories.FirstOrDefault(c => c.CategoryId == b.CategoryId)?.Name ?? "",
                        Price = b.Price,
                        PublishedDate = b.PublishedDate
                    })
                    .ToList();

                // Chart (Books per day based on PublishedDate)
                var groupedBooks = books
                    .GroupBy(b => b.PublishedDate.Date)
                    .OrderBy(g => g.Key)
                    .ToList();

                model.ChartLabels = groupedBooks.Select(g => g.Key.ToString("yyyy-MM-dd")).ToList();
                model.BooksData = groupedBooks.Select(g => g.Count()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard data");
            }

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
