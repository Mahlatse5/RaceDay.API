using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }

        public int ParticipantId { get; set; }
        public User Participant { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public string? FinishTime { get; set; }
        public int? Position { get; set; }
    }
}