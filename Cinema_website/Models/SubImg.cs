namespace Cinema_website.Models
{
    public class SubImg
    {
        public int Id { get; set; }
        public string Sub_Img { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
