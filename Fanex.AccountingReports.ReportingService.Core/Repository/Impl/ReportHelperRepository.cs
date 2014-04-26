// --------------------------------------------------------------------------
// <copyright file="ReportHelperRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Repository.Impl
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.ReportingService.Core.Criteria;
    using Fanex.AccountingReports.ReportingService.Core.Repository.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportHelperRepository : IReportHelperRepository
    {
        /// <summary>
        /// Gets the currency by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currency by licensee.</returns>
        public List<dynamic> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        ExchangeID = row["ExchangeID"],
                        Currency = row["Currency"]
                    });
                }
            }

            return report;
        }
    }
}