namespace fullcalendarcore.Library
{
    public class EventShare
    {
        public int ShareId { get; set; }
        public int EventId { get; set; }
        public string OwnerUserId { get; set; }
        public string SharedWithUserId { get; set; }
        public string SharedWithUserName { get; set; }
        public bool CanEdit { get; set; }
        public string SharedDate { get; set; }
    }
}
