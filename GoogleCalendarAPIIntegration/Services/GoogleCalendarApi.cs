using GoogleCalendarAPIIntegration.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
//using Google.Apis.Calendar.v3.Data;
using Event = GoogleCalendarAPIIntegration.Models.Event;

namespace GoogleCalendarAPIIntegration.Services
{
    public class GoogleCalendarApi : IGoogleCalendarApi
    {
        private readonly string _tokenFilePath;
        private readonly string _googleApiKey;
        private readonly string _googleCalendarApiBaseUrl;
        private readonly GoogleApiOptions _googleApiOptions;

        private readonly RestClient _restClient;
        private readonly RestRequest _restRequest;

        public GoogleCalendarApi(IOptions<GoogleApiOptions> options)
        {
            _googleApiOptions = options.Value;
            _tokenFilePath = options.Value.TokenFilePath;
            _googleApiKey = options.Value.GoogleApiKey;
            _googleCalendarApiBaseUrl = options.Value.GoogleCalendarApiBaseUrl;

            _restClient = new RestClient();
            _restRequest = new RestRequest();

            _restRequest.AddQueryParameter("key", _googleApiKey);
        }

        public Event CreateEvent(Event calendarEvent, string reminderMinutes, string reminderMethod)
        {
            var tokens = JObject.Parse(File.ReadAllText(_tokenFilePath));

            calendarEvent.Start.DateTime = DateTime.Parse(calendarEvent.Start.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            calendarEvent.End.DateTime = DateTime.Parse(calendarEvent.End.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            if(reminderMethod != null)
            {
               var reminderOverrides= GetReminderOverrides(reminderMethod, reminderMinutes);

                calendarEvent.Reminders = new Reminders()
                {
                    Overrides = reminderOverrides,
                    UseDefault = false
                };
            }
            var model = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            AddAuthorizationHeader();
            _restRequest.AddParameter("application/json", model, ParameterType.RequestBody);
            _restRequest.AddParameter("sendUpdates", "all");


            _restClient.BaseUrl = new Uri(_googleCalendarApiBaseUrl);
            var response = _restClient.Post(_restRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject calendarEventJson = JObject.Parse(response.Content);
                return calendarEventJson.ToObject<Event>();
            }
            throw new Exception($"Error creating event: {response.StatusCode}");
        }

        public Event GetEvent(string identifier)
        {
            AddAuthorizationHeader();
            _restClient.BaseUrl = new Uri($"{_googleCalendarApiBaseUrl}/{identifier}");
            var response = _restClient.Get(_restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject calendarEventJson = JObject.Parse(response.Content);
                return calendarEventJson.ToObject<Event>();
            }
            throw new Exception($"Error getting event: {response.StatusCode}");
        }

        public IEnumerable<Event> GetAllEvents()
        {
            AddAuthorizationHeader();

            _restClient.BaseUrl = new Uri(_googleCalendarApiBaseUrl);
            var response = _restClient.Get(_restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject calendarEvents = JObject.Parse(response.Content);
                var allEvents = calendarEvents["items"].ToObject<IEnumerable<Event>>();
                return allEvents;
            }

            throw new Exception($"Error getting event: {response.StatusCode}");
        }

        public bool UpdateEvent(string identifier, Event calendarEvent, string reminderMinutes, string reminderMethod)
        {

            calendarEvent.Start.DateTime = DateTime.Parse(calendarEvent.Start.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            calendarEvent.End.DateTime = DateTime.Parse(calendarEvent.End.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
            if (reminderMethod != null)
            {
                var reminderOverrides = GetReminderOverrides(reminderMethod, reminderMinutes);

                calendarEvent.Reminders = new Reminders()
                {
                    Overrides = reminderOverrides,
                    UseDefault = false
                };
            }

            var model = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            AddAuthorizationHeader();
            _restRequest.AddParameter("application/json", model, ParameterType.RequestBody);

            _restClient.BaseUrl = new Uri($"{_googleCalendarApiBaseUrl}/{identifier}");
            var response = _restClient.Patch(_restRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            throw new Exception($"Error updating event: {response.StatusCode}");
        }
        public bool DeleteEvent(string identifier)
        {
            AddAuthorizationHeader();
            _restRequest.AddParameter("sendUpdates", "all");


            _restClient.BaseUrl = new Uri($"{_googleCalendarApiBaseUrl}/{identifier}");
            var response = _restClient.Delete(_restRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                throw new Exception($"Error deleting event: {response.StatusCode}");
            }
        }

        #region Private Methods

        private Reminder[] GetReminderOverrides(string reminderMethod, string reminderMinutes)
        {
            var reminderOverrides = new List<Reminder>();

            if (reminderMethod == "email")
            {
                reminderOverrides.Add(new Reminder
                {
                    Minutes = int.Parse(reminderMinutes),
                    Method = "email"
                });
            }
            else if (reminderMethod == "popup")
            {
                reminderOverrides.Add(new Reminder
                {
                    Minutes = int.Parse(reminderMinutes),
                    Method = "popup"
                });
            }
            else if (reminderMethod == "both")
            {
                reminderOverrides.Add(new Reminder
                {
                    Minutes = int.Parse(reminderMinutes),
                    Method = "email"
                });

                reminderOverrides.Add(new Reminder
                {
                    Minutes = int.Parse(reminderMinutes),
                    Method = "popup"
                });
            }

            return reminderOverrides.ToArray();
        }

        private void AddAuthorizationHeader()
        {
            var tokens = JObject.Parse(File.ReadAllText(_tokenFilePath));

            _restRequest.AddHeader("Authorization", "Bearer " + tokens["access_token"]);
            _restRequest.AddHeader("Accept", "application/json");
            _restRequest.AddHeader("Content-Type", "application/json");

        }
        #endregion

    }
}