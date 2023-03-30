using GoogleCalendarAPIIntegration.Models;
using GoogleCalendarAPIIntegration.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCalendarAPIIntegration.Controllers
{
    public class CalendarEventController : Controller
    {
        private readonly IGoogleCalendarApi _googleCalendarApi;

        public CalendarEventController(IGoogleCalendarApi googleCalendarApi)
        {
            _googleCalendarApi = googleCalendarApi;
        }

        [HttpPost]
        public ActionResult CreateEvent(Event calendarEvent,  string reminderMinutes, string reminderMethod)
        {
            // Parse start and end times from the input event
            if (DateTime.TryParse(calendarEvent.Start.DateTime, out DateTime startTime) && DateTime.TryParse(calendarEvent.End.DateTime, out DateTime endTime))
            {
                // Validate that the end time is after the start time
                if (endTime <= startTime)
                {
                    // Return an error message if end time is not after start time
                    var errorMessage = "The end time must be after the start time.";
                    ViewBag.ErrorMessage = errorMessage;
                    return View("Error");
                }
            }

            try
            {
                // Create the event using the Google Calendar API
                var createdEvent = _googleCalendarApi.CreateEvent(calendarEvent, reminderMinutes, reminderMethod);
            }
            catch (Exception ex)
            {
                // Return an error message if an exception occurs during the event creation
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }

            // Redirect to the list of all events with a success message
            return RedirectToAction("AllEvents", "CalendarEvent", new { status = "created" });
        }


        public ActionResult Event(string identifier)
        {
            try
            {
                var getEvent = _googleCalendarApi.GetEvent(identifier);
                return View(getEvent);

            }
            catch (Exception ex)
            {
                var responseContent = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = responseContent;
                return View("Error");
            }
        }

        public ActionResult AllEvents()
        {
            try
            {
                var allEvents = _googleCalendarApi.GetAllEvents();
                return View(allEvents);
            }
            catch (Exception ex)
            {
                var responseContent = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = responseContent;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult UpdateEvent(string identifier)
        {
            try
            {
                var calendarEvent = _googleCalendarApi.GetEvent(identifier);
                return View(calendarEvent);
            }
            catch (Exception ex)
            {
                var responseContent = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = responseContent;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult UpdateEvent(string identifier, Event calendarEvent, string reminderMinutes, string reminderMethod)
        {
            if (!ModelState.IsValid)
            {
                return View(calendarEvent);
            }
            try
            {
                var result = _googleCalendarApi.UpdateEvent(identifier, calendarEvent, reminderMinutes, reminderMethod);
                return RedirectToAction("AllEvents", "CalendarEvent", new { status = "updated" });

            }
            catch (Exception ex)
            {
                var responseContent = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = responseContent;
                return View("Error");
            }

        }

        public ActionResult DeleteEvent(string identifier)
        {
            try
            {
                   _googleCalendarApi.DeleteEvent(identifier);
                    return RedirectToAction("AllEvents", "CalendarEvent", new { status = "deleted" });
            }
            catch (Exception ex)
            {
                var responseContent = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ViewBag.ErrorMessage = responseContent;
                return View("Error");
            }
            
        }

        public IActionResult Search(string searchTerm)
        {
            var events = _googleCalendarApi.GetAllEvents();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return View(events); // Return all events if search term is empty
            }

            // Filter events by search term
            List<Event> filteredEvents = events.Where(e =>
                e.Summary != null && e.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Description != null && e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            return View(filteredEvents);
        }

    }
}
