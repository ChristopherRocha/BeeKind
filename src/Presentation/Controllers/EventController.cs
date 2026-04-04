using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[Authorize]
public class EventController : BaseController
{
	private readonly IEventService _eventService;

	public EventController(IEventService eventService)
	{
		_eventService = eventService;
	}

	[HttpGet]
	public async Task<IActionResult> GetAllEvents()
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		var events = await _eventService.GetAllEvents(userId);
		return Ok(events);
	}

	[HttpGet("contact/{contactId}")]
	public async Task<IActionResult> GetEventsByContactId(int contactId)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		var events = await _eventService.GetEventsByContactId(contactId, userId);
		return Ok(events);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetEventById(int id)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		var eventItem = await _eventService.GetEventById(id, userId);
		return Ok(eventItem);
	}

	[HttpPost]
	public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _eventService.CreateEvent(eventDto, userId);
		return Ok();
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto eventDto)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _eventService.UpdateEvent(id, eventDto, userId);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteEvent(int id)
	{
		var userId = GetUserId();
		if (string.IsNullOrWhiteSpace(userId))
			return Unauthorized("User not authenticated");

		await _eventService.DeleteEvent(id, userId);
		return Ok();
	}
}