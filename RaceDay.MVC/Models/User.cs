using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } // 'Organiser' or 'Participant'

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Navigation properties (Links to other tables)
        public ICollection<Event> OrganisedEvents { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; }
    }
} 