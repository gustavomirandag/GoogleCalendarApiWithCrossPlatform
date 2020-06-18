using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalendarManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleCalendarManagerApp
{
    public class GoogleCalendarService
    {
        public static string[] Scopes = { CalendarService.Scope.Calendar };
        public static string ApplicationName = "CalendarConsole";

        private string CredentialsPath = string.Empty;

        public GoogleCalendarService(string credentialsPath)
        {
            CredentialsPath = credentialsPath;
        }

        public IEnumerable<String> ShowUpCommingEvent()
        {
            UserCredential credential = GetCredential(UserRole.User);

            // Creat Google Calendar API service.
            CalendarService service = GetService(credential);

            // Define parameters of request
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            // Prepare upcomming events result
            var result = new List<String>();
            Console.WriteLine("Upcomming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    result.Add($"{eventItem.OriginalStartTime?.DateTime} - {eventItem.Summary}");
                }
            }
            return result;
        }

        public void CreateEvent(String title, String[] participants, DateTime startDateTime, DateTime finishDateTime)
        {
            UserCredential credential = GetCredential(UserRole.Admin);
            CalendarService service = GetService(credential);

            Event newEvent = new Event()
            {
                Summary = title,
                Start = new EventDateTime() { DateTime = startDateTime },
                End = new EventDateTime() { DateTime = finishDateTime },
                Attendees = new List<EventAttendee>()
            };
            foreach(var participant in participants)
            {
                var attendee = new EventAttendee
                {
                    Email = participant
                };
                newEvent.Attendees.Add(attendee);
            }

            string calendarId = "primary";

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            request.SendNotifications = true;
            Event createdEvent = request.Execute();
        }

        private CalendarService GetService(UserCredential credential)
        {
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        private UserCredential GetCredential(UserRole userRole)
        {
            UserCredential credential;
            using (var stream =
                new FileStream(CredentialsPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                userRole.ToString(),
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;

                Console.WriteLine($"Credential file saved to: {credPath}");
            }

            return credential;
        }
    }
}