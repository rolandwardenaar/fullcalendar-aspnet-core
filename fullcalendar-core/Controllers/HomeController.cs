using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using fullcalendarcore.Models;
using Microsoft.Extensions.Options;
using fullcalendarcore.DataAccessLayer;
using fullcalendarcore.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using fullcalendarcore.Services;

namespace fullcalendarcore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DA _DA { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IOptions<AppSettings> settings, UserManager<ApplicationUser> userManager)
        {
            _DA = new DA(settings.Value.ConnectionStr);
            _userManager = userManager;
        }

        public IActionResult Index() 
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCalendarEvents(string start, string end) {
            // Convert JavaScript date strings to ISO format for SQLite
            DateTime startDate = DateTime.Parse(start);
            DateTime endDate = DateTime.Parse(end);

            string startISO = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            string endISO = endDate.ToString("yyyy-MM-dd HH:mm:ss");

            // Get ALL events (all users can see each other's events)
            List<Event> events = _DA.GetCalendarEvents(startISO, endISO);

            // Get current user ID to mark which events belong to them
            var currentUserId = _userManager.GetUserId(User);

            // Get events shared with current user
            var sharedEventIds = _DA.GetSharedEventIds(currentUserId);

            // Process recurring events
            var allEvents = new List<Event>();
            foreach (var evt in events)
            {
                if (evt.IsRecurring)
                {
                    // Generate recurring instances
                    var instances = RecurrenceService.GenerateRecurringEvents(evt, startDate, endDate);
                    allEvents.AddRange(instances);
                }
                else
                {
                    allEvents.Add(evt);
                }
            }

            // Map to FullCalendar format with 'id' instead of 'EventId'
            var calendarEvents = allEvents.Select(e => new {
                id = e.EventId,
                title = e.Title,
                start = e.Start,
                end = e.End,
                allDay = e.AllDay,
                editable = e.UserId == currentUserId, // Only own events are editable
                backgroundColor = EventTypeConfig.GetEventColor(e.EventType),
                borderColor = EventTypeConfig.GetEventColor(e.EventType),
                extendedProps = new {
                    description = e.Description,
                    eventId = e.EventId,
                    userName = e.UserName ?? "Onbekend",
                    isOwn = e.UserId == currentUserId,
                    isShared = sharedEventIds.Contains(e.EventId),
                    eventType = (int)e.EventType,
                    eventTypeName = EventTypeConfig.GetEventTypeName(e.EventType),
                    eventIcon = EventTypeConfig.GetEventIcon(e.EventType),
                    isRecurring = e.IsRecurring,
                    recurrencePattern = (int)e.RecurrencePattern,
                    parentEventId = e.ParentEventId
                }
            });

            return Json(calendarEvents);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEvent([FromBody] Event evt) 
        {
            if (evt == null || evt.EventId <= 0)
            {
                return BadRequest("Invalid event data.");
            }

            string message = String.Empty;

            // Get current user info
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            evt.UserId = user.Id;
            evt.UserName = user.DisplayName ?? user.UserName;

            // Convert to ISO format for database storage
            evt.Start = ParseAndFormatDate(evt.Start);
            evt.End = string.IsNullOrEmpty(evt.End) ? null : ParseAndFormatDate(evt.End);

            // Validate recurrence if enabled
            if (evt.IsRecurring)
            {
                if (!string.IsNullOrEmpty(evt.RecurrenceEndDate))
                {
                    evt.RecurrenceEndDate = ParseAndFormatDate(evt.RecurrenceEndDate);
                }

                string validationMessage = RecurrenceService.ValidateRecurrence(evt);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return BadRequest(new { message = validationMessage });
                }
            }

            // Pass current user ID for ownership check
            message = _DA.UpdateEvent(evt, user.Id);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Event succesvol bijgewerkt." });
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Event evt) 
        {
            string message = String.Empty;
            int eventId = 0;

            // Get current user info
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            evt.UserId = user.Id;
            evt.UserName = user.DisplayName ?? user.UserName;

            // Convert to ISO format for database storage
            evt.Start = ParseAndFormatDate(evt.Start);
            evt.End = string.IsNullOrEmpty(evt.End) ? null : ParseAndFormatDate(evt.End);

            // Validate recurrence if enabled
            if (evt.IsRecurring)
            {
                if (!string.IsNullOrEmpty(evt.RecurrenceEndDate))
                {
                    evt.RecurrenceEndDate = ParseAndFormatDate(evt.RecurrenceEndDate);
                }

                string validationMessage = RecurrenceService.ValidateRecurrence(evt);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return BadRequest(new { message = validationMessage });
                }
            }

            message = _DA.AddEvent(evt, out eventId);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { 
                message = "Event succesvol toegevoegd.",
                eventId = eventId,
                userId = evt.UserId,
                userName = evt.UserName,
                eventType = (int)evt.EventType,
                isRecurring = evt.IsRecurring
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent([FromBody] Event evt) {
            string message = String.Empty;

            // Get current user info
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Pass current user ID for ownership check
            message = _DA.DeleteEvent(evt.EventId, user.Id);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Event succesvol verwijderd." });
        }

        private string ParseAndFormatDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr)) return null;

            // Remove comma if present
            dateStr = dateStr.Replace(",", "").Trim();

            // Try ISO format first (yyyy-MM-dd HH:mm:ss)
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime isoDateTime))
            {
                return isoDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try ISO format with T separator (yyyy-MM-ddTHH:mm:ss)
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime isoTDateTime))
            {
                return isoTDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try Dutch format from flatpickr (dd-MM-yyyy HH:mm)
            if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime nlDateTime))
            {
                return nlDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try Dutch format with seconds (dd-MM-yyyy HH:mm:ss)
            if (DateTime.TryParseExact(dateStr, "dd-MM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime nlDateTimeSeconds))
            {
                return nlDateTimeSeconds.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Try JavaScript ISO string format
            if (DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind, out DateTime jsDate))
            {
                return jsDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Last resort: try with Dutch culture
            if (DateTime.TryParse(dateStr, new System.Globalization.CultureInfo("nl-NL"),
                System.Globalization.DateTimeStyles.None, out DateTime nlDate))
            {
                return nlDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return dateStr; // Return as-is if parsing fails
        }

        [HttpPost]
        public async Task<IActionResult> MoveEvent([FromBody] Event evt)
        {
            if (evt == null || evt.EventId <= 0)
            {
                return BadRequest("Invalid event data.");
            }

            // Get current user info
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Convert to ISO format for database storage
            evt.Start = ParseAndFormatDate(evt.Start);
            evt.End = string.IsNullOrEmpty(evt.End) ? null : ParseAndFormatDate(evt.End);

            // Update only the date/time fields
            string message = _DA.MoveEvent(evt.EventId, evt.Start, evt.End, evt.AllDay, user.Id);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Event succesvol verplaatst." });
        }

        [HttpPost]
        public async Task<IActionResult> ResizeEvent([FromBody] Event evt)
        {
            if (evt == null || evt.EventId <= 0)
            {
                return BadRequest("Invalid event data.");
            }

            // Get current user info
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Convert to ISO format for database storage
            evt.Start = ParseAndFormatDate(evt.Start);
            evt.End = string.IsNullOrEmpty(evt.End) ? null : ParseAndFormatDate(evt.End);

            // Update only the date/time fields
            string message = _DA.ResizeEvent(evt.EventId, evt.Start, evt.End, user.Id);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Event succesvol aangepast." });
        }

        [HttpPost]
        public async Task<IActionResult> ShareEvent([FromBody] EventShare share)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            share.OwnerUserId = user.Id;
            share.SharedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            // Get the user being shared with to populate their name
            var sharedWithUser = await _userManager.FindByIdAsync(share.SharedWithUserId);
            if (sharedWithUser == null)
            {
                return BadRequest(new { message = "Gebruiker niet gevonden." });
            }

            share.SharedWithUserName = sharedWithUser.DisplayName ?? sharedWithUser.UserName;

            string message = _DA.ShareEvent(share);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Event succesvol gedeeld." });
        }

        [HttpPost]
        public async Task<IActionResult> UnshareEvent([FromBody] EventShare share)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            string message = _DA.UnshareEvent(share.EventId, share.SharedWithUserId, user.Id);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { message = "Delen van event ingetrokken." });
        }

        [HttpGet]
        public IActionResult GetEventShares(int eventId)
        {
            var shares = _DA.GetEventShares(eventId);
            return Json(shares);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = _userManager.Users
                .Where(u => u.Id != currentUser.Id)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.DisplayName ?? u.UserName,
                    email = u.Email
                })
                .ToList();

            return Json(users);
        }

        [HttpGet]
        public IActionResult ExportICalendar(string start, string end)
        {
            // Convert JavaScript date strings to ISO format for SQLite
            DateTime startDate = DateTime.Parse(start);
            DateTime endDate = DateTime.Parse(end);

            string startISO = startDate.ToString("yyyy-MM-dd HH:mm:ss");
            string endISO = endDate.ToString("yyyy-MM-dd HH:mm:ss");

            // Get current user's events only
            List<Event> allEvents = _DA.GetCalendarEvents(startISO, endISO);
            var currentUserId = _userManager.GetUserId(User);

            // Filter to only user's own events
            var userEvents = allEvents.Where(e => e.UserId == currentUserId).ToList();

            // Process recurring events
            var eventsToExport = new List<Event>();
            foreach (var evt in userEvents)
            {
                if (evt.IsRecurring)
                {
                    // For recurring events, export the master event with RRULE
                    eventsToExport.Add(evt);
                }
                else
                {
                    eventsToExport.Add(evt);
                }
            }

            // Generate iCal content
            string iCalContent = ICalService.GenerateICalendar(eventsToExport, "Mijn Agenda");

            // Return as downloadable file
            var bytes = System.Text.Encoding.UTF8.GetBytes(iCalContent);
            return File(bytes, "text/calendar", $"agenda_{DateTime.Now:yyyyMMdd}.ics");
        }

        [HttpGet]
        public IActionResult ExportEventICalendar(int eventId)
        {
            // Get the specific event
            var events = _DA.GetCalendarEvents(
                DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                DateTime.Now.AddYears(2).ToString("yyyy-MM-dd HH:mm:ss")
            );

            var evt = events.FirstOrDefault(e => e.EventId == eventId);
            if (evt == null)
            {
                return NotFound("Event niet gevonden.");
            }

            // Check if user owns this event
            var currentUserId = _userManager.GetUserId(User);
            if (evt.UserId != currentUserId)
            {
                return Forbid("Je mag alleen je eigen events exporteren.");
            }

            // Generate iCal content for single event
            string iCalContent = ICalService.GenerateICalendar(new List<Event> { evt }, evt.Title);

            // Return as downloadable file
            var bytes = System.Text.Encoding.UTF8.GetBytes(iCalContent);
            return File(bytes, "text/calendar", $"event_{evt.EventId}.ics");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() 
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
