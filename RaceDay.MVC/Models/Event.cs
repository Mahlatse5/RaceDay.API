using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(100)]
        public string Location { get; set; }
        
        [Required, Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        // Foreign Keys
        [Required]
        public int EventTypeId { get; set; }
        [ForeignKey("EventTypeId")]
        public EventType EventType { get; set; }

        [Required]
        public int OrganiserId { get; set; }
        [ForeignKey("OrganiserId")]
        public User Organiser { get; set; }

        [MaxLength(255)]
        public string? BannerImageUrl { get; set; }

        // Navigation properties
        public ICollection<Category> Categories { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
}