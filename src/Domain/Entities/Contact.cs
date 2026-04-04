using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Contact
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; private set; }
    [JsonIgnore]
    public User User { get; private set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public List<Event> Events { get; set; } = new List<Event>();
    public Contact(string name, string email, string phoneNumber, int userId, List<Event>? events)
    {
        UserId = userId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Events = events ?? new List<Event>();
    }
    private Contact() { }
    
    public string UpdateName(string name)
    {
        return Name = name;
    }

    public string UpdateEmail(string email)
    {
        return Email = email;
    }

    public string UpdatePhoneNumber(string phoneNumber)
    {
        return PhoneNumber = phoneNumber;
    }

}