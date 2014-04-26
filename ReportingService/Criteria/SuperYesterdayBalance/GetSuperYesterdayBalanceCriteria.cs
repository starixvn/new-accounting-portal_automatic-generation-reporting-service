// -----------------------------------------------------------------------
// <copyright file="GetSuperYesterdayBalanceCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.08">Created</change>
// </history>

namespace Fanex.AccountingReports.ReportingService.Criteria.SuperYesterdayBalance
{
    using Fanex.AccountingReports.ReportingService.Enums;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get super yesterday balance criteria.
    /// </summary>
    public class GetSuperYesterdayBalanceCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int BalanceType
        {
            get
            {
                return (int)SuperBalance.Yesterday;
            }
        }

        [SpParam(SqlParamName = "Keyword")]
        public string Username
        {
            get
            {
                return string.Empty;
            }
        }

        [SpParam(SqlParamName = "FirstChar")]
        public string FirstLetter
        {
            get
            {
                return string.Empty;
            }
        }

        [SpParam(SqlParamName = "Currency")]
        public int? Currency
        {
            get
            {
                return null;
            }
        }

        [SpParam(SqlParamName = "IsViewTotalOnly")]
        public bool IsViewTotalOnly
        {
            get
            {
                return false;
            }
        }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>Setting key.</returns>
        public override string GetSettingKey()
        {
            return "Balance_SuperBalance_SuperYesterday";
        }
    }
}