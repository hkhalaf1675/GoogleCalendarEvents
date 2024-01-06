using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarEvents.DTOs;
using GoogleCalendarEvents.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;

namespace GoogleCalendarEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarEventsController : ControllerBase
    {
        private readonly IGoogleServices googleServices;
        private readonly IConfiguration configuration;

        public CalendarEventsController(IGoogleServices googleServices)
        {
            this.googleServices = googleServices;
        }

        [HttpGet("get-token")]
        public async Task<IActionResult> GetToken()
        {
            TokenResponse? token = await googleServices.GetTokenAsync();

            if (token == null)
            {
                return BadRequest("There is an error on configring the access token");
            }

            return Ok(token);
        }

        [HttpPost("insert-event")]
        public async Task<IActionResult> InsertEvent(EventRequestDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseMessageDto response = await googleServices.AddCalendarEventAsync(eventDto);

            if (response.IsJobDone)
            {
                return Ok(response.Message);
            }

            return BadRequest(response.Message);
        }

        [HttpGet("get-all-events")]
        public async Task<IActionResult> GetAllEvents()
        {
            List<EventResponseDto> events = await googleServices.GetAllCalendarEventsAsync();

            return Ok(events);
        }

        [HttpGet("get-event-by-id")]
        public async Task<IActionResult> GetCalendarEventById(string eventId)
        {
            EventResponseDto? targetEvent = await googleServices.GetCalendarEventByIdAsync(eventId);

            if (targetEvent == null)
                return NotFound("There is no events with that id");

            return Ok(targetEvent);
        }

        [HttpDelete("delete-event")]
        public async Task<IActionResult> DeleteCalendarEvent(string eventId)
        {
            ResponseMessageDto response = await googleServices.DeleteCalendarEventAsync(eventId);

            if (response.IsJobDone)
                return Ok(response.Message);

            return BadRequest(response.Message);
        }

        [HttpGet("search-event")]
        public async Task<IActionResult> SearchCalendarEvents([FromQuery]QueryParamsDto queryParams)
        {
            var response = await googleServices.SearchCalendarEventsAsync(queryParams);
            return Ok(response);
        }
    }
}
