namespace GoogleCalendarAPIIntegration.Models
{
    public class Event
    {
        public Event()
        {
            this.Start = new EventDateTime()
            {
                TimeZone = "Africa/Cairo"
            };
            this.End = new EventDateTime()
            {
                TimeZone = "Africa/Cairo"
            };
        }

        public string Id { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public EventDateTime Start { get; set; }

        public EventDateTime End { get; set; }

        public Reminders Reminders { get; set; }
    

    }

    public class EventDateTime
    {
        public string DateTime { get; set; }

        public string TimeZone { get; set; }
    }

    public class Reminders
    {
        public Reminder[] Overrides { get; set; }

        public bool UseDefault { get; set; }
    }

    public class Reminder
    {
        public int Minutes { get; set; }
        public string Method { get; set; }
    }
}