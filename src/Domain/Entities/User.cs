using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int Id { get; private set; }

    public string? IdentityUserId { get; private set; }

    [Required]
    public string Name { get; private set; } = default!;

    [Required]
    public string Email { get; private set; } = default!;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public List<Contact> Contacts { get; private set; } = new();

    private User() { }

    public User(string name, string email, string? identityUserId = null, List<Contact>? contacts = null)
    {
        Name = name;
        Email = email;
        IdentityUserId = identityUserId;
        Uuid = Guid.NewGuid();
        Contacts = contacts ?? new List<Contact>();
    }

    public string UpdateName(string name)
    {
        return Name = name;
    }
    public string UpdateEmail(string email)
    {
        return Email = email;
    }

    public void LinkIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            throw new ArgumentException("Identity user id is required", nameof(identityUserId));
        }

        if (!string.IsNullOrEmpty(IdentityUserId) && IdentityUserId != identityUserId)
        {
            throw new InvalidOperationException("User is already linked to a different identity user");
        }

        IdentityUserId = identityUserId;
    }
}