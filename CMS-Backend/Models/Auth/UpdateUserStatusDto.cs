namespace CMS_Backend.Models.Auth
{
	public class UpdateUserStatusDto
	{
		public int UserId { get; set; }
		public string? Status { get; set; }
	}

}
