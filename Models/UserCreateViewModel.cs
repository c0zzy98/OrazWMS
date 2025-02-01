namespace OrazWMS.Models
{
    public class UserCreateViewModel
    {
        public string UserName { get; set; } = string.Empty; 
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; }
    }
}
