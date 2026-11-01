using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public decimal Distance { get; set; }

        // FOREIGN KEYS (Keep [Required] here)
        [Required]
        public int EventTypeId { get; set; }

        [Required]
        public int OrganiserId { get; set; }

        // NAVIGATION PROPERTIES 
        public EventType EventType { get; set; }
        public User Organiser { get; set; }
        public List<Category> Categories { get; set; }
        public List<Enrolment> Enrolments { get; set; }
    }
}