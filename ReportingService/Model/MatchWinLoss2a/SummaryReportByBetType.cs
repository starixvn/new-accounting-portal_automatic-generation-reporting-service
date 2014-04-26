// -----------------------------------------------------------------------
// <copyright file="SummaryReportByBetType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.18">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Model.MatchWinLoss2a
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Summary report by bet type model.
    /// </summary>
    [Serializable]
    public class SummaryReportByBetType
    {
        public string Name { get; set; }

        public string Name_Suffix { get; set; }

        public int Id { get; set; }

        [ScriptIgnore]
        public decimal LineTotal_Soccer_Live { get; set; }

        [ScriptIgnore]
        public decimal VirtualLineTotal_Soccer_Live { get; set; }

        [ScriptIgnore]
        public decimal LineTotal_Soccer_Non_Live { get; set; }

        [ScriptIgnore]
        public decimal VirtualLineTotal_Soccer_Non_Live { get; set; }

        [ScriptIgnore]
        public decimal LineTotal_Others_Live { get; set; }

        [ScriptIgnore]
        public decimal VirtualLineTotal_Others_Live { get; set; }

        [ScriptIgnore]
        public decimal LineTotal_Others_Non_Live { get; set; }

        [ScriptIgnore]
        public decimal VirtualLineTotal_Others_Non_Live { get; set; }

        [ScriptIgnore]
        public decimal LineTotalReal { get; set; }

        [ScriptIgnore]
        public decimal LineTotalVirtual { get; set; }

        [ScriptIgnore]
        public decimal Variance { get; set; }

        [ScriptIgnore]
        public string Hedge { get; set; }

        [ScriptIgnore]
        public decimal Turnover { get; set; }

        [ScriptIgnore]
        public decimal Margin { get; set; }

        public List<SummaryReportByBetType> ChildsInfo { get; set; }

        public SummaryReportByBetType GrandTotal { get; set; }

        //// FieldToDisplay

        public string LineTotal_Soccer_Live_Str { get; set; }

        public string VirtualLineTotal_Soccer_Live_Str { get; set; }

        public string LineTotal_Soccer_Non_Live_Str { get; set; }

        public string VirtualLineTotal_Soccer_Non_Live_Str { get; set; }

        public string LineTotal_Others_Live_Str { get; set; }

        public string VirtualLineTotal_Others_Live_Str { get; set; }

        public string LineTotal_Others_Non_Live_Str { get; set; }

        public string VirtualLineTotal_Others_Non_Live_Str { get; set; }

        public string LineTotalReal_Str { get; set; }

        public string LineTotalVirtual_Str { get; set; }

        public string Variance_Str { get; set; }

        public string Turnover_Str { get; set; }

        public string Margin_Str { get; set; }

        //// FieldToDisplay
    }
}
