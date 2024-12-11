using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewPayGenixAPI.Data;
using NewPayGenixAPI.DTO;
using NewPayGenixAPI.Models;
using PaygenixProject.DTO;
using PaygenixProject.Models;

namespace NewPayGenixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly PaygenixDBContext _context;
        private readonly IConfiguration _configuration;

        // Logger instance
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserController));
        public UserController(PaygenixDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try {
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
                    CreatedDate = DateTime.UtcNow,
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
                        ActiveStatus = employeeDto.ActiveStatus,
                        HireDate = DateTime.UtcNow,
                    };

                    await _context.Employees.AddAsync(newEmployee);
                    await _context.SaveChangesAsync();
                }


                return Ok("User registered successfully and your userID is : " + newUser.UserID);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.Error($"Database error occurred while registering user: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while registering the user.");
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while registering user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                _logger.Info($"Login attempt for username: {loginDto.Username}");

                // Validate user credentials
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.RoleID == loginDto.RoleID);

                if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    _logger.Warn($"Login failed for username: {loginDto.Username}");
                    return Unauthorized("Invalid username or password.");
                }

                // Generate JWT Token
                var token = GenerateAccessToken(user);

                // Generate Refresh Token
                var refreshToken = GenerateRefreshToken();
                refreshToken.UserID = user.UserID;

                // Store Refresh Token in the database
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();

                _logger.Info($"Login successful for username: {loginDto.Username}, Role: {user.Role.RoleName}");


                return Ok(new
                {
                    Token = token,
                    refreshToken = refreshToken.Token,
                    Role = user.Role.RoleName
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred during login: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                // Find the refresh token in the database
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed)
                {
                    return Unauthorized("Invalid or expired refresh token.");
                }

                // Check if the refresh token is expired
                if (storedToken.Expires < DateTime.UtcNow)
                {
                    return Unauthorized("Refresh token has expired.");
                }

                // Get the user associated with the refresh token
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserID == storedToken.UserID);

                if (user == null)
                {
                    return Unauthorized("Invalid refresh token.");
                }

                // Generate a new JWT token
                var newJwtToken = GenerateAccessToken(user);

                // Mark the refresh token as used
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                // Generate a new refresh token
                var newRefreshToken = GenerateRefreshToken();
                newRefreshToken.UserID = user.UserID;

                // Store the new refresh token
                await _context.RefreshTokens.AddAsync(newRefreshToken);
                await _context.SaveChangesAsync();

                // Return new tokens
                return Ok(new
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken.Token
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred during refresh token: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }


        // Method to Generate JWT Access Token
        private string GenerateAccessToken(User user)
        {
            try
            {
                var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, user.Username),
               // new Claim(ClaimTypes.Role, user.Role.RoleName),
                
                    new Claim("username", user.Username),
                    new Claim("role", user.Role.RoleName),
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

                _logger.Debug($"JWT access token generated for UserID: {user.UserID}");

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while generating the JWT token: {ex.Message}");
                throw new InvalidOperationException("Error generating the access token.", ex);
            }
        }

        // Method to generate a Refresh Token
        private RefreshToken GenerateRefreshToken()
        {
            try
            {
                var token = new RefreshToken
                {
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    Expires = DateTime.UtcNow.AddDays(7), // Refresh token expires in 7 days
                    IsRevoked = false,
                    IsUsed = false
                };


                _logger.Debug("Refresh token generated.");

                return token;
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred while generating the refresh token: {ex.Message}");
                throw new InvalidOperationException("Error generating the refresh token.", ex);
            }
        }

        //[HttpPost("logout")]
        //[Authorize] // Ensure only authenticated users can log out
        //public async Task<IActionResult> Logout([FromBody] LogoutDTO logoutDto)
        //{
        //    try
        //    {
        //        // Get the current user from the claims
        //        var userId = int.Parse(User.FindFirst("UserID")?.Value);

        //        _logger.Info($"Logout attempt for UserID: {userId}");

        //        // If a specific refresh token is provided, revoke it
        //        if (!string.IsNullOrEmpty(logoutDto.RefreshToken))
        //        {
        //            var token = await _context.RefreshTokens
        //                .FirstOrDefaultAsync(rt => rt.Token == logoutDto.RefreshToken && rt.UserID == userId);

        //            if (token == null)
        //            {
        //                _logger.Warn($"Invalid refresh token for UserID: {userId}");
        //                return BadRequest("Invalid refresh token.");
        //            }

        //            // Revoke the token
        //            token.IsRevoked = true;
        //            _context.RefreshTokens.Update(token);
        //            await _context.SaveChangesAsync();

        //            _logger.Info($"Logout successful for UserID: {userId} (specific token revoked)");
        //            return Ok("Logged out successfully.");
        //        }

        //        // Revoke all refresh tokens for the user (optional)
        //        var allTokens = await _context.RefreshTokens
        //            .Where(rt => rt.UserID == userId && !rt.IsRevoked && !rt.IsUsed)
        //            .ToListAsync();

        //        foreach (var refreshToken in allTokens)
        //        {
        //            refreshToken.IsRevoked = true;
        //        }

        //        _context.RefreshTokens.UpdateRange(allTokens);
        //        await _context.SaveChangesAsync();

        //        _logger.Info($"Logout successful for UserID: {userId} (all tokens revoked)");
        //        return Ok("Logged out successfully from all devices.");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error($"An error occurred during logout: {ex.Message}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        //    }
        //}

        // Helper Method to Verify Password 
        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return inputPassword == storedPasswordHash;
        }
    }
}