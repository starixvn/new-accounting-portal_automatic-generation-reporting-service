// -----------------------------------------------------------------------
// <copyright file="WinLossByProductsRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinLossByProducts.Repositories.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.SqLiteDatabase;
    using Fanex.Common;
    using Fanex.Data;
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Enums;
    using WinLossByProducts.Model;
    using WinLossByProducts.Repositories.Interface;
    using WinLossByProducts.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByProductsRepository : IWinLossByProductsRepository
    {
        private SqLite _db = null;

        public WinLossByProductsRepository()
        {
            _db = new SqLite(AppSettings.ConnectionString);
        }

        /// <summary>
        /// Inserts the specified type.
        /// </summary>
        /// <param name="type">The type of task.</param>
        /// <returns>Identify of task.</returns>
        public int Insert(TaskType type)
        {
            int id = 0;
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("Status", ((int)Status.Running).ToString());
                data.Add("CreatedDate", DateTime.Now.ToString());
                data.Add("Type", ((int)type).ToString());

                id = Convert.ToInt32(_db.Insert("Log", data));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return id;
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="status">The status.</param>
        public void UpdateStatus(int id, Status status)
        {
            try
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("Status", ((int)status).ToString());
                data.Add("UpdatedDate", DateTime.Now.ToString());

                string where = " Id = " + id;
                _db.Update("Log", data, where);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the log not enough data to send email.
        /// </summary>
        /// <returns>Log data table.</returns>
        public DataTable GetLogNotEnoughDataToSendEmail()
        {
            DataTable log = new DataTable();
            try
            {
                var sql = "select * from Log where ID in( select max(ID) from Log) and Status = " + ((int)Status.NotEnoughDataToSendEmail).ToString();
                log = _db.GetDataTable(sql);
                if (log.Rows.Count > 0)
                {
                    //// Just get today log
                    if (Convert.ToDateTime(log.Rows[0]["CreatedDate"]).ToShortDateString() != DateTime.Today.ToShortDateString())
                    {
                        log = new DataTable();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return log;
        }

        /// <summary>
        /// Gets the win loss by products data.
        /// </summary>
        /// <param name="criteria">The get win loss by products criteria.</param>
        /// <returns>
        /// Win loss products data.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public WinLossByProductsInfo GetWinLossByProductsData(GetWinLossByProductsCriteria criteria)
        {
            WinLossByProductsInfo winLossByProduct = new WinLossByProductsInfo
                {
                    SportbookTotal = new WinLossByProductsInfo
                    {
                        ProductName = WinLossByProductsLabel.Sportsbook_Total
                    },
                    Total = new WinLossByProductsInfo
                    {
                        ProductName = WinLossByProductsLabel.Total
                    }
                };

            IDataReader reader = null;

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hashTable = new Hashtable();
                hashTable.Add("allowsendemail", 0);
                objectDb.AssignParamValues(hashTable);

                reader = objectDb.ExecuteReader();
                while (reader.Read())
                {
                    var winLossByProductInfo = new WinLossByProductsInfo
                    {
                        GroupName = Converter.ToString2(reader["group"]),
                        ProductName = Converter.ToString2(reader["product"]),
                        WinLoss = Converter.ToDecimal(reader["winlostyesterday"]),
                        MonthToDateWinLoss = Converter.ToDecimal(reader["winlostmonthtodate"])
                    };

                    if (winLossByProductInfo.GroupName.ToLower() == "sb")
                    {
                        winLossByProduct.Sportbook.Add(winLossByProductInfo);
                        winLossByProduct.SportbookTotal.CalculateTotal(winLossByProductInfo);
                        winLossByProduct.Total.CalculateTotal(winLossByProductInfo);
                    }
                    else
                    {
                        if (winLossByProductInfo.GroupName.ToLower() == "3rd")
                        {
                            winLossByProduct.Total.CalculateTotal(winLossByProductInfo);
                        }

                        winLossByProduct.ThirdPartySport.Add(winLossByProductInfo);
                    }
                }

                reader.Close();
                var outputParams = objectDb.GetOuputParams();
                winLossByProduct.IsEnoughData = Converter.ToBool(outputParams["allowsendemail"]);
            }

            return winLossByProduct;
        }

        /// <summary>
        /// Determines whether [has task already run today].
        /// </summary>
        /// <returns>Task has already.</returns>
        public bool HasTaskAlreadyRunToday()
        {
            bool already = false;

            var sql = "select * from Log where ID in( select max(ID) from Log) and Type = " + ((int)TaskType.Daily).ToString();
            var dataTable = _db.GetDataTable(sql);
            if (dataTable.Rows.Count > 0)
            {
                already = dataTable.Rows[0]["CreatedDate"] == DBNull.Value ?
                    false :
                    (Convert.ToDateTime(dataTable.Rows[0]["CreatedDate"]).ToShortDateString() == DateTime.Today.ToShortDateString());
            }

            return already;
        }
    }
}