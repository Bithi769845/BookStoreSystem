using BookStore.DTOs;
using BookStore.Helpers;
using BookStore.Models.Identity;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly IPermissionService _permissionService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            JwtTokenGenerator jwtGenerator,
            IPermissionService permissionService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtGenerator = jwtGenerator;
            _permissionService = permissionService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                Console.WriteLine($"API Register called for: {dto?.Email}");

                var userExists = await _userManager.FindByEmailAsync(dto.Email);
                if (userExists != null)
                {
                    Console.WriteLine("Register: user already exists");
                    return BadRequest("User already exists!");
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Register: create failed. Errors: {string.Join(';', result.Errors.Select(e => e.Description))}");
                    return BadRequest(result.Errors);
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = dto.Role });
                }

                await _userManager.AddToRoleAsync(user, dto.Role);
                Console.WriteLine($"Register: user created {user.Id}");
                return Ok("User registered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register exception: {ex}");
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                Console.WriteLine($"API Login attempt for: {dto?.Email}");

                var user = await _userManager.FindByEmailAsync(dto.Email);
                Console.WriteLine(user == null ? "Login: User not found" : $"Login: User found {user.Id}");
                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                Console.WriteLine($"Login: Password valid: {isPasswordValid}");
                if (!isPasswordValid)
                {
                    return Unauthorized("Invalid credentials");
                }

                // 1️⃣ Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // 2️⃣ Get allowed modules for this user from DB (via Role → Module mapping)
                var modules = await _permissionService.GetModulesForUser(user.Id);
                Console.WriteLine($"Login: modules count for user: {modules?.Count}");

                // 3️⃣ Convert modules into claims
                var moduleClaims = modules.Select(m => new System.Security.Claims.Claim("Module", m.Name)).ToList();

                // 4️⃣ Generate token (send roles + modules)
                var token = _jwtGenerator.GenerateToken(user, roles, moduleClaims);
                Console.WriteLine($"Login: token generated length={token?.Length}");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex}");
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
