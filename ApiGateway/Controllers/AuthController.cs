using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Generate JWT token for authentication
        /// </summary>
        /// <param name="request">Login request with username and password</param>
        /// <returns>Returns JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Login attempt with empty credentials");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Demo authentication - replace with real authentication
            if (request.Username != "admin" || request.Password != "admin123")
            {
                _logger.LogWarning($"Failed login attempt for user: {request.Username}");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(request.Username, "user");
            _logger.LogInformation($"User {request.Username} logged in successfully");

            return Ok(new
            {
                token = token,
                message = "Login successful",
                expiresIn = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes")
            });
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>Returns new JWT token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // In a real application, validate the refresh token against a database
            // For demo purposes, we'll just generate a new token
            var token = GenerateJwtToken("admin", "user");
            _logger.LogInformation("Token refreshed successfully");

            return Ok(new
            {
                token = token,
                message = "Token refreshed successfully",
                expiresIn = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes")
            });
        }

        private string GenerateJwtToken(string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? "your-super-secret-key-change-in-production-1234567890";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("scope", "read write")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetValue<string>("Issuer"),
                audience: jwtSettings.GetValue<string>("Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpirationMinutes")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; }
    }
}
