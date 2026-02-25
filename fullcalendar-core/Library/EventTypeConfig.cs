namespace fullcalendarcore.Library
{
    public static class EventTypeConfig
    {
        public static string GetEventColor(EventType eventType)
        {
            return eventType switch
            {
                EventType.Meeting => "#1565c0",      // Dark Blue - Donkerblauw
                EventType.Birthday => "#c2185b",     // Dark Pink - Donker Roze
                EventType.Anniversary => "#7b1fa2",  // Dark Purple - Donker Paars
                EventType.Reminder => "#ef6c00",     // Dark Orange - Donker Oranje
                EventType.Task => "#2e7d32",         // Dark Green - Donker Groen
                EventType.Holiday => "#c62828",      // Dark Red - Donker Rood
                EventType.Other => "#37474f",        // Dark Blue Grey - Donker Blauw Grijs
                _ => "#1565c0"
            };
        }

        public static string GetEventIcon(EventType eventType)
        {
            return eventType switch
            {
                EventType.Meeting => "📅",
                EventType.Birthday => "🎂",
                EventType.Anniversary => "💐",
                EventType.Reminder => "⏰",
                EventType.Task => "✓",
                EventType.Holiday => "🎉",
                EventType.Other => "📌",
                _ => "📅"
            };
        }

        public static string GetEventTypeName(EventType eventType)
        {
            return eventType switch
            {
                EventType.Meeting => "Afspraak",
                EventType.Birthday => "Verjaardag",
                EventType.Anniversary => "Jubileum",
                EventType.Reminder => "Herinnering",
                EventType.Task => "Taak",
                EventType.Holiday => "Vakantie",
                EventType.Other => "Overig",
                _ => "Afspraak"
            };
        }
    }
}
