using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EventRepository
{
	private readonly AppDbContext _context;

	public EventRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Event>> GetEvents(string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		return await _context.Events
			.Include(e => e.Contact)
			.Where(e => e.Contact.UserId == appUserId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Event>> GetEventsByContactId(int contactId, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == contactId && c.UserId == appUserId);
		if (contact == null)
		{
			throw new KeyNotFoundException("Contact not found");
		}

		return await _context.Events
			.Include(e => e.Contact)
			.Where(e => e.ContactId == contactId)
			.ToListAsync();
	}

	public async Task<Event> GetEventById(int id, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var ev = await _context.Events
			.Include(e => e.Contact)
			.FirstOrDefaultAsync(e => e.Id == id && e.Contact.UserId == appUserId);
		if (ev == null)
		{
			throw new KeyNotFoundException("Event not found");
		}
		return ev;
	}

	public async Task AddEvent(EventDto eventDto, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == eventDto.ContactId && c.UserId == appUserId);
		if (contact == null)
		{
			throw new KeyNotFoundException("Contact not found");
		}

		var ev = new Event(eventDto.Title, eventDto.Date, eventDto.Location, eventDto.ContactId, eventDto.Message, eventDto.Description);
		contact.Events.Add(ev);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateEvent(int id, EventDto eventDto, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);

		var ev = await _context.Events
			.Include(e => e.Contact)
			.FirstOrDefaultAsync(e => e.Id == id && e.Contact.UserId == appUserId);
		if (ev == null)
			throw new KeyNotFoundException("Event not found");

		var targetContact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == eventDto.ContactId && c.UserId == appUserId);
		if (targetContact == null)
		{
			throw new KeyNotFoundException("Contact not found");
		}

		ev.Contact = targetContact;
		ev.Title = eventDto.Title;
		ev.Date = eventDto.Date;
		ev.Location = eventDto.Location;
		ev.Message = eventDto.Message;
		ev.Description = eventDto.Description;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteEvent(int id, string userId)
	{
		var appUserId = await ResolveAppUserIdAsync(userId);
		var ev = await _context.Events
			.Include(e => e.Contact)
			.FirstOrDefaultAsync(e => e.Id == id && e.Contact.UserId == appUserId);
		if (ev == null)
		{
			throw new KeyNotFoundException("Event not found");
		}
		_context.Events.Remove(ev);
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