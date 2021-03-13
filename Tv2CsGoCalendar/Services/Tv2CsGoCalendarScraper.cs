using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.Extensions.Caching.Memory;
using Calendar = Ical.Net.Calendar;

namespace Tv2CsGoCalendar.Services
{
    public class Tv2CsGoCalendarScraper
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public Tv2CsGoCalendarScraper(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        private async Task<Calendar> ScrapeCalendar()
        { 
            var context = BrowsingContext.New(Configuration.Default);

            var body = await _httpClient.GetStringAsync("https://sport.tv2.dk/e-sport/2021-02-02-hvornaar-er-der-csgo-paa-zulu");
            var document = await context.OpenAsync(req => req.Content(body));

            var contentDiv = document.QuerySelector("div.tc_richcontent");

            var calendar = new Calendar();
            calendar.AddProperty("X-WR-CALNAME", "CsGo på TV2");
            calendar.AddProperty("X-WR-TIMEZONE", "Europe/Amsterdam");

            var uls = contentDiv.QuerySelectorAll("ul");
            var dateString = "";
            foreach (var ul in uls)
            {
                var dateEl = ul.PreviousElementSibling.FirstElementChild;
                if (dateEl.TagName == "STRONG")
                {
                    dateString = dateEl.InnerHtml.Split(':').LastOrDefault()?.Replace("D.", "").Trim();
                }

                foreach (var li in ul.Children)
                {
                    var time = li.InnerHtml.Split(":").FirstOrDefault();
                    DateTime.TryParse(dateString + $" {DateTime.Now.Year} {time}", new CultureInfo("da-dk"), DateTimeStyles.None, out var startDate);

                    var name = li.InnerHtml.Replace(time + ":", "").Replace(System.Environment.NewLine, string.Empty).Trim();

                    calendar.Events.Add(new CalendarEvent
                    {
                        Summary = name,
                        DtStart = new CalDateTime(startDate),
                        Start = new CalDateTime(startDate),
                        DtEnd = new CalDateTime(startDate.AddHours(1)),
                        End = new CalDateTime(startDate.AddHours(1)),

                    });
                }
            }

            return calendar;
        }

        public async Task<Calendar> ScrapeAndBuildCalendar()
        {
            var cached = await _memoryCache.GetOrCreateAsync<Calendar>("", entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                return ScrapeCalendar();
            });

            return cached;
        }
    }
}
