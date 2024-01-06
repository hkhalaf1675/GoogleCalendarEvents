using System.ComponentModel.DataAnnotations;

namespace GoogleCalendarEvents.DTOs
{
    public class EventRequestDto
    {
        [Required]
        [MinLength(3)]
        public string? Summary { get; set; }
        [Required]
        [MinLength(3)]
        public string? Location { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
    }
}
