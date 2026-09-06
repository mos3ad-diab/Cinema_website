using Microsoft.AspNetCore.Identity;

namespace Cinema_website.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
    }
}
