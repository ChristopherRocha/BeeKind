using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Event
{
    [Key]
    public int Id { get; set; }
    public int ContactId { get; set; }
    [JsonIgnore]
    public Contact Contact { get; set; } = default!;
    public string Title { get; set; } = default!;
    public DateTime Date { get; set; } 
    public string? Location { get; set; } = default!;
    public string? Message { get; set; } = default!;
    public string? Description { get; set; } = default!;

    private Event() { }

    public Event(string title, DateTime date, string location, int contactId, string? message = null, string? description = null)
    {
        Title = title;
        Date = date;
        Location = location;
        ContactId = contactId;
        Message = message;
        Description = description;
    }
    
}