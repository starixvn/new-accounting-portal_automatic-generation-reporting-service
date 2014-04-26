// --------------------------------------------------------------------------
// <copyright file="DateTimeFormatConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DateTimeFormatConstants
    {
        public const string ShortDate = "MMM dd, yyyy";
        public const string ShortDateWithHourMinute = "MMM dd, yyyy hh:mm";
        public const string ShortTime = "{0: H:mm}";
        public const string MonthYear = "MMM yyyy";
        public const string ShortDateWithSlash = "MM/dd/yyyy";
        public const string LongDateWithSlash = "MM/dd/yyyy hh:mm:ss tt";
        public const string MonthYearWithSlash = "MM/yyyy";
        public const string LongDate = "dd MMM, yyyy hh:mm:ss tt";
        public const string MonthDay = "dd-MMM";
        public const string MonthDayWithNoHyphen = "dd MMM";
        public const string FullMonthName = "MMMM";
        public const string YearMonthDateWithSlash = "yyyy/MM/dd";
    }
}
