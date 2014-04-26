// -----------------------------------------------------------------------
// <copyright file="DirectoryExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Common
{
    using System.IO;

    /// <summary>
    /// Directory utility.
    /// </summary>
    public static class DirectoryExtensions
    {
        private static readonly int _MaxNumberOfDirectories = 3;

        /// <summary>
        /// Check the and create directory.
        /// </summary>
        /// <param name="path">The path of directory.</param>
        public static void CheckAndCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path of directory.</param>
        public static void DeleteDirectory(string path)
        {
            var directories = Directory.GetDirectories(path);
            var directoryLength = directories.Length;
            if (directoryLength > _MaxNumberOfDirectories)
            {
                //// Delete old data. Remain data 3 lasted month.
                for (int i = 0; i < directoryLength - _MaxNumberOfDirectories; i++)
                {
                    var directory = directories[i];
                    Directory.Delete(directory);
                }
            }
        }
    }
}