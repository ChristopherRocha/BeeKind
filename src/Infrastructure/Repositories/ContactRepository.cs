using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ContactRepository
{
	private readonly AppDbContext _context;

	public ContactRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Contact>> GetContacts(string identityUserId)
	{
		var appUserId = await ResolveAppUserIdAsync(identityUserId);

		return await _context.Contacts
			.Include(c => c.Events)
			.Where(c => c.UserId == appUserId)
			.ToListAsync();
	}

	public async Task<Contact> GetContactById(int id, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = await _context.Contacts
			.Include(c => c.Events)
			.FirstOrDefaultAsync(c => c.Id == id && c.UserId == appUserId);
		if (contact == null)
		{
			throw new KeyNotFoundException("Contact not found");
		}
		return contact;
	}

	public async Task AddContact(ContactDto contactDto, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = new Contact(contactDto.Name, contactDto.Email, contactDto.PhoneNumber, appUserId, null);
		_context.Contacts.Add(contact);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateContact(int id, ContactDto contactDto, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == appUserId);
		if (contact == null)
			throw new KeyNotFoundException("Contact not found");
            
		contact.UpdateName(contactDto.Name);
		contact.UpdateEmail(contactDto.Email);
		contact.UpdatePhoneNumber(contactDto.PhoneNumber);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteContact(int id, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == appUserId);
		if (contact == null)
		{
			throw new KeyNotFoundException("Contact not found");
		}
		_context.Contacts.Remove(contact);
		await _context.SaveChangesAsync();
	}

	private async Task<int> ResolveAppUserIdAsync(string identityUserId)
	{
		var appUser = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
		if (appUser == null)
		{
			throw new UnauthorizedAccessException("User not linked to application profile");
		}

		return appUser.Id;
	}
}
