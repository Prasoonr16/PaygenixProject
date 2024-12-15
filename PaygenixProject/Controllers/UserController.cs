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
using NewPayGenixAPI.Repositories;
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
        private readonly IAdminRepository _adminRepository;

        // Logger instance
        private static readonly ILog _logger = LogManager.GetLogger(typeof(UserController));
        public UserController(PaygenixDBContext context, IConfiguration configuration, IAdminRepository adminRepository)
        {
            _context = context;
            _configuration = configuration;
            _adminRepository = adminRepository;
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

                    // Log failed login
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Login Failed",
                        PerformedBy = loginDto.Username,
                        Timestamp = DateTime.Now,
                        Details = "Invalid credentials",
                    });
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

                // Log successful login
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Login Successful",
                    PerformedBy = user.Username,
                    Timestamp = DateTime.Now,
                    Details = "Login successfull"
                }); 

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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                // Step 1: Verify if the user exists using UserID
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == resetPasswordDto.UserID);


                if (user == null) 
                {
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Reset Password Failed",
                        PerformedBy = "User ID : "+resetPasswordDto.UserID.ToString(),
                        Timestamp = DateTime.Now,
                        Details = "User not found",
                    });
                    return NotFound("User not found");
                }

                _logger.Info($"Reset Password attempt for username: {user.Username}");

                // Step 2: Verify the existing password
                if (user.PasswordHash != resetPasswordDto.ExistingPassword)
                {
                    await _adminRepository.LogAuditTrailAsync(new AuditTrail
                    {
                        Action = "Reset Password Failed",
                        PerformedBy = "User ID : " + resetPasswordDto.UserID.ToString(),
                        Timestamp = DateTime.Now,
                        Details = "Incorrect existing password",
                    });
                    _logger.Warn($"Reset Password failed for username: {user.Username}");
                    return BadRequest("Existing password is incorrect.");
                }

                // Step 3: Update the password in the database
                user.PasswordHash = resetPasswordDto.NewPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Reset Password Successful",
                    PerformedBy = "User ID : "+resetPasswordDto.UserID.ToString(),
                    Timestamp = DateTime.Now,
                    Details = "Password reset successfully",
                });

                return Ok("Password has been reset successfully!");
            }
            catch (Exception ex)
            {
                await _adminRepository.LogAuditTrailAsync(new AuditTrail
                {
                    Action = "Reset Password Error",
                    PerformedBy = "User ID : "+resetPasswordDto.UserID.ToString(),
                    Timestamp = DateTime.Now,
                    Details = $"Exception: {ex.Message}",
                });
                _logger.Error($"An error occurred during password reset: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
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

        // Helper Method to Verify Password 
        private bool VerifyPassword(string inputPassword, string storedPasswordHash)
        {
            return inputPassword == storedPasswordHash;
        }
    }
}