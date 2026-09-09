using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_website.Models
{
    public class ApplicationUserOTP
    {
        

        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        public string OTP { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserOTP(string OTP, string userId)
        {
            Id = Guid.NewGuid().ToString();
            this.OTP = OTP;
            ApplicationUserId = userId;
            IsValid = true;
            CreatedAt = DateTime.UtcNow;
            ValidTo = CreatedAt.AddMinutes(30);

        }

        public ApplicationUserOTP()
        {
        }
    }
}
