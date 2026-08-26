namespace Cinema_website.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }

        public List<Movie> Movies { get; set; }
    }
}
