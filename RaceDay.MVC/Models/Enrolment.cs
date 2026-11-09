using RaceDay.API.Models;

namespace RaceDay.MVC.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string? Status { get; set; }
        public string? FinishTime { get; set; }
        public int? Position { get; set; }

        // Navigation properties (as simple objects, not EF Core navigation)
        public User? Participant { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
    }
}