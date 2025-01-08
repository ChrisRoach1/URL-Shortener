

namespace URL_Shortener.Data.Models
{
    public class URL
    {
        public int Id { get; set; } 

        public string ShortenedURL { get; set; }

        public string OriginalURL { get; set; } 

        public DateTime CreatedOn { get; set; }

        public string IPAddress { get; set; }   

    }
}
