using Cinema_website.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema_website.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<SubImg> SubImgs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             base.OnConfiguring(optionsBuilder);
             optionsBuilder.UseSqlServer("Data Source=DESKTOP-IRRUHA5;Initial Catalog = CinemaDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
         }*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieActor>()
                 .HasKey(x => new { x.MovieId, x.ActorId });
        }
    }
}
