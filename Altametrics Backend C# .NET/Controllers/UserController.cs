using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Text;
using System.Security.Claims;
using Altametrics_Backend_C__.NET.Data;
using Altametrics_Backend_C__.NET.Models.DTOs.Auth;
using Altametrics_Backend_C__.NET.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("auth")]
public class UserController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;



    public UserController(AppDBContext context, IMapper mapper, IConfiguration config)
    {
        _context = context;
        _mapper = mapper;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || !IsValidEmail(model.Email))
            return BadRequest("Invalid email format.");
        var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
        if (exists)
            return BadRequest("User already exists.");

        if (!IsValidPassword(model.Password))
            return BadRequest("Password must be at least 8 characters long, contain at least one number, and one special character.");

        var hashed = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User { Email = model.Email, PasswordHash = hashed };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = GenerateJwtToken(user);
        var response = _mapper.Map<AuthRespModel>(user);
        response.Token = token;

        return Ok(response);
    }

    private string GenerateJwtToken(User user)
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY");
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            throw new InvalidOperationException("JWT environment variables are not properly set.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    //instead of writing a custom attribute, due to time constraints, I will use a simple method to validate the password.
    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasNumber = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasNumber && hasSpecial;
    }
}
