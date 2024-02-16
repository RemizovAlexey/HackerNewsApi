namespace HackerNews.Data.Models
{
    public class StoryDto
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string By { get; set; }
        public int Time { get; set; }
        public int Score { get; set; }
        public int Descendants { get; set; }
    }
}
