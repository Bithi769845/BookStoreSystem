using BookStore.Models;
using System.Collections.Generic;

namespace BookStoreMVC.Models
{
 public class RecentBookItem
 {
 public int BookId { get; set; }
 public string Title { get; set; } = string.Empty;
 public string AuthorName { get; set; } = string.Empty;
 public string CategoryName { get; set; } = string.Empty;
 public decimal Price { get; set; }
 public DateTime PublishedDate { get; set; }
 }

 public class TopAuthorItem
 {
 public string Name { get; set; } = string.Empty;
 public int BookCount { get; set; }
 }

 public class DashboardViewModel
 {
 public int TotalBooks { get; set; }
 public int TotalAuthors { get; set; }
 public int TotalCategories { get; set; }
 public int TotalUsers { get; set; }

 public List<int> BooksData { get; set; } = new();
 public List<int> AuthorsData { get; set; } = new();
public List<int> CategoriesData { get; set; } = new();
public List<RecentBookItem> RecentBooks { get; set; } = new List<RecentBookItem>();

 // Chart data for books over time
 public List<string> ChartLabels { get; set; } = new List<string>();
 public List<int> ChartData { get; set; } = new List<int>();

 // Category distribution
 public List<string> CategoryLabels { get; set; } = new List<string>();
 public List<int> CategoryData { get; set; } = new List<int>();

 // Top authors by book count
 public List<TopAuthorItem> TopAuthors { get; set; } = new List<TopAuthorItem>();

 // Activity feed (simple messages)
 public List<string> RecentActivities { get; set; } = new List<string>();
 }
}
