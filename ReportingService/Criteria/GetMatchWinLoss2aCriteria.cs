// -----------------------------------------------------------------------
// <copyright file="GetMatchWinLoss2aCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchWinLoss2aCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_MatchWinLoss2_New";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
