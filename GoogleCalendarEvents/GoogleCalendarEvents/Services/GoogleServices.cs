using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using GoogleCalendarEvents.DTOs;
using GoogleCalendarEvents.IServices;
using GoogleCalendarEvents.Profiles;
using System.Text;

namespace GoogleCalendarEvents.Services
{
    public class GoogleServices : IGoogleServices
    {
        private readonly IConfiguration configuration;

        public GoogleServices(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<CalendarService> GetCalendarServiceAsync()
        {
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync
                (
                    new ClientSecrets
                    {
                        ClientId = configuration.GetValue<string>("GoogleCredentials:ClientId"),
                        ClientSecret = configuration.GetValue<string>("GoogleCredentials:ClientSecret")
                    },
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None
                );

            CalendarService service = new CalendarService(new BaseClientService.Initializer
            {
                ApplicationName = configuration.GetValue<string>("GoogleCredentials:ApplicationName"),
                HttpClientInitializer = credential
            });

            return service;

        }

        public async Task<TokenResponse?> GetTokenAsync()
        {
            //var scopes = new[]
            //{
            //    "https://www.googleapis.com/auth/calendar",
            //    "https://www.googleapis.com/auth/calendar.events"
            //};

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync
                (
                    new ClientSecrets
                    {
                        ClientId = configuration.GetValue<string>("GoogleCredentials:ClientId"),
                        ClientSecret = configuration.GetValue<string>("GoogleCredentials:ClientSecret")
                    },
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None
                );

            return credential?.Token;
            
        }

        public async Task<ResponseMessageDto?> AddCalendarEventAsync(EventRequestDto eventDto)
        {
            Event newEvent = new Event()
            {
                Summary = eventDto?.Summary,
                Description = eventDto?.Description,
                Location = eventDto?.Location,
                Start = new EventDateTime
                {
                    DateTime = eventDto?.Start,
                    TimeZone = "Africa/Cairo"
                },
                End = new EventDateTime
                {
                    DateTime = eventDto?.End,
                    TimeZone = "Africa/Cairo"
                }
            };

            string calendarId = "primary";

            CalendarService service = await GetCalendarServiceAsync();

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);

            try
            {
                Event createdEvent = await request.ExecuteAsync();

                return new ResponseMessageDto
                {
                    IsJobDone = true,
                    Message = createdEvent.HtmlLink
                };
            }
            catch(Exception ex )
            {
                return new ResponseMessageDto
                {
                    IsJobDone = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<List<EventResponseDto>> GetAllCalendarEventsAsync()
        {
            List<EventResponseDto> events = new List<EventResponseDto>();

            CalendarService service = await GetCalendarServiceAsync();

            try
            {
                var response = service.Events.List("primary").Execute();

                foreach (var item in response.Items)
                {
                    events.Add(MapCalendarEvent.Map(item));
                }

                return events;
            }
            catch(Exception ex)
            {
                return new List<EventResponseDto>();
            }
        }

        public async Task<EventResponseDto?> GetCalendarEventByIdAsync(string id)
        {
            EventResponseDto eventDto;

            CalendarService service = await GetCalendarServiceAsync();

            try
            {
                Event targetEvent = service.Events.Get("primary", id).Execute();

                eventDto = MapCalendarEvent.Map(targetEvent);

                return eventDto;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseMessageDto> DeleteCalendarEventAsync(string id)
        {
            CalendarService service = await GetCalendarServiceAsync();

            try
            {
                string? response = await service.Events.Delete("primary", id).ExecuteAsync();

                return new ResponseMessageDto
                {
                    IsJobDone = response != null,
                    Message = response
                };
            }
            catch( Exception ex)
            {
                return new ResponseMessageDto
                {
                    IsJobDone = false,
                    Message = "There is an error at deleting the event"
                };
            }
        }

        public async Task<List<EventResponseDto>> SearchCalendarEventsAsync(QueryParamsDto queryParams)
        {
            List<EventResponseDto> events = new List<EventResponseDto>();

            CalendarService service = await GetCalendarServiceAsync();

            var request = service.Events.List("primary");

            StringBuilder requestQuery = new StringBuilder();

            if(queryParams?.Summary != null)
            {
                requestQuery.Append($"summary:{queryParams.Summary}");
            }

            if(queryParams?.Description != null)
            {
                if(requestQuery.Length > 0)
                {
                    requestQuery.Append($"AND description:{queryParams.Description}");
                }
                else
                {
                    requestQuery.Append($"description:{queryParams.Description}");
                }
            }

            if(queryParams?.Location  != null)
            {
                if (requestQuery.Length > 0)
                {
                    requestQuery.Append($"AND location:{queryParams.Location}");
                }
                else
                {
                    requestQuery.Append($"location:{queryParams.Location}");
                }
            }
            if (requestQuery.Length > 0)
                request.Q = requestQuery.ToString();

            var responseEvents = await request.ExecuteAsync();

            foreach(var item in responseEvents.Items)
            {
                events.Add(MapCalendarEvent.Map(item));
            }

            return events;
        }
    }
}
