// -----------------------------------------------------------------------
// <copyright file="GetAllDetailReportBySportsCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetAllDetailReportBySportsCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public DateTime Fromdate
        {
            get
            {
                return this.Winlostdate;
            }
        }

        public DateTime Todate
        {
            get
            {
                return this.Winlostdate;
            }
        }

        public decimal WinLost_8RM { get; set; }

        public decimal WinLost_BARM { get; set; }

        public decimal IBCTurnover_8RM { get; set; }

        public decimal IBCTurnover_BARM { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetAllDetailReportBySportsType" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}