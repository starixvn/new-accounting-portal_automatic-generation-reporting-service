// -----------------------------------------------------------------------
// <copyright file="ReportingServiceRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.ReportingService.Repositories.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.ReportingService.Enums;
    using Fanex.AccountingReports.ReportingService.Model;
    using Fanex.AccountingReports.ReportingService.Repositories.Interface;
    using Fanex.AccountingReports.SqLiteDatabase;
    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportingServiceRepository : IReportingServiceRepository
    {
        private SqLite _db = null;

        public ReportingServiceRepository()
        {
            _db = new SqLite(AppSettings.ConnectionString);
        }

        /// <summary>
        /// Inserts the task queue.
        /// </summary>
        /// <param name="type">The type of task.</param>
        /// <returns>Identity of task.</returns>
        public int InsertTaskQueue(TaskType type)
        {
            int id = 0;

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Status", ((int)Status.Running).ToString());
            data.Add("CreatedDate", DateTime.Now.ToString());
            data.Add("Type", ((int)type).ToString());

            id = Convert.ToInt32(_db.Insert("TaskQueue", data));

            return id;
        }

        /// <summary>
        /// Determines whether this instance is running.
        /// </summary>
        /// <returns>Task is running or not.</returns>
        public bool IsRunning()
        {
            bool isRunning = false;

            var sql = "select * from TaskQueue where ID in( select max(ID) from TaskQueue)";
            var dataTable = _db.GetDataTable(sql);
            if (dataTable.Rows.Count > 0)
            {
                isRunning = dataTable.Rows[0]["Status"] == DBNull.Value ? 
                    false :
                    (Convert.ToInt32(dataTable.Rows[0]["Status"]) == Convert.ToInt32(Status.Running));
            }

            return isRunning;
        }

        /// <summary>
        /// Inserts the task queue details.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Identity of task detail.</returns>
        public int InsertTaskQueueDetails(TaskQueueDetail info)
        {
            int id = 0;

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("MainID", info.Id.ToString());
            data.Add("Status", ((int)Status.Running).ToString());
            data.Add("CreatedDate", info.CreatedDate);
            data.Add("SPName", info.SPName);
            data.Add("Retry", info.Retry.ToString());
            data.Add("Parameters", JsonConvert.SerializeObject(info.Parameters));
            data.Add("TaskName", info.TaskName);

            id = Convert.ToInt32(_db.Insert("TaskQueue_Detail", data));

            return id;
        }

        /// <summary>
        /// Updates the status task queue details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="status">The status.</param>
        public void UpdateStatusTaskQueueDetails(int id, Status status)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Status", ((int)status).ToString());
            string where = " ID = " + id;
            _db.Update("TaskQueue_Detail", data, where);
        }

        public void UpdateStatusTaskQueue(int id, Status status)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Status", ((int)status).ToString());
            string where = " Id = " + id;
            _db.Update("TaskQueue", data, where);
        }

        /// <summary>
        /// Gets the task details failed.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>List task failed.</returns>
        public List<TaskQueueDetail> GetTaskDetailsFailed(int id)
        {
            List<TaskQueueDetail> tasks = new List<TaskQueueDetail>();
            var sql = "select * from TaskQueue_Detail where MainId= " + id + " and Status = " + (int)Status.Failed;
            var dataTable = _db.GetDataTable(sql);

            var rows = dataTable.Rows;
            foreach (DataRow row in rows)
            {
                tasks.Add(new TaskQueueDetail()
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Status = Convert.ToInt32(row["Status"]),
                    CreatedDate = Convert.ToString(row["CreatedDate"]),
                    Retry = Convert.ToInt32(row["Retry"]),
                    SPName = Convert.ToString(row["SPName"]),
                    Parameters = JsonConvert.DeserializeObject<Hashtable>(Convert.ToString(row["Parameters"])),
                    TaskName = Convert.ToString(row["TaskName"])
                });
            }

            return tasks;
        }

        /// <summary>
        /// Updates the retry task queue details.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="retry">The retry.</param>
        public void UpdateRetryTaskQueueDetails(int id, int retry)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Retry", retry.ToString());
            string where = " Id = " + id;
            _db.Update("TaskQueue_Detail", data, where);
        }

        /// <summary>
        /// Determines whether [has task already run today].
        /// </summary>
        /// <returns>Has task to run or not.</returns>
        public bool HasTaskAlreadyRunToday()
        {
            bool already = false;

            var sql = "select * from TaskQueue where ID in( select max(ID) from TaskQueue) and Type = " + 
                ((int)TaskType.Daily).ToString() + " and Status= " + 
                ((int)Status.Completed).ToString();
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