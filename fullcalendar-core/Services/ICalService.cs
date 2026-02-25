using fullcalendarcore.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace fullcalendarcore.Services
{
    public class ICalService
    {
        /// <summary>
        /// Generates iCal (RFC 5545) formatted string from events
        /// </summary>
        public static string GenerateICalendar(List<Event> events, string calendarName = "Mijn Agenda")
        {
            var sb = new StringBuilder();

            // Calendar header
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//FullCalendar .NET//Calendar//NL");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine($"X-WR-CALNAME:{EscapeText(calendarName)}");
            sb.AppendLine("X-WR-TIMEZONE:Europe/Amsterdam");

            // Add timezone definition for Europe/Amsterdam
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine("TZID:Europe/Amsterdam");
            sb.AppendLine("BEGIN:DAYLIGHT");
            sb.AppendLine("TZOFFSETFROM:+0100");
            sb.AppendLine("TZOFFSETTO:+0200");
            sb.AppendLine("TZNAME:CEST");
            sb.AppendLine("DTSTART:19700329T020000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=-1SU");
            sb.AppendLine("END:DAYLIGHT");
            sb.AppendLine("BEGIN:STANDARD");
            sb.AppendLine("TZOFFSETFROM:+0200");
            sb.AppendLine("TZOFFSETTO:+0100");
            sb.AppendLine("TZNAME:CET");
            sb.AppendLine("DTSTART:19701025T030000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=10;BYDAY=-1SU");
            sb.AppendLine("END:STANDARD");
            sb.AppendLine("END:VTIMEZONE");

            // Add events
            foreach (var evt in events)
            {
                sb.AppendLine(GenerateVEvent(evt));
            }

            // Calendar footer
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        private static string GenerateVEvent(Event evt)
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{evt.EventId}@fullcalendar-aspnet-core");
            sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
            
            // Parse and format dates
            DateTime startDate = DateTime.Parse(evt.Start);
            
            if (evt.AllDay)
            {
                // All-day events use DATE format (YYYYMMDD)
                sb.AppendLine($"DTSTART;VALUE=DATE:{startDate:yyyyMMdd}");
                
                if (!string.IsNullOrEmpty(evt.End))
                {
                    DateTime endDate = DateTime.Parse(evt.End);
                    // iCal all-day events are exclusive, so add 1 day
                    sb.AppendLine($"DTEND;VALUE=DATE:{endDate.AddDays(1):yyyyMMdd}");
                }
            }
            else
            {
                // Timed events use DATETIME format with timezone
                sb.AppendLine($"DTSTART;TZID=Europe/Amsterdam:{FormatDateTimeLocal(startDate)}");
                
                if (!string.IsNullOrEmpty(evt.End))
                {
                    DateTime endDate = DateTime.Parse(evt.End);
                    sb.AppendLine($"DTEND;TZID=Europe/Amsterdam:{FormatDateTimeLocal(endDate)}");
                }
            }

            sb.AppendLine($"SUMMARY:{EscapeText(evt.Title)}");
            
            if (!string.IsNullOrEmpty(evt.Description))
            {
                sb.AppendLine($"DESCRIPTION:{EscapeText(evt.Description)}");
            }

            // Add event type as category
            if (evt.EventType != EventType.Meeting)
            {
                sb.AppendLine($"CATEGORIES:{EventTypeConfig.GetEventTypeName(evt.EventType)}");
            }

            // Add organizer if available
            if (!string.IsNullOrEmpty(evt.UserName))
            {
                sb.AppendLine($"ORGANIZER;CN={EscapeText(evt.UserName)}:mailto:noreply@fullcalendar.local");
            }

            // Add recurrence rule if recurring
            if (evt.IsRecurring && evt.RecurrencePattern != RecurrencePattern.None)
            {
                sb.AppendLine(GenerateRRule(evt));
            }

            sb.AppendLine("END:VEVENT");

            return sb.ToString();
        }

        private static string GenerateRRule(Event evt)
        {
            var freq = evt.RecurrencePattern switch
            {
                RecurrencePattern.Daily => "DAILY",
                RecurrencePattern.Weekly => "WEEKLY",
                RecurrencePattern.Monthly => "MONTHLY",
                RecurrencePattern.Yearly => "YEARLY",
                _ => "DAILY"
            };

            var rrule = $"RRULE:FREQ={freq};INTERVAL={evt.RecurrenceInterval}";

            if (!string.IsNullOrEmpty(evt.RecurrenceEndDate))
            {
                DateTime endDate = DateTime.Parse(evt.RecurrenceEndDate);
                rrule += $";UNTIL={FormatDateTime(endDate)}";
            }

            return rrule;
        }

        private static string FormatDateTime(DateTime dt)
        {
            // UTC format: YYYYMMDDTHHmmssZ
            return dt.ToUniversalTime().ToString("yyyyMMddTHHmmss") + "Z";
        }

        private static string FormatDateTimeLocal(DateTime dt)
        {
            // Local format without Z: YYYYMMDDTHHmmss
            return dt.ToString("yyyyMMddTHHmmss");
        }

        private static string EscapeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Escape special characters according to RFC 5545
            return text
                .Replace("\\", "\\\\")  // Backslash
                .Replace(",", "\\,")    // Comma
                .Replace(";", "\\;")    // Semicolon
                .Replace("\n", "\\n")   // Newline
                .Replace("\r", "");     // Remove carriage return
        }
    }
}
