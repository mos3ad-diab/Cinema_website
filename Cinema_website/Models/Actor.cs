namespace Cinema_website.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }

        public List<MovieActor> MovieActors { get; set; }

    }
}
