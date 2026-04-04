using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UygAPI.DTOs.Auth;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IJwtService jwtService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return BadRequest(new AuthResponseDto { Success = false, Message = "Bu email zaten kayıtlı." });

            var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
                return BadRequest(new AuthResponseDto { Success = false, Message = "Bu kullanıcı adı zaten alınmış." });

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new Role { Name = "User", Slug = "user" });

                await _userManager.AddToRoleAsync(user, "User");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(24),
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Message = "Kayıt başarılı."
                });
            }

            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Kayıt başarısız: " + string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new AuthResponseDto { Success = false, Message = "Email veya şifre hatalı." });

            if (!user.IsActive)
                return Unauthorized(new AuthResponseDto { Success = false, Message = "Hesabınız aktif değil." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(24),
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Message = "Giriş başarılı."
                });
            }

            return Unauthorized(new AuthResponseDto { Success = false, Message = "Email veya şifre hatalı." });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Çıkış başarılı." });
        }

        [HttpPost("create-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return BadRequest("Rol adı boş olamaz.");

            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Bu rol zaten var.");

            var role = new Role
            {
                Name = roleName,
                Slug = roleName.ToLower(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Ok(new { message = $"{roleName} rolü oluşturuldu." });

            return BadRequest(result.Errors);
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                return BadRequest("Böyle bir rol yok.");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (result.Succeeded)
                return Ok(new { message = $"{user.UserName} kullanıcısına {model.RoleName} rolü atandı." });

            return BadRequest(result.Errors);
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.IsActive,
                    user.CreatedAt,
                    Roles = roles
                });
            }

            return Ok(userList);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.IsActive,
                user.CreatedAt,
                Roles = roles
            });
        }
    }
}