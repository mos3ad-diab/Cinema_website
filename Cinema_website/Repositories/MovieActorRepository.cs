using Cinema_website.Models;

namespace Cinema_website.Repositories
{
    public class MovieActorRepository : Repository<MovieActor> , IMovieActorRepository
    {
        public void DeleteRange(IEnumerable<MovieActor> movieActors)
        {
            _context.MovieActors.RemoveRange(movieActors);
        }
    }
}
