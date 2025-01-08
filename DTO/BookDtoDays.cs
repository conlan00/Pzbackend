namespace Backend.DTO
{
    public class BookDtoDays
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Cover { get; set; } = null!;

        public string CategoryName { get; set; } = null!;
        public int Days { get; set; }
    }
}
