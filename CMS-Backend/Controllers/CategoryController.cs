using CMS_Backend.Data;
using CMS_Backend.Models;
using CMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize] 
	public class CategoryController : ControllerBase
	{
		private readonly ApplicationDbContext dbContext;

		public CategoryController(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		[HttpGet]
		public IActionResult GetAllCategories()
		{
			return Ok(dbContext.Categories.ToList());
		}
		[HttpGet("{id:int}")]
		public IActionResult GetCategoryById(Guid id)
		{
			var category = dbContext.Categories.Find(id);

			if (category == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Category not found"
				});
			}

			return Ok(new
			{
				success = true,
				message = "Category fetched successfully",
				data = category
			});
		}
		[HttpPost]
		[Authorize(Roles = "admin")]
		public IActionResult AddCategory(AddCategoryDto addCategoryDto)
		{
			var categoryEntity = new Category
			{
				Name = addCategoryDto.Name
			};

			dbContext.Categories.Add(categoryEntity);
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Category added successfully",
				data = categoryEntity
			});
		}
		[HttpPut("{id:guid}")]
		[Authorize(Roles = "admin")]
		public IActionResult UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
		{
			var category = dbContext.Categories.Find(id);

			if (category == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Category not found"
				});
			}

			category.Name = updateCategoryDto.Name;
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Category updated successfully",
				data = category
			});
		}
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "admin")]
		public IActionResult DeleteCategory(Guid id)
		{
			var category = dbContext.Categories.Find(id);

			if (category == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Category not found"
				});
			}

			dbContext.Categories.Remove(category);
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Category deleted successfully"
			});
		}
	}
}
