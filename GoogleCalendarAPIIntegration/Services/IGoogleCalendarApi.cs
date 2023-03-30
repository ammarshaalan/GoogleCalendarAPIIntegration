using GoogleCalendarAPIIntegration.Models;
using System.Collections.Generic;

namespace GoogleCalendarAPIIntegration.Services
{
    public interface IGoogleCalendarApi
    {
        Event CreateEvent(Event calendarEvent, string reminderMinutes, string reminderMethod);
        Event GetEvent(string identifier);
        IEnumerable<Event> GetAllEvents();
        bool UpdateEvent(string identifier, Event calendarEvent, string reminderMinutes, string reminderMethod);
        bool DeleteEvent(string identifier);
    }
}
