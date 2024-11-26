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
    public class UserController : ControllerBase
    {
        private readonly PaygenixDBContext _context;
        private readonly IConfiguration _configuration;
        
        public UserController(PaygenixDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            // Validate RoleID
            var role = await _context.Roles.FindAsync(registerDto.RoleID);
            if (role == null)
            {
                return BadRequest("Invalid RoleID.");
            }

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

            // If the user is registering as an Employee, add details to the Employees table
            if (registerDto.RoleID == 2 && registerDto.EmployeeDetails != null)
            {
                var employeeDto = registerDto.EmployeeDetails;

                // Validate required employee-specific details
                if (string.IsNullOrWhiteSpace(employeeDto.FirstName) || string.IsNullOrWhiteSpace(employeeDto.LastName))
                {
                    return BadRequest("FirstName and LastName are required for employees.");
                }

                var newEmployee = new Employee
                {
                    UserID = newUser.UserID, // Link to the newly created user
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email,
                    PhoneNumber = employeeDto.PhoneNumber,
                    Department = employeeDto.Department,
                    Position = employeeDto.Position,
                    HireDate = DateTime.UtcNow
                };

                await _context.Employees.AddAsync(newEmployee);
                await _context.SaveChangesAsync();
            }


            return Ok("User registered successfully and your userID is : " + newUser.UserID);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Validate user credentials
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.RoleID == loginDto.RoleID);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);
            return Ok(new
            { 
                Token = token ,
                Role = user.Role.RoleName
            });
        }

        // Helper Method to Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("UserID", user.UserID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

           return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper Method to Verify Password (assuming passwords are hashed)
        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return inputPassword == storedPasswordHash; 
        }
    }
}