// -----------------------------------------------------------------------
// <copyright file="DailyTurnover.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Model.DailyTurnOver
{
    /// <summary>
    /// Daily turn over model.
    /// </summary>
    public class DailyTurnover
    {
        public DailyTurnover()
        {
            Turnover = new TurnoverInformation();
        }

        public string Date { get; set; }

        public TurnoverInformation Turnover { get; set; }

        public string TicketNormalRejectCount { get; set; }
    }
}