// -----------------------------------------------------------------------
// <copyright file="SuperYesterdayBalance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Model.SuperYesterdayBalance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Super yesterday balance model.
    /// </summary>
    public class SuperYesterdayBalance
    {
        public string CustId { get; set; }

        public string Username { get; set; }

        public string Currency { get; set; }

        public decimal Balance { get; set; }

        public decimal BaseBalance { get; set; }

        public bool HasData { get; set; }

        public List<SuperYesterdayBalance> SuperBalanceList { get; set; }

        public SuperYesterdayBalance BaseBalanceTotal { get; set; }

        public static SuperYesterdayBalance FormatInfo(SuperYesterdayBalance balanceInfo)
        {
            if (balanceInfo == null)
            {
                return balanceInfo;
            }

            balanceInfo.Balance = Math.Round(balanceInfo.Balance, 2, MidpointRounding.AwayFromZero);
            balanceInfo.BaseBalance = Math.Round(balanceInfo.BaseBalance, 2, MidpointRounding.AwayFromZero);
            return balanceInfo;
        }

        public void AddToBaseBalance(SuperYesterdayBalance superBalance)
        {
            if (superBalance == null)
            {
                return;
            }

            this.BaseBalance += superBalance.BaseBalance;
        }
    }
}
