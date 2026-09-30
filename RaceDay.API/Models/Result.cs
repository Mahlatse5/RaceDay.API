using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        [ForeignKey("EnrolmentId")]
        public Enrolment Enrolment { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; } 

        [Required]
        public int Position { get; set; }
    }
}