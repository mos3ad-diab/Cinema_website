using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_website.Models
{
    [PrimaryKey(nameof(ApplicationUserId), nameof(MovieId))]
    public class Cart
    {
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
