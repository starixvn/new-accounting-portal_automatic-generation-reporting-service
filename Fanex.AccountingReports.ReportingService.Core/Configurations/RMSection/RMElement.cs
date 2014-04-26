// --------------------------------------------------------------------------
// <copyright file="RMElement.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Configurations
{
    using System.Configuration;
    using Fanex.Common;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RMElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RMElement"/> class.
        /// </summary>
        public RMElement()
        {
        }

        [ConfigurationProperty("Id", IsRequired = false)]
        public string Id
        {
            get
            {
                return Converter.ToString2(this["Id"]);
            }

            set
            {
                this["Id"] = value;
            }
        }

        [ConfigurationProperty("Name", IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }

            set
            {
                this["Name"] = value;
            }
        }

        [ConfigurationProperty("DisplayName", IsRequired = false)]
        public string DisplayName
        {
            get
            {
                return (string)this["DisplayName"];
            }

            set
            {
                this["DisplayName"] = value;
            }
        }

        [ConfigurationProperty("IsActive", IsRequired = false)]
        public bool IsActive
        {
            get
            {
                return Converter.ToBool(this["IsActive"]);
            }

            set
            {
                this["IsActives"] = value;
            }
        }

        [ConfigurationProperty("ReportConfigs", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RMCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RMCollection ReportConfigs
        {
            get
            {
                var stores = (RMCollection)base["ReportConfigs"];
                return stores;
            }
        }
    }
}
