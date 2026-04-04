public interface IContactService
{
    Task<IEnumerable<Contact>> GetAllContacts(string userId);
    Task<Contact> GetContactById(int id, string userId);
    Task CreateContact(ContactDto contactDto, string userId);
    Task UpdateContact(int id, ContactDto contactDto, string userId);
    Task DeleteContact(int id, string userId);
}

public class ContactService : IContactService
{
    private readonly ContactRepository _contactRepository;

    public ContactService(ContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<IEnumerable<Contact>> GetAllContacts(string userId)
        => await _contactRepository.GetContacts(userId);

    public async Task<Contact> GetContactById(int id, string userId)
        => await _contactRepository.GetContactById(id, userId);

    public async Task CreateContact(ContactDto contactDto, string userId)
        => await _contactRepository.AddContact(contactDto, userId);

    public async Task UpdateContact(int id, ContactDto contactDto, string userId)
        => await _contactRepository.UpdateContact(id, contactDto, userId);

    public async Task DeleteContact(int id, string userId)
        => await _contactRepository.DeleteContact(id, userId);
}