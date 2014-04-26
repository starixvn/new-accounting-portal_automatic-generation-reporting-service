// --------------------------------------------------------------------------
// <copyright file="NumberExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.6">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Extensions
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this int value)
        {
            return DoFormat(value, 0);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this object value)
        {
            return DoFormat(value, 0);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decDigits">The decimal digits.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this decimal value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decDigits">The decimal digits.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this object value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decDigits">The decimal digits.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this double value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decDigits">The decimal digits.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this float value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Does the format.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="decDigits">The decimal digits.</param>
        /// <returns>The formatted string.</returns>
        /// <exception cref="System.NullReferenceException">Null Reference Exception.</exception>
        private static string DoFormat(object value, byte decDigits)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            string formatString = "{0:n" + decDigits.ToString() + "}";
            return string.Format(formatString, value);
        }
    }
}
