using System.ComponentModel.DataAnnotations;

namespace Cinema_website.ViewModels
{
    public class ChangePasswordVM
    {
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
