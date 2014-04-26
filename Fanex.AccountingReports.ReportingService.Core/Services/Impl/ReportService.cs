// --------------------------------------------------------------------------
// <copyright file="ReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Core.Constants;
    using Fanex.AccountingReports.ReportingService.Core.Criteria;
    using Fanex.AccountingReports.ReportingService.Core.Models;
    using Fanex.AccountingReports.ReportingService.Core.Repository.Impl;
    using Fanex.AccountingReports.ReportingService.Core.Repository.Interface;
    using Fanex.AccountingReports.ReportingService.Core.Resources;
    using Fanex.Common;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ReportService
    {
        #region StaticFields

        public const string CacheExchangeCurrencies = "Fanex.AccountingReports.Core.ReportService.ExchangeCurrencies";
        public const string BetTrans14SPSuffix = "_14";
        public const string WarehouseSPSuffix = "_WH";
        private static readonly IDynamicRepository BalanceUpRepository;

        private static readonly IReportHelperRepository ReportHelperRepository;

        #endregion StaticFields

        static ReportService()
        {
            BalanceUpRepository = new DynamicRepository();
            ReportHelperRepository = new ReportHelperRepository();
        }

        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                var criteria = new BalanceUpCriteria();
                var balanceUp = BalanceUpRepository.Get<BalanceUp>(criteria);
                return balanceUp;
            }
        }

        public static Dictionary<int, string> ExchangeCurrencies
        {
            get
            {
                var exchangeCurrencies = GetExchangeCurrencies(new GetExchangeCurrenciesCriteria
                    {
                        ExcludeTest = false
                    });
                return exchangeCurrencies;
            }
        }

        #endregion StaticProperties

        #region StaticMethods

        /// <summary>
        /// Gets the store mapping suffix.
        /// </summary>
        /// <param name="reportDate">The report date.</param>
        /// <returns>The store mapping suffix.</returns>
        public static string GetStoreMappingSuffix(DateTime reportDate)
        {
            if (reportDate > BalanceUp.MoveBetsDay)
            {
                return string.Empty;
            }
            else if (reportDate >= BalanceUp.BalanceUpDay)
            {
                return BetTrans14SPSuffix;
            }
            else
            {
                return WarehouseSPSuffix;
            }
        }

        /// <summary>
        /// Updates the latest exchange currencies.
        /// </summary>
        public static void UpdateLatestExchangeCurrencies()
        {
            var exchangeCurrencies = GetExchangeCurrencies(new GetExchangeCurrenciesCriteria());
        }

        /// <summary>
        /// Calculates the margin.
        /// </summary>
        /// <param name="lineTotal">The line total.</param>
        /// <param name="turnover">The turnover.</param>
        /// <returns>The margin.</returns>
        public static decimal CalculateMargin(decimal lineTotal, decimal turnover)
        {
            decimal result = 0;
            if (turnover == 0)
            {
                return 0;
            }

            result = lineTotal / turnover;
            return result;
        }

        /// <summary>
        /// Gets the currency by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currency by licensee.</returns>
        public static List<EntityBase<int, string>> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            var currencyList = new List<EntityBase<int, string>>();
            try
            {
                var rawData = ReportHelperRepository.GetCurrencyByLicensee(criteria);
                EntityBase<int, string> info = new EntityBase<int, string>
                {
                    Id = CommonConstants.CurrencyIdBaseRM,
                    Name = Labels.BaseRM
                };

                currencyList.Add(info);

                foreach (var data in rawData)
                {
                    if (IsExcludeCurrency(data.Currency))
                    {
                        continue;
                    }

                    info = new EntityBase<int, string>
                    {
                        Id = Converter.ToInt(data.ExchangeID),
                        Name = Converter.ToString2(data.Currency)
                    };

                    currencyList.Add(info);
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return currencyList;
        }

        /// <summary>
        /// Gets the exchange currencies.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The exchange currencies.</returns>
        public static Dictionary<int, string> GetExchangeCurrencies(GetExchangeCurrenciesCriteria criteria)
        {
            var exchangeCurrencies = new Dictionary<int, string>();
            try
            {
                var rawData = (new ExchangeCurrencyRepository()).GetExchangeCurrencies(criteria);
                if (rawData != null)
                {
                    rawData.ForEach(currency =>
                    {
                        if (!exchangeCurrencies.ContainsKey(currency.CurrencyId))
                        {
                            exchangeCurrencies.Add(currency.CurrencyId, currency.CurrencyName);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return exchangeCurrencies;
        }

        public static bool IsExcludeCurrency(string currencyName)
        {
            return string.Equals(currencyName, "C") ||
                string.Equals(currencyName, "H");
        }

        #endregion StaticMethods
    }
}