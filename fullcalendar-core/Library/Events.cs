using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace fullcalendarcore.Library
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool AllDay { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        [JsonConverter(typeof(JsonNumberEnumConverter<EventType>))]
        public EventType EventType { get; set; } = EventType.Meeting;

        // Recurrence properties
        public bool IsRecurring { get; set; }

        [JsonConverter(typeof(JsonNumberEnumConverter<RecurrencePattern>))]
        public RecurrencePattern RecurrencePattern { get; set; }
        public int RecurrenceInterval { get; set; } = 1;
        public string RecurrenceEndDate { get; set; }
        public int? ParentEventId { get; set; }
    }
}
