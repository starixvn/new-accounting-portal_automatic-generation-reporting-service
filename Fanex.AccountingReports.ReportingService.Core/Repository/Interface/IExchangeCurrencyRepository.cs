// --------------------------------------------------------------------------
// <copyright file="IExchangeCurrencyRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.2">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Repository.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.ReportingService.Core.Models;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IExchangeCurrencyRepository
    {
        List<Currency> GetExchangeCurrencies(ICriteria criteria);
    }
}