namespace CMS_Backend.Models.Auth
{
	public class ChangePasswordDto
	{
		public required string OldPassword { get; set; }
		public required string NewPassword { get; set; }
	}

}
