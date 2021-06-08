/*
 * 
 * Copyright (c) 2021 AoiKamishiro
 * Released under the MIT license
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the “Software”), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
*/

using System;

/// <summary>
/// Example Json Class
/// </summary>
namespace CalendarBuilderSample
{
    public class CalendarEvent
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Summary { get; set; }
        public string TimeZone { get; set; }

        public string GetStartDateString()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(StartTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)).ToString("yyyy/MM/dd");
        }
        public string GetDateString()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(StartTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)).ToString("MM月dd日") + " (" + GetDayOfWeek() + ")";
        }
        public string GetStartTimeString()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(StartTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)).ToString("HH:mm");
        }
        public string GetEndTimeString()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(EndTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)).ToString("HH:mm");
        }
        public string GetDayOfWeek()
        {
            switch (TimeZoneInfo.ConvertTimeFromUtc(StartTime.UtcDateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone)).DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "日";
                case DayOfWeek.Monday:
                    return "月";
                case DayOfWeek.Tuesday:
                    return "火";
                case DayOfWeek.Wednesday:
                    return "水";
                case DayOfWeek.Thursday:
                    return "木";
                case DayOfWeek.Friday:
                    return "金";
                case DayOfWeek.Saturday:
                    return "土";
                default:
                    return "不";
            };
        }
    }

}
