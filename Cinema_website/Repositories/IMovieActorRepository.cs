using Cinema_website.Models;

namespace Cinema_website.Repositories
{
    public interface IMovieActorRepository : IRepository<MovieActor>
    {
        public void DeleteRange(IEnumerable<MovieActor> movieActors);
    }
}
