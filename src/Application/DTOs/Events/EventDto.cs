public class EventDto
{
    public int ContactId { get; set; }
    public string Title { get; set; } = default!;
    public DateTime Date { get; set; }
    public string Location { get; set; } = default!;
    public string? Message { get; set; } = default!;
    public string? Description { get; set; } = default!;
}
