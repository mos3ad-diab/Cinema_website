using System.ComponentModel.DataAnnotations;

namespace Cinema_website.ViewModels
{
    public class ProfileVM
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
