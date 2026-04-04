public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEvents(string userId);
    Task<IEnumerable<Event>> GetEventsByContactId(int contactId, string userId);
    Task<Event> GetEventById(int id, string userId);
    Task CreateEvent(EventDto eventDto, string userId);
    Task UpdateEvent(int id, EventDto eventDto, string userId);
    Task DeleteEvent(int id, string userId);
}

public class EventService : IEventService
{
    private readonly EventRepository _eventRepository;

    public EventService(EventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Event>> GetAllEvents(string userId)
        => await _eventRepository.GetEvents(userId);

    public async Task<IEnumerable<Event>> GetEventsByContactId(int contactId, string userId)
        => await _eventRepository.GetEventsByContactId(contactId, userId);

    public async Task<Event> GetEventById(int id, string userId)
        => await _eventRepository.GetEventById(id, userId);

    public async Task CreateEvent(EventDto eventDto, string userId)
        => await _eventRepository.AddEvent(eventDto, userId);

    public async Task UpdateEvent(int id, EventDto eventDto, string userId)
        => await _eventRepository.UpdateEvent(id, eventDto, userId);

    public async Task DeleteEvent(int id, string userId)
        => await _eventRepository.DeleteEvent(id, userId);
}