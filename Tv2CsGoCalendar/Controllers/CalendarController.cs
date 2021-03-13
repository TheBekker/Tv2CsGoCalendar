using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.Serialization;
using Tv2CsGoCalendar.Services;

namespace Tv2CsGoCalendar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly ILogger<CalendarController> _logger;
        private readonly Tv2CsGoCalendarScraper _scraper;

        public CalendarController(ILogger<CalendarController> logger, Tv2CsGoCalendarScraper scraper)
        {
            _logger = logger;
            _scraper = scraper;
        }

        [HttpGet]
        [Produces("text/calendar")]
        public async Task<Calendar> Get()
        {
            var calendar = await _scraper.ScrapeAndBuildCalendar();

            return calendar;
        }
    }
}
