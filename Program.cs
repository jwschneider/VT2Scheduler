using System;
using System.Collections.Generic;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Windows;


namespace VT2Scheduler
{
    class Program
    {
        static string url = "https://www.cnatra.navy.mil/scheds/schedule_data.aspx?sq=vt-2";
        static HtmlWeb web = new HtmlWeb();
        static HttpClient client = new HttpClient();

        static DateTime scheduleDate;
        static MyCalendar calendar = new MyCalendar();

        static void Main(string[] args)
        {
            scheduleDate = new DateTime(2020, 8, 7);
            HtmlDocument mySchedule = GetScheduleByName("Schneider", scheduleDate);
            //FileStream fs = File.Create("out.html");
            //mySchedule.Save(fs);
            List<ScheduledEvent> myEvents = ParseByEvent(mySchedule);
            foreach (ScheduledEvent schedueldevent in myEvents)
            {
                calendar.AddEvent(schedueldevent);
            }

        }

        static List<ScheduledEvent> ParseByEvent(HtmlDocument schedule)
        {
            HtmlNode eventData = schedule.GetElementbyId("dgEvents");
            List<List<string>> tableData = new List<List<string>>();
            if (eventData != null)
            {
                var tableBody = eventData.ChildNodes;
                foreach (var child in tableBody)
                {
                    if (child.Name == "tr")
                    {
                        List<string> rowValues = new List<string>();
                        var rowElements = child.ChildNodes;
                        foreach (var elem in rowElements)
                        {
                            rowValues.Add(elem.InnerText);
                        }
                        rowValues.RemoveAt(0);
                        rowValues.RemoveAt(rowValues.Count - 1);
                        tableData.Add(rowValues);
                    }
                }
                tableData.RemoveAt(0);
            }
            List<ScheduledEvent> events = new List<ScheduledEvent>();
            foreach (var eventAsList in tableData)
            {
                events.Add(new ScheduledEvent(eventAsList, scheduleDate));
            }
            return events;
        }

        static HtmlDocument GetPageInitial()
        {
            return web.Load(url);
        }

        static HtmlDocument GetCalendarDay(DateTime date)
        {
            string calendarDayKey = CalendarDayIdentifier(date);
            HtmlDocument initial = GetPageInitial();
            var postBackInitial = copyStateAndValidation(initial);
            postBackInitial.Add("__EVENTTARGET", "ctrlCalendar");
            postBackInitial.Add("__EVENTARGUMENT", calendarDayKey);
            return PostBack(postBackInitial);
        }

        private static string CalendarDayIdentifier(DateTime date)
        {
            DateTime baseline = new DateTime(2020, 8, 7);
            int baselineKey = 7524;
            int daysElapsed = date.Subtract(baseline).Days;
            int dateKey = baselineKey + daysElapsed;
            return dateKey.ToString();

        }

        static HtmlDocument GetScheduleByName(string name, DateTime date)
        {
            HtmlDocument calendarDay = GetCalendarDay(date);
            var PostBackValuesDay = copyStateAndValidation(calendarDay);
            PostBackValuesDay.Add("txtNameSearch", "Schneider");
            PostBackValuesDay.Add("btnFilter", "Search");
            return PostBack(PostBackValuesDay);
        }

        static HtmlDocument PostBack(Dictionary<string, string> stateAndValidation)
        {
            var content = new FormUrlEncodedContent(stateAndValidation);
            var response = client.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();
            string responseString = response.Content.ReadAsStringAsync().Result;
            var doc = new HtmlDocument();
            doc.LoadHtml(responseString);
            return doc;
        }

        static Dictionary<string, string> copyStateAndValidation(HtmlDocument doc)
        {
            var values = new Dictionary<string, string>();
            var postBackData = new[]
            {
                "__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION"
            };
            foreach (var elem in postBackData)
            {
                values.Add(elem, GetAttributeValueById(doc, elem));
            }
            return values;
        }

        static string GetAttributeValueById(HtmlDocument doc, string id)
        {
            HtmlNode node = doc.GetElementbyId(id);
            return node.Attributes["value"].Value;
        }
    }
}


