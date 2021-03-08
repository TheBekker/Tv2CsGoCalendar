using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Events;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Calendar = Ical.Net.Calendar;

namespace Tv2CsGoCalendar.Services
{
    public class Tv2CsGoCalendarScraper
    {
        private readonly HttpClient _httpClient;

        public Tv2CsGoCalendarScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Calendar> ScrapeAndBuildCalendar()
        {
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            var body = await _httpClient.GetStringAsync("https://sport.tv2.dk/e-sport/2021-02-02-hvornaar-er-der-csgo-paa-zulu");

            //Create a virtual request to specify the document to load (here from our fixed string)
            var document = await context.OpenAsync(req => req.Content(body));

            var contentDiv = document.QuerySelector("div.tc_richcontent");

            var calendar = new Calendar();

            var uls = contentDiv.QuerySelectorAll("ul");
            foreach (var ul in uls)
            {
                var dateEl = ul.PreviousElementSibling.FirstElementChild;
                var dateString = dateEl.InnerHtml.Split(':').LastOrDefault().Replace("D.", "").Trim();

                var lis = ul.QuerySelectorAll("li");
                foreach (var li in lis)
                {
                    var time = li.InnerHtml.Split(":").FirstOrDefault();
                    DateTime.TryParse(dateString + $" {DateTime.Now.Year} {time}", new CultureInfo("da-dk"), DateTimeStyles.None, out var startDate);

                    var name = li.InnerHtml.Replace(time + ":", "").Trim();

                    calendar.Events.Add(new CalendarEvent
                    {
                        Summary = name,
                        Start = new CalDateTime(startDate),
                        End = new CalDateTime(startDate.AddHours(1))
                    });
                }
            }

            return calendar;
        }
    }
}
