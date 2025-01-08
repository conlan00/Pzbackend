public class AddBookRequest
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public int ShelterId { get; set; }
}
