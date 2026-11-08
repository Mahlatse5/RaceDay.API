using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")] 
        public Event Event { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
    }
}