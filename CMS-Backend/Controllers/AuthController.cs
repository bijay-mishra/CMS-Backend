using CMS_Backend.Data;
using CMS_Backend.Models.Auth;
using CMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMS_Backend.Controllers
{
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
				Name = dto.Name,
				ContactNumber = dto.ContactNumber,
				Email = dto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
				Role = "user",
				Status = "false"
			};

			_context.Users.Add(user);
			_context.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Registered successfully. Wait for admin approval"
			});
		}
		[HttpPost("login")]
		public IActionResult Login(LoginDto dto)
		{
			var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

			if (user == null)
			{
				return Unauthorized(new
				{
					success = false,
					message = "Invalid email or password"
				});
			}

			bool isPasswordValid = false;

			// Check if stored password is already hashed
			if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash.StartsWith("$2a$"))
			{
				// Verify BCrypt password
				isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
			}
			else
			{
				// Legacy plain-text password
				isPasswordValid = user.PasswordHash == dto.Password;

				// If valid, hash it and update the DB
				if (isPasswordValid)
				{
					user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
					_context.SaveChanges();
				}
			}

			if (!isPasswordValid)
			{
				return Unauthorized(new
				{
					success = false,
					message = "Invalid email or password"
				});
			}

			if (user.Status != "true")
			{
				return Unauthorized(new
				{
					success = false,
					message = "Wait for admin approval"
				});
			}

			var token = _jwtService.GenerateToken(user);

			return Ok(new
			{
				success = true,
				message = "Login successful",
				token
			});
		}

		[Authorize]
		[HttpGet("checkToken")]
		public IActionResult CheckToken()
		{
			return Ok(new
			{
				success = true,
				message = "Token is valid"
			});
		}
		[Authorize(Roles = "admin")]
		[HttpGet("getAllUser")]
		public IActionResult GetAllUser()
		{
			var users = _context.Users
				.Where(x => x.Role == "user")
				.Select(x => new
				{
					x.Id,
					x.Name,
					x.ContactNumber,
					x.Email,
					x.Status,
					x.Role
				})
				.ToList();

			return Ok(new
			{
				success = true,
				data = users
			});
		}
		[Authorize(Roles = "admin")]
		[HttpPost("updateUserStatus")]
		public IActionResult UpdateUserStatus(UpdateUserStatusDto dto)
		{
			var user = _context.Users.Find(dto.UserId);

			if (user == null)
			{
				return NotFound(new
				{
					success = false,
					message = "User not found"
				});
			}

			user.Status = dto.Status;
			_context.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "User status updated successfully"
			});
		}
		[Authorize]
		[HttpPost("changePassword")]
		public IActionResult ChangePassword(ChangePasswordDto dto)
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			var user = _context.Users.FirstOrDefault(x => x.Email == email);

			if (user == null ||
				!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
			{
				return BadRequest(new
				{
					success = false,
					message = "Incorrect old password"
				});
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
			_context.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Password updated successfully"
			});
		}
		[HttpPost("forgot-password")]
		public IActionResult ForgotPassword(ForgotPasswordDto dto)
		{
			var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

			return Ok(new
			{
				success = true,
				message = "If the email exists, a password reset link has been sent"
			});
		}
	}
}
