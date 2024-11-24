using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly PaygenixDBContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(PaygenixDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username is already taken.");
            }

            // Validate RoleID
            var role = await _context.Roles.FindAsync(registerDto.RoleID);
            if (role == null)
            {
                return BadRequest("Invalid RoleID.");
            }

            // Hash the password
            //var passwordHash = HashPassword(registerDto.Password);

            // Create a new User object
            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = registerDto.Password,
                RoleID = registerDto.RoleID,
                CreatedDate = DateTime.UtcNow
            };

            // Save the user to the database
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // Helper method to hash passwords
        //private string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(hashedBytes);
        //    }
        //}


        //    [HttpPost("authenticate")]
        //    public async Task<IActionResult> Authenticate([FromBody] LoginDTO loginDto)
        //    {
        //        // Validate user credentials
        //        var user = await _context.Users
        //            .Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        //        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        //        {
        //            return Unauthorized("Invalid username or password.");
        //        }

        //        // Generate JWT Token
        //        var token = GenerateJwtToken(user);
        //        return Ok(new { Token = token });
        //    }

        //    // Helper Method to Generate JWT Token
        //    private string GenerateJwtToken(User user)
        //    {
        //        var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Username),
        //        new Claim(ClaimTypes.Role, user.Role.RoleName),
        //        new Claim("UserID", user.UserID.ToString())
        //    };

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.Now.AddHours(1),
        //            signingCredentials: creds);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }

        //    // Helper Method to Verify Password (assuming passwords are hashed)
        //    private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        //    {
        //        // You can use any hashing mechanism (e.g., BCrypt, SHA256).
        //        // For demonstration, this assumes plain text matching (not recommended in production).
        //        return inputPassword == storedPasswordHash; // Replace with hashing logic.
        //    }
        //}

    }
}