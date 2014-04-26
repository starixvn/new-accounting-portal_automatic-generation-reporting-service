// --------------------------------------------------------------------------
// <copyright file="RMSection.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Configurations
{
    using System.Configuration;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RMSection : ConfigurationSection
    {
        public enum RMType
        {
            /// <summary>
            /// The eight RM.
            /// </summary>
            EightRM = 0,

            /// <summary>
            /// The BARM type.
            /// </summary>
            BARM = 1
        }

        public static RMSection Instance
        {
            get
            {
                var storeProcedureSection =
                 ConfigurationManager.GetSection("RMSection") as RMSection;
                return storeProcedureSection;
            }
        }

        [ConfigurationProperty("Report", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RMCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RMCollection Report
        {
            get
            {
                var storeProcedureCollection =
                    (RMCollection)base["Report"];
                return storeProcedureCollection;
            }
        }
    }
}
