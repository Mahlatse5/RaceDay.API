namespace RaceDay.API.Models
{
    public class EventUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public decimal Distance { get; set; }
        public int EventTypeId { get; set; }
    }
}