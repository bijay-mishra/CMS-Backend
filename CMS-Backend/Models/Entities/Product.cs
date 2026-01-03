namespace CMS_Backend.Models.Entities
{
    public class Product
    {
		public int Id { get; set; }
		public required string Name { get; set; }
		public int? CategoryId { get; set; }

		public string? Description { get; set; }
		public decimal Price { get; set; }
		public string? Status { get; set; }

	}

}
