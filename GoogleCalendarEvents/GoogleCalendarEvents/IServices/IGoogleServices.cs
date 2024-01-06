using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using GoogleCalendarEvents.DTOs;

namespace GoogleCalendarEvents.IServices
{
    public interface IGoogleServices
    {
        Task<TokenResponse?> GetTokenAsync();
        Task<CalendarService> GetCalendarServiceAsync();
        Task<ResponseMessageDto?> AddCalendarEventAsync(EventRequestDto eventDto);
        Task<List<EventResponseDto>> GetAllCalendarEventsAsync();
        Task<EventResponseDto?> GetCalendarEventByIdAsync(string id);
        Task<ResponseMessageDto> DeleteCalendarEventAsync(string id);
        Task<List<EventResponseDto>> SearchCalendarEventsAsync(QueryParamsDto queryParams);
    }
}
