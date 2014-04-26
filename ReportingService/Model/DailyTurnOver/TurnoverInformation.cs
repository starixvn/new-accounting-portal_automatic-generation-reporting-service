// -----------------------------------------------------------------------
// <copyright file="TurnoverInformation.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Model.DailyTurnOver
{
    using System;

    /// <summary>
    /// Turn over information.
    /// </summary>
    [Serializable]
    public class TurnoverInformation
    {
        public decimal TotalTurnover { get; set; }

        public decimal SuperTurnover { get; set; }

        public decimal MasterTurnover { get; set; }

        public decimal AgentTurnover { get; set; }

        public decimal IBCTurnover { get; set; }

        public bool IsActual { get; set; }
    }
}