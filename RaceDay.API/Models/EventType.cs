using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required, MaxLength(50)]
        public string TypeName { get; set; } // 'Run', 'Walk', 'Cycle'

        public ICollection<Event> Events { get; set; }
    }
}