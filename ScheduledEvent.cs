using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VT2Scheduler
{
    class ScheduledEvent
    {
        private string _date;
        public string Type;
        public string VT;
        public DateTime BriefTime;
        public DateTime EDT;
        public DateTime RTB;
        public string Instructor;
        public string Student;
        public string Event;
        public double Hours;
        public string Remarks;
        public string Location;


        public ScheduledEvent(List<string> data, DateTime date)
        {
            Type = data[0];
            VT = data[1];
            BriefTime = eventTime(data[2], date);
            EDT = eventTime(data[3], date);
            RTB = eventTime(data[4], date);
            Instructor = data[5];
            Student = data[6];
            Event = data[7];
            Hours = eventDuration(data[8]);
            Remarks = data[9];
            Location = data[10];
        }

        private Tuple<int, int> toHoursMintues(string time)
        {
            Regex rx = new Regex(@"(\d\d):(\d\d)");
            // program crashes here because the header row is sent through; make sure header row can safely pass through,
            //    then remove header row from data being converted
            var matches = rx.Matches(time);
            if (matches.Count == 1)
            {
                int hours = Convert.ToInt32(matches[0].Groups[1].Value);
                int minutes = Convert.ToInt32(matches[0].Groups[2].Value);
                return new Tuple<int, int>(hours, minutes);
            }
            else return new Tuple<int, int>(0, 0);
        }

        private DateTime eventTime (string hoursMintues, DateTime day)
        {
            Tuple<int, int> timeHoursMinutes = toHoursMintues(hoursMintues);
            return new DateTime(day.Year, day.Month, day.Day, timeHoursMinutes.Item1, timeHoursMinutes.Item2, 0);
        }

        private double eventDuration (string duration)
        {
            double retval;
            try {
                retval = Convert.ToDouble(duration);    
            }
            catch (FormatException e)
            {
                retval = 0;
            }
            return retval;
        }
    }

}
