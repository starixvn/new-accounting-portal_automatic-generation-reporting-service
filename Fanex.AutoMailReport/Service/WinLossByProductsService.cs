// -----------------------------------------------------------------------
// <copyright file="WinLossByProductsService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.21">Created</change>
// </history>

namespace WinLossByProducts.Service
{
    using System;
    using System.Data;
    using WinLossByProducts.Common;
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Enums;
    using WinLossByProducts.Model;
    using WinLossByProducts.Repositories.Implement;
    using WinLossByProducts.Repositories.Interface;
    using WinLossByProducts.Service.Implement;
    using WinLossByProducts.Service.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByProductsService
    {
        private IWinLossByProductsRepository _repository = null;
        private IWinLossByProductsExportDataService _winLossByProductsExportDataService = null;

        public WinLossByProductsService()
        {
            _repository = new WinLossByProductsRepository();
            _winLossByProductsExportDataService = new WinLossByProductsExportDataService();
        }

        public void OnRunningDaily()
        {
            ProcessBusinessDaily();
        }

        public void OnRunningUrgently()
        {
            ProcessBusinessUrgently();
        }

        /// <summary>
        /// Processes the business urgently.
        /// </summary>
        private void ProcessBusinessUrgently()
        {
            if (!AppSettings.OnProcessing)
            {
                try
                {
                    AppSettings.OnProcessing = true;
                    
                    GetWinLossByProductsCriteria criteria = new GetWinLossByProductsCriteria
                    {
                        WinLossDate = AppSettings.WinlostDateForceRunning
                    };

                    bool isEnoughData = ProcessDataToSendMail(criteria);
                }
                catch (Exception ex)
                {
                    LogExtensions.ExceptionHandler(ex);
                }

                AppSettings.OnProcessing = false;
            }
        }

        /// <summary>
        /// Processes the business daily.
        /// </summary>
        /// <returns>Identify of task.</returns>
        private int ProcessBusinessDaily()
        {
            int id = 0;

            if (!AppSettings.OnProcessing)
            {
                try
                {
                    AppSettings.OnProcessing = true;

                    if (_repository.HasTaskAlreadyRunToday())
                    {
                        CheckNotCompleteTask();
                    }
                    else
                    {
                        id = _repository.Insert(TaskType.Daily);
                        GetWinLossByProductsCriteria criteria = new GetWinLossByProductsCriteria
                        {
                            WinLossDate = DateTime.Now.AddDays(-1)
                        };

                        bool isEnoughData = ProcessDataToSendMail(criteria);
                        var status = isEnoughData ? Status.SentEmailSuccessful : Status.NotEnoughDataToSendEmail;
                        _repository.UpdateStatus(id, status);
                    }
                }
                catch (Exception ex)
                {
                    LogExtensions.ExceptionHandler(ex);
                }

                AppSettings.OnProcessing = false;
            }

            return id;
        }

        /// <summary>
        /// Checks the not complete task.
        /// </summary>
        private void CheckNotCompleteTask()
        {
            DataTable log = _repository.GetLogNotEnoughDataToSendEmail();
            if (log.Rows.Count > 0)
            {
                int id = Convert.ToInt32(log.Rows[0]["Id"]);
                DateTime updatedDate = Convert.ToDateTime(log.Rows[0]["UpdatedDate"]).AddMinutes(AppSettings.RunNextTimeInMinutes);

                var currentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                TimeSpan executeTime = new TimeSpan(updatedDate.Hour, updatedDate.Minute, 0);

                bool isEnoughData = false;
                int compareResult = TimeSpan.Compare(currentTime, executeTime);
                if (compareResult >= 0)
                {
                    GetWinLossByProductsCriteria criteria = new GetWinLossByProductsCriteria
                    {
                        WinLossDate = DateTime.Now.AddDays(-1)
                    };

                    isEnoughData = ProcessDataToSendMail(criteria);
                    var status = isEnoughData ? Status.SentEmailSuccessful : Status.NotEnoughDataToSendEmail;
                    _repository.UpdateStatus(id, status);
                }
            }
        }

        /// <summary>
        /// Processes the data to send mail.
        /// </summary>
        /// <param name="criteria">The get win loss by products criteria.</param>
        /// <returns>Data is enough to send email.</returns>
        private bool ProcessDataToSendMail(GetWinLossByProductsCriteria criteria)
        {
            bool isEnoughData = false;
            try
            {
                WinLossByProductsInfo winLossByProduct = _repository.GetWinLossByProductsData(criteria);
                if (winLossByProduct.IsEnoughData)
                {
                    ////1. Export excel
                    string fileAttachment = _winLossByProductsExportDataService.ExportReportToExcel(winLossByProduct, criteria);

                    ////2. Send email and attach excel file
                    Mailing.MailingWinLossByProducts(winLossByProduct, fileAttachment, criteria);
                    isEnoughData = true;
                }
                else
                {
                    Mailing.WinLossByProductEmailNotification(criteria);
                }
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                isEnoughData = false;
            }

            return isEnoughData;
        }
    }
}