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

            // Map to FullCalendar format with 'id' instead of 'EventId'
            var calendarEvents = events.Select(e => new {
                id = e.EventId,
                title = e.Title,
                start = e.Start,
                end = e.End,
                allDay = e.AllDay,
                editable = e.UserId == currentUserId, // Only own events are editable
                extendedProps = new {
                    description = e.Description,
                    eventId = e.EventId,
                    userName = e.UserName ?? "Onbekend",
                    isOwn = e.UserId == currentUserId
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

            message = _DA.AddEvent(evt, out eventId);

            if (!string.IsNullOrEmpty(message))
            {
                return BadRequest(new { message });
            }

            return Json(new { 
                message = "Event succesvol toegevoegd.",
                eventId = eventId,
                userId = evt.UserId,
                userName = evt.UserName
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() 
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
