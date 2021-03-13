using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ical.Net;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Tv2CsGoCalendar.Formatters
{
    public class CalendarFormatter : TextOutputFormatter
    {
        public CalendarFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/calendar"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        protected override bool CanWriteType(Type type)
        {
            return typeof(Calendar).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            var buffer = new StringBuilder();
            var serializer = new CalendarSerializer();

            buffer.Append(serializer.SerializeToString(context.Object));


            await httpContext.Response.WriteAsync(buffer.ToString(), selectedEncoding);
        }
    }
}
