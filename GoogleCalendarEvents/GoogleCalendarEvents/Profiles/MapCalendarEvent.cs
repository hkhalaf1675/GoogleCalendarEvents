using Google.Apis.Calendar.v3.Data;
using GoogleCalendarEvents.DTOs;

namespace GoogleCalendarEvents.Profiles
{
    public static class MapCalendarEvent
    {
        public static EventResponseDto Map(Event newEvent)
        {
            return new EventResponseDto
            {
                Id = newEvent.Id,
                Summary = newEvent.Summary,
                Description = newEvent.Description,
                Location = newEvent.Location,
                Start = newEvent.Start.DateTime,
                End = newEvent.End.DateTime
            };
        }
    }
}
