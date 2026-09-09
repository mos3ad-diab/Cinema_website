using System.ComponentModel.DataAnnotations;

namespace Cinema_website.ViewModels
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
