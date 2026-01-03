using CMS_Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class DashboardController : ControllerBase
	{
		private readonly ApplicationDbContext dbContext;

		public DashboardController(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		[HttpGet("details")]
		public IActionResult GetDetails()
		{
			var data = new
			{
				category = dbContext.Categories.Count(),
				//product = dbContext.Products.Count(),
				//bill = dbContext.Bills.Count(),
				user = dbContext.Users.Count()
			};

			return Ok(new
			{
				success = true,
				data
			});
		}
	}
}
