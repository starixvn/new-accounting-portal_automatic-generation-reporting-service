// --------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.10.11">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Extensions
{
    using System;
    using Fanex.AccountingReports.ReportingService.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static string ToShortFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDate();
            }

            return string.Empty;
        }

        public static string ToShortFormatedDate(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.ShortDate);
        }

        public static string ToShortFormatedDateWithHourMinute(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDateWithHourMinute();
            }

            return string.Empty;
        }

        public static string ToShortFormatedDateWithHourMinute(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.ShortDateWithHourMinute);
        }

        public static string ToLongFormatedDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToLongFormatedDate();
            }

            return string.Empty;
        }

        public static string ToLongFormatedDate(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.LongDate);
        }

        public static string ToMonthAndDay(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDay();
            }

            return string.Empty;
        }

        public static string ToMonthAndDay(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.MonthDay);
        }

        public static string ToMonthAndDayWithNoHyphen(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthAndDayWithNoHyphen();
            }

            return string.Empty;
        }

        public static string ToMonthAndDayWithNoHyphen(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.MonthDayWithNoHyphen);
        }

        public static string ToMonthYear(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthYear();
            }

            return string.Empty;
        }

        public static string ToMonthYear(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.MonthYear);
        }

        public static string ToShortFormatedDateWithSlash(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToShortFormatedDateWithSlash();
            }

            return string.Empty;
        }

        public static string ToLongFormatedDateWithSlash(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.LongDateWithSlash);
        }

        public static string ToShortFormatedDateWithSlash(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.ShortDateWithSlash);
        }

        public static string ToMonthYearWithSlash(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.ToMonthYearWithSlash();
            }

            return string.Empty;
        }

        public static string ToMonthYearWithSlash(this DateTime date)
        {
            return date.ToString(DateTimeFormatConstants.MonthYearWithSlash);
        }

        public static string GetFullMonthName(int month)
        {
            return new DateTime(1, month, 1).ToString(DateTimeFormatConstants.FullMonthName);
        }

        public static DateTime FirstDateOfWeek(int year, int week)
        {
            DateTime firstDateOfYear = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - firstDateOfYear.DayOfWeek;
            DateTime firstMonday = firstDateOfYear.AddDays(daysOffset);
            return firstMonday.AddDays((week - 1) * 7);
        }

        public static DateTime LastDateOfWeek(int year, int week)
        {
            DateTime firstDateOfYear = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - firstDateOfYear.DayOfWeek;
            DateTime firstMonday = firstDateOfYear.AddDays(daysOffset);
            return firstMonday.AddDays((week * 7) - 1);
        }

        public static DateTime GetEndOfDate(this DateTime date)
        {
            if (date == null)
            {
                date = DateTime.Today;
            }

            date = date.AddDays(1).AddSeconds(-1);
            return date;
        }
    }
}