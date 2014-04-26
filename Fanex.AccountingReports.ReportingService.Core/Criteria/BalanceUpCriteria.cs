// --------------------------------------------------------------------------
// <copyright file="BalanceUpCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Core.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// BalanceUpCriteria: criteria for getting balance up day and move bet day.
    /// </summary>
    public class BalanceUpCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Core_ReportService_GetBalanceUpDay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
