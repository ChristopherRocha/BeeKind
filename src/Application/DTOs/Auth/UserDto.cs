public class UserDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<ContactDto>? Contacts { get; set; }
}
