using CMS_Backend.Data;
using CMS_Backend.Models;
using CMS_Backend.Models.Entities;
using CMS_Backend.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly ApplicationDbContext dbContext;

		public ProductController(ApplicationDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		[HttpGet]
		public IActionResult GetAllProducts()
		{
			var products = from p in dbContext.Products
						   join c in dbContext.Categories
						   on p.CategoryId equals c.Id
						   select new
						   {
							   p.Id,
							   p.Name,
							   p.Description,
							   p.Price,
							   p.Status,
							   CategoryId = c.Id,
							   CategoryName = c.Name
						   };

			return Ok(products.ToList());
		}

		[HttpGet("{id:int}")]
		public IActionResult GetProductById(int id)
		{
			var product = dbContext.Products.Find(id);

			if (product == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Product not found"
				});
			}

			return Ok(new
			{
				success = true,
				message = "Product fetched successfully",
				data = product
			});
		}
		[HttpGet("byCategory/{categoryId:int}")]
		public IActionResult GetProductByCategory(int categoryId)
		{
			var products = dbContext.Products
				.Where(x => x.CategoryId == categoryId && x.Status == "true")
				.Select(x => new
				{
					x.Id,
					x.Name
				})
				.ToList();

			return Ok(products);
		}
		[HttpPost]
		[Authorize]
		public IActionResult AddProduct(AddProductDto addProductDto)
		{
			var product = new Product
			{
				Name = addProductDto.Name,
				CategoryId = addProductDto.CategoryId,
				Description = addProductDto.Description,
				Price = addProductDto.Price,
				Status = "true"
			};

			dbContext.Products.Add(product);
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Product added successfully",
				data = product
			});
		}
		[HttpPut("{id:int}")]
		[Authorize(Roles = "admin")]
		public IActionResult UpdateProduct(int id, UpdateProductDto updateProductDto)
		{
			var product = dbContext.Products.Find(id);

			if (product == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Product not found"
				});
			}

			product.Name = updateProductDto.Name;
			product.CategoryId = updateProductDto.CategoryId;
			product.Description = updateProductDto.Description;
			product.Price = updateProductDto.Price;

			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Product updated successfully",
				data = product
			});
		}
		[HttpDelete("{id:int}")]
		[Authorize(Roles = "admin")]
		public IActionResult DeleteProduct(int id)
		{
			var product = dbContext.Products.Find(id);

			if (product == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Product not found"
				});
			}

			dbContext.Products.Remove(product);
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Product deleted successfully"
			});
		}
		[HttpPut("{id:int}/status")]
		[Authorize(Roles = "admin")]
		public IActionResult UpdateProductStatus(int id, UpdateProductStatusDto statusDto)
		{
			var product = dbContext.Products.Find(id);

			if (product == null)
			{
				return NotFound(new
				{
					success = false,
					message = "Product not found"
				});
			}

			product.Status = statusDto.Status;
			dbContext.SaveChanges();

			return Ok(new
			{
				success = true,
				message = "Product status updated successfully"
			});
		}
	}
}
