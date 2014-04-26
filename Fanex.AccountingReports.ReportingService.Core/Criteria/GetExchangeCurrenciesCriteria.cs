// --------------------------------------------------------------------------
// <copyright file="GetExchangeCurrenciesCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.2">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetExchangeCurrenciesCriteria : CriteriaBase, ICriteria
    {
        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_ExchangeCurrencySel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
