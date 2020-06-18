using GoogleCalendarManagerApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyCalendarMobileApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mycalendar_credentials.json");
            var googleCalendar = new GoogleCalendarService(path);
            var eventTitle = EntryEventTitle.Text;
            var participants = EntryParticipants.Text.Split(';');
            var eventDateTime = new DateTime(DatePickerMeeting.Date.Year,
                DatePickerMeeting.Date.Month,
                DatePickerMeeting.Date.Day,
                TimePickerMeeting.Time.Hours,
                TimePickerMeeting.Time.Minutes,
                TimePickerMeeting.Time.Seconds
               );
            googleCalendar.CreateEvent(eventTitle, participants, eventDateTime, eventDateTime.AddHours(1));
        }
    }
}
