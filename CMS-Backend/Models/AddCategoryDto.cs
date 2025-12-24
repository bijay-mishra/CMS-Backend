namespace CMS_Backend.Models
{
    public class AddCategoryDto
    {
		public required string Name { get; set; }
		public required string Email { get; set; }
		public string? Phone { get; set; }
		public decimal Salary { get; set; }

	}
}
