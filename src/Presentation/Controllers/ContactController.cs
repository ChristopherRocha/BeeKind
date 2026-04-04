using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[Authorize]
public class ContactController : BaseController
{
	private readonly IContactService _contactService;

	public ContactController(IContactService contactService)
	{
		_contactService = contactService;
	}

	[HttpGet]
	public async Task<IActionResult> GetAllContacts()
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		var contacts = await _contactService.GetAllContacts(userId);
		return Ok(contacts);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetContactById(int id)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		var contact = await _contactService.GetContactById(id, userId);
		return Ok(contact);
	}

	[HttpPost]
	public async Task<IActionResult> CreateContact([FromBody] ContactDto contactDto)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _contactService.CreateContact(contactDto, userId);
		return Ok();
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactDto contactDto)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _contactService.UpdateContact(id, contactDto, userId);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteContact(int id)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _contactService.DeleteContact(id, userId);
		return Ok();
	}
}