using fullcalendarcore.Library;
using System;
using System.Collections.Generic;

namespace fullcalendarcore.Services
{
    public class RecurrenceService
    {
        /// <summary>
        /// Generates recurring event instances based on recurrence pattern
        /// </summary>
        public static List<Event> GenerateRecurringEvents(Event masterEvent, DateTime rangeStart, DateTime rangeEnd)
        {
            var events = new List<Event>();

            if (!masterEvent.IsRecurring || masterEvent.RecurrencePattern == RecurrencePattern.None)
            {
                return events;
            }

            DateTime eventStart = DateTime.Parse(masterEvent.Start);
            DateTime? recurrenceEnd = string.IsNullOrEmpty(masterEvent.RecurrenceEndDate) 
                ? (DateTime?)null 
                : DateTime.Parse(masterEvent.RecurrenceEndDate);

            // Calculate duration
            TimeSpan? duration = null;
            if (!string.IsNullOrEmpty(masterEvent.End))
            {
                DateTime eventEnd = DateTime.Parse(masterEvent.End);
                duration = eventEnd - eventStart;
            }

            DateTime currentDate = eventStart;
            int instanceCount = 0;
            int maxInstances = 1000; // Safety limit

            while (currentDate <= rangeEnd && instanceCount < maxInstances)
            {
                // Check if this instance is within the recurrence end date
                if (recurrenceEnd.HasValue && currentDate > recurrenceEnd.Value)
                {
                    break;
                }

                // Only add if it's within the requested range
                if (currentDate >= rangeStart && currentDate <= rangeEnd)
                {
                    var instance = new Event
                    {
                        EventId = masterEvent.EventId, // Will be virtual/client-side only
                        Title = masterEvent.Title,
                        Description = masterEvent.Description,
                        Start = currentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        End = duration.HasValue 
                            ? currentDate.Add(duration.Value).ToString("yyyy-MM-dd HH:mm:ss") 
                            : null,
                        AllDay = masterEvent.AllDay,
                        UserId = masterEvent.UserId,
                        UserName = masterEvent.UserName,
                        EventType = masterEvent.EventType,
                        IsRecurring = true,
                        RecurrencePattern = masterEvent.RecurrencePattern,
                        ParentEventId = masterEvent.EventId
                    };

                    events.Add(instance);
                }

                // Move to next occurrence
                currentDate = GetNextOccurrence(currentDate, masterEvent.RecurrencePattern, masterEvent.RecurrenceInterval);
                instanceCount++;
            }

            return events;
        }

        private static DateTime GetNextOccurrence(DateTime current, RecurrencePattern pattern, int interval)
        {
            return pattern switch
            {
                RecurrencePattern.Daily => current.AddDays(interval),
                RecurrencePattern.Weekly => current.AddDays(7 * interval),
                RecurrencePattern.Monthly => current.AddMonths(interval),
                RecurrencePattern.Yearly => current.AddYears(interval),
                _ => current
            };
        }

        /// <summary>
        /// Validates recurrence settings
        /// </summary>
        public static string ValidateRecurrence(Event evt)
        {
            if (!evt.IsRecurring)
            {
                return string.Empty;
            }

            if (evt.RecurrencePattern == RecurrencePattern.None)
            {
                return "Selecteer een herhalingspatroon.";
            }

            if (evt.RecurrenceInterval < 1)
            {
                return "Herhalingsinterval moet minimaal 1 zijn.";
            }

            if (!string.IsNullOrEmpty(evt.RecurrenceEndDate))
            {
                if (!DateTime.TryParse(evt.RecurrenceEndDate, out DateTime endDate))
                {
                    return "Ongeldige einddatum voor herhaling.";
                }

                if (!DateTime.TryParse(evt.Start, out DateTime startDate))
                {
                    return "Ongeldige startdatum.";
                }

                if (endDate <= startDate)
                {
                    return "Einddatum van herhaling moet na de startdatum zijn.";
                }
            }

            return string.Empty;
        }
    }
}
