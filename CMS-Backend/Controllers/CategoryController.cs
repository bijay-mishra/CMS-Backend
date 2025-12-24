using CMS_Backend.Data;
using CMS_Backend.Models;
using CMS_Backend.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

		[HttpGet]

		[Route("{id:guid}")]
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
        public IActionResult AddCategory(AddCategoryDto addCategoryDto)
        {

            var categoryEntity = new Category()
            {
                Name = addCategoryDto.Name,
                Email = addCategoryDto.Email,
                Phone = addCategoryDto.Phone,
                Salary = addCategoryDto.Salary
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
        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            var category = dbContext.Categories.Find(id);
            if (category is null)
            {
                return NotFound();
            }
            category.Name = updateCategoryDto.Name;
            category.Email = updateCategoryDto.Email;
            category.Phone = updateCategoryDto.Phone;
            category.Salary = updateCategoryDto.Salary;
            dbContext.SaveChanges();
            return Ok(new
            {
                sucess=true,
                message="Category updated sucessfully",
                data=category
            });

        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteCategory(Guid id)
        {
            var category = dbContext.Categories.Find(id);
            if (category is null)
            {
                return NotFound();
            }
            dbContext.Categories.Remove(category);
            dbContext.SaveChanges();
            return Ok(new
            {
                sucess = true,
                message = "Category deleted sucessfully"
			});

               
        }
    }
}
