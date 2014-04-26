// -----------------------------------------------------------------------
// <copyright file="GetWinLossByProductsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace WinLossByProducts.Criteria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get win loss by products criteria.
    /// </summary>
    public class GetWinLossByProductsCriteria : CriteriaBase
    {
        public DateTime WinLossDate
        {
            get;
            set;
        }

        public override string GetSettingKey()
        {
            return "Win_Loss_By_Products_Report";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
