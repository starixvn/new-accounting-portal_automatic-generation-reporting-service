// -----------------------------------------------------------------------
// <copyright file="GetDetailReportByBetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.ReportingService.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDetailReportByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public string Sporttype { get; set; }

        public int Bettype { get; set; }

        public string BetTypeName { get; set; }

        public bool LiveIndicator { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetWinLossDetailReportByBetType" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}