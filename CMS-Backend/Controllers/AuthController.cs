using CMS_Backend.Data;
using CMS_Backend.Models.Auth;
using CMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly ApplicationDbContext _context;
	private readonly JwtService _jwtService;

	public AuthController(ApplicationDbContext context, JwtService jwtService)
	{
		_context = context;
		_jwtService = jwtService;
	}
	[HttpPost("signup")]
	public IActionResult Signup(RegisterDto dto)
	{
		if (_context.Users.Any(x => x.Email == dto.Email))
		{
			return BadRequest(new
			{
				success = false,
				message = "Email already exists"
			});
		}
		var user = new User
		{
			Id = Guid.NewGuid(),
			Name = dto.Name,
			Email = dto.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
		};

		_context.Users.Add(user);
		_context.SaveChanges();

		return Ok(new
		{
			success = true,
			message = "Signup successful"
		});
	}
	[HttpPost("login")]
	public IActionResult Login(LoginDto dto)
	{
		var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

		if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
		{
			return Unauthorized(new
			{
				success = false,
				message = "Invalid email or password"
			});
		}

		var token = _jwtService.GenerateToken(user);

		return Ok(new
		{
			success = true,
			message = "Login successful",
			token = token
		});
	}
	[HttpPost("forgot-password")]
	public IActionResult ForgotPassword(ForgotPasswordDto dto)
	{
		var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

		if (user == null)
		{
			return NotFound(new
			{
				success = false,
				message = "User not found"
			});
		}

		return Ok(new
		{
			success = true,
			message = "Password reset link sent"
		});
	}
}
