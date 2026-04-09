using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Thakkirni.API.Application.Interfaces;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(loginDto, cancellationToken);
        return Ok(response);
=======
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Thakkirni.API.Data;
using Thakkirni.API.DTOs;

namespace Thakkirni.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Agency)
                .Include(u => u.Department)
                .Include(u => u.Section)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("DepartmentId", user.DepartmentId?.ToString() ?? "")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return full user details (including org hierarchy) in the response body.
            // The JWT token itself only carries the minimal claims needed for authorization.
            return Ok(new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    NationalId = user.NationalId,
                    Role = user.Role,
                    Avatar = user.Avatar,
                    JobTitle = user.JobTitle,
                    AgencyId = user.AgencyId,
                    AgencyName = user.Agency?.Name,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = user.Department?.Name,
                    SectionId = user.SectionId,
                    SectionName = user.Section?.Name
                }
            });
        }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    }
}
