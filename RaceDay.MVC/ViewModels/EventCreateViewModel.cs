using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.ViewModels
{
    public class EventCreateViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; }

        [Required, Range(1, 1000)]
        public decimal Distance { get; set; }

        // We will send the EventTypeId to the API
        [Required]
        public int EventTypeId { get; set; }

        // For the Azure Blob Storage requirement
        public IFormFile? BannerImage { get; set; }
    }
}