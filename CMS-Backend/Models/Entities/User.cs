namespace CMS_Backend.Models.Entities
{
	public class User
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string ContactNumber { get; set; }

		public required string Email { get; set; }
		public required string PasswordHash { get; set; }
		public  string? Status { get; set; }

		public  string? Role { get; set; }


	}
}
