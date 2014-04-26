// -----------------------------------------------------------------------
// <copyright file="AppSettings.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService
{
    using System;
    using System.Configuration;

    /// <summary>
    /// App Settings.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public static string ServiceName
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ServiceName"];
                return val == null ? "_Fanex.AccountingReports.ReportingService" : val;
            }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public static int Interval
        {
            get
            {
                var val = ConfigurationManager.AppSettings["Interval.Second"];
                return val == null ? 300 : Convert.ToInt32(ConfigurationManager.AppSettings["Interval.Second"]) * 1000;
            }
        }

        /// <summary>
        /// Gets run at hours.
        /// </summary>
        /// <value>
        /// The Hours.
        /// </value>
        public static int RunAtHours
        {
            get
            {
                var val = ConfigurationManager.AppSettings["RunAtHours"];
                return val == null ? 3 : Convert.ToInt32(ConfigurationManager.AppSettings["RunAtHours"]);
            }
        }

        /// <summary>
        /// Gets run at minutes.
        /// </summary>
        /// <value>
        /// The Minute.
        /// </value>
        public static int RunAtMinutes
        {
            get
            {
                var val = ConfigurationManager.AppSettings["RunAtMinutes"];
                return val == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["RunAtMinutes"]);
            }
        }

        /// <summary>
        /// Gets connection string.
        /// </summary>
        /// <value>
        /// The Minute.
        /// </value>
        public static string ConnectionString
        {
            get
            {
                var val = ConfigurationManager.AppSettings["SQLiteDBPath"];
                return val == null ? string.Empty : Convert.ToString(ConfigurationManager.AppSettings["SQLiteDBPath"]);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [on processing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [on processing]; otherwise, <c>false</c>.
        /// </value>
        public static bool OnProcessing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [force running on processing].
        /// </summary>
        /// <value>
        /// <c>true</c> if [force running on processing]; otherwise, <c>false</c>.
        /// </value>
        public static bool ForceRunningOnProcessing { get; set; }

        /// <summary>
        /// Gets Retry.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public static int Retry
        {
            get
            {
                var val = ConfigurationManager.AppSettings["Retry"];
                return val == null ? 5 : Convert.ToInt32(ConfigurationManager.AppSettings["Retry"]);
            }
        }

        public static bool ForceRunning
        {
            get
            {
                var val = ConfigurationManager.AppSettings["ForceRunning"];
                return val == null ? false : (Convert.ToInt32(ConfigurationManager.AppSettings["ForceRunning"]) == 1);
            }
        }

        public static DateTime WinlostDateForceRunning
        {
            get
            {
                var val = ConfigurationManager.AppSettings["WinlostDateForceRunning"];
                return val == null ? DateTime.Today : Convert.ToDateTime(ConfigurationManager.AppSettings["WinlostDateForceRunning"]);
            }
        }
    }
}