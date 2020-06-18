using Google.Apis.Services;
using System;
using System.Threading.Tasks;

namespace GoogleCalendarManagerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var googleCalendar = new GoogleCalendar("my_googlecalendar_credentials.json");
            googleCalendar.ShowUpCommingEvent();
            googleCalendar.CreateEvent();
        }
    }
}
