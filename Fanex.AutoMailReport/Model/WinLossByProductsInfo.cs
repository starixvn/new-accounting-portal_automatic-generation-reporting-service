// -----------------------------------------------------------------------
// <copyright file="WinLossByProductsInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace WinLossByProducts.Model
{
    using System;
    using System.Collections.Generic;
    using WinLossByProducts.Extensions;
    using WinLossByProducts.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByProductsInfo
    {
        public WinLossByProductsInfo()
        {
            Sportbook = new List<WinLossByProductsInfo>();
            ThirdPartySport = new List<WinLossByProductsInfo>();
        }

        public string Date
        {
            get
            {
                return string.Format(WinLossByProductsResource.WinLossByProducts_ReportTitle, DateTime.Now.AddDays(-1).ToShortFormatedDate());
            }
        }

        public string GroupName { get; set; }

        public string ProductName { get; set; }

        public decimal WinLoss { get; set; }

        public decimal MonthToDateWinLoss { get; set; }

        public bool IsEnoughData { get; set; }

        public List<WinLossByProductsInfo> Sportbook { get; set; }

        public List<WinLossByProductsInfo> ThirdPartySport { get; set; }

        public WinLossByProductsInfo SportbookTotal { get; set; }

        public WinLossByProductsInfo Total { get; set; }

        public void CalculateTotal(WinLossByProductsInfo winLossByProductsInfo)
        {
            this.WinLoss += winLossByProductsInfo.WinLoss;
            this.MonthToDateWinLoss += winLossByProductsInfo.MonthToDateWinLoss;
        }
    }
}