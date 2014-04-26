// -----------------------------------------------------------------------
// <copyright file="SuperYesterdayBalanceRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Repositories.Implement.SuperYesterdayBalance
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Super yesterday balance repository.
    /// </summary>
    public class SuperYesterdayBalanceRepository : ISuperYesterdayBalanceRepository
    {
        /// <summary>
        /// Gets the super yesterday balance.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>Super yesterday balance data.</returns>
        public List<dynamic> GetSuperYesterdayBalance(GetSuperYesterdayBalanceCriteria criteria, ref Hashtable outputParam)
        {
            DataSet result = null;

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            var superBalance = new List<dynamic>();
            if (criteria.IsViewTotalOnly)
            {
                superBalance.Add(outputParam["total"]);
                return superBalance;
            }

            if (result == null || result.Tables.Count == 0)
            {
                return null;
            }

            var currencies = ReportService.ExchangeCurrencies;
            var superBalanceTable = result.Tables[0];
            foreach (DataRow row in superBalanceTable.Rows)
            {
                var currencyId = Converter.ToInt(row["currency"]);

                superBalance.Add(new
                {
                    CustId = Converter.ToString2(row["custid"]),
                    Username = row["username"],
                    Currency = currencies[currencyId],
                    Balance = Converter.ToDecimal(row["Balance"]),
                    BaseBalance = Converter.ToDecimal(row["BaseBalance"])
                });
            }

            return superBalance;
        }
    }
}