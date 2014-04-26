// -----------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
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
    /// Win loss information model.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Name { get; set; }

        public int Id { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal VirtualComm { get; set; }

        [ScriptIgnore]
        public decimal LineTotal { get; set; }

        [ScriptIgnore]
        public decimal VirtualLineTotal { get; set; }

        [ScriptIgnore]
        public decimal Variance { get; set; }

        [ScriptIgnore]
        public decimal WinLoss { get; set; }

        [ScriptIgnore]
        public decimal Position { get; set; }

        [ScriptIgnore]
        public decimal Turnover { get; set; }

        [ScriptIgnore]
        public decimal Margin { get; set; }

        public WinLossInfo ThirdPartyInfo { get; set; }

        public WinLossInfo NonThirdPartyInfo { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public WinLossInfo EightRMTotal { get; set; }

        public WinLossInfo BARMTotal { get; set; }

        public WinLossInfo Total { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        //// FieldToDisplay

        public string LeagueName { get; set; }

        public string MatchCode { get; set; }

        public string EventDate { get; set; }

        public string KickOffTime { get; set; }

        public bool LiveIndicatorId { get; set; }

        public string LiveIndicatorName { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; }

        public bool IsNeutral { get; set; }

        public string Currency { get; set; }

        public string Commission_Str { get; set; }

        public string VirtualComm_Str { get; set; }

        public string LineTotal_Str { get; set; }

        public string VirtualLineTotal_Str { get; set; }

        public string Variance_Str { get; set; }

        public string WinLoss_Str { get; set; }

        public string Position_Str { get; set; }

        public string Turnover_Str { get; set; }

        public string Margin_Str { get; set; }

        //// FieldToDisplay

        /// <summary>
        /// Adds to grand total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void AddToGrandTotal(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.GrandTotal.Turnover += winLoss.Turnover;
            this.GrandTotal.WinLoss += winLoss.WinLoss;
            this.GrandTotal.Position += winLoss.Position;
            this.GrandTotal.Commission += winLoss.Commission;
            this.GrandTotal.VirtualComm += winLoss.VirtualComm;
            this.GrandTotal.LineTotal += winLoss.LineTotal;
            this.GrandTotal.VirtualLineTotal += winLoss.VirtualLineTotal;
            this.GrandTotal.Variance += winLoss.Variance;
        }

        /// <summary>
        /// Adds the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        public void Add(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return;
            }

            this.Turnover += winLoss.Turnover;
            this.WinLoss += winLoss.WinLoss;
            this.Position += winLoss.Position;
            this.Commission += winLoss.Commission;
            this.VirtualComm += winLoss.VirtualComm;
            this.LineTotal += winLoss.LineTotal;
            this.VirtualLineTotal += winLoss.VirtualLineTotal;
            this.Variance += winLoss.Variance;
        }
    }
}
