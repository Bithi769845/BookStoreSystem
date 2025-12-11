using BookStore.DTOs;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text;
using BookStore.Models.Identity;
using System.Security.Claims;

namespace BookStoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _httpClient = httpClientFactory.CreateClient("BookStore");
            _httpClient.BaseAddress = new Uri("https://localhost:44364/"); // API base URL
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var payload = JsonConvert.SerializeObject(new
                {
                    email = model.Email,
                    password = model.Password
                });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Auth/login", content);

                var respText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login API status: {(int)response.StatusCode} {response.ReasonPhrase}; body: {respText}");

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Invalid email or password";
                    return View(model);
                }

                var result = respText;
                var tokenObj = JsonConvert.DeserializeObject<TokenResponse>(result);

                if (tokenObj == null || string.IsNullOrEmpty(tokenObj.Token))
                {
                    model.ErrorMessage = "Login failed. Token missing.";
                    return View(model);
                }

                // Save JWT in session (keep JWT for API calls)
                HttpContext.Session.SetString("JWToken", tokenObj.Token);

                // Also sign in the user with cookie so MVC Authorize works
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Get roles and include them in the cookie claims to ensure [Authorize(Roles=..)] works
                    var roles = await _userManager.GetRolesAsync(user);
                    var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

                    // Sign in with role claims included
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims: roleClaims);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AccountController.Login exception: {ex}");
                model.ErrorMessage = $"Login failed: {ex.Message}";
                return View(model);
            }
        }


        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegistrationViewModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var payload = JsonConvert.SerializeObject(new
                {
                    fullName = model.FullName,
                    email = model.Email,
                    password = model.Password,
                    role = model.Role
                });

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToAction("Login");
                }

                var apiError = await response.Content.ReadAsStringAsync();

                try
                {
                    var errors = JsonConvert.DeserializeObject<List<IdentityError>>(apiError);
                    model.ErrorMessages = errors?.Select(e => e.Description).ToList() ?? new List<string> { apiError };
                }
                catch
                {
                    model.ErrorMessages = new List<string> { apiError };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                model.ErrorMessages = new List<string> { $"Registration failed: {ex.Message}" };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Remove JWT from session
            HttpContext.Session.Remove("JWToken");
            // Sign out cookie
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public class TokenResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }
        }
    }
}
