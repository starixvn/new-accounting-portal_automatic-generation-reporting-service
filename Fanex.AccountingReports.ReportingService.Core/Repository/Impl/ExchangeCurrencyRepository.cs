// --------------------------------------------------------------------------
// <copyright file="ExchangeCurrencyRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Repository.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.ReportingService.Core.Models;
    using Fanex.AccountingReports.ReportingService.Core.Repository.Interface;
    using Fanex.Common.Extensions;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExchangeCurrencyRepository : BaseRepository<Currency>, IExchangeCurrencyRepository
    {
        /// <summary>
        /// Gets the exchange currencies.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The exchange currencies.</returns>
        public List<Currency> GetExchangeCurrencies(ICriteria criteria)
        {
            // Create Function will map Data from DataReader to Schedule Object
            Func<IDataReader, Currency> mapObjectFunc = (reader) =>
            {
                return new Currency
                {
                    CurrencyId = reader.ToInt("exchangeid"),
                    CurrencyName = reader.To<string>("currency")
                };
            };

            return Fetch<Currency>(criteria, mapObjectFunc);
        }
    }
}