// -----------------------------------------------------------------------
// <copyright file="Mailing.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinLossByProducts.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using RazorEngine;
    using WinLossByProducts.Common;
    using WinLossByProducts.Constants;
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Extensions;
    using WinLossByProducts.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Mailing
    {
        /// <summary>
        /// Mailings the win loss by products.
        /// </summary>
        /// <param name="winLossByProducts">The win loss by products.</param>
        /// <param name="fileAttachment">The file attachment.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Email will be send success or not.</returns>
        public static bool MailingWinLossByProducts(WinLossByProductsInfo winLossByProducts, string fileAttachment, GetWinLossByProductsCriteria criteria)
        {
            bool isSuccess = true;
            try
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = root + EmailTemplateConstants.WinLossByProductsTemplate;
                string template = File.ReadAllText(filePath);
                var emailContent = Razor.Parse(template, winLossByProducts);

                string emailTo = AppSettings.MailTo;
                string emailSubject = string.Format(AppSettings.MailSubject, criteria.WinLossDate.ToShortFormatedDate());
                string emailTitle = AppSettings.MailTitle;
                string emailCcTo = AppSettings.MailCcTo;
                string emailBCcTo = AppSettings.MailBCcTo;

                var mailItem = new Fanex.MailDeliver.Model.MailRecord
                {
                    MailTo = emailTo,
                    MailCc = emailCcTo,
                    MailBcc = emailBCcTo,
                    Subject = emailSubject,
                    Title = emailTitle,
                    Body = emailContent
                };

                mailItem.Attachments = new List<Fanex.MailDeliver.Model.MailAttachment>();
                var attachmentItem = new Fanex.MailDeliver.Model.MailAttachment(@fileAttachment, Fanex.MailDeliver.Model.FileExtentions.MS_EXCEL);
                mailItem.Attachments.Add(attachmentItem);

                if (!string.IsNullOrEmpty(mailItem.MailTo.Trim()))
                {
                    using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
                    {
                        client.SendMail(mailItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Wins the loss by product email notification.
        /// </summary>
        /// <param name="criteria">The win loss by products criteria.</param>
        /// <returns>Email will be send success or not.</returns>
        public static bool WinLossByProductEmailNotification(GetWinLossByProductsCriteria criteria)
        {
            bool isSuccess = true;
            try
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = root + EmailTemplateConstants.WinLossByProductsNotificationTemplate;
                string emailContent = File.ReadAllText(filePath);
                string emailTo = AppSettings.AdminEmail;
                string emailSubject = string.Format(AppSettings.MailSubject, criteria.WinLossDate.ToShortFormatedDateWithHourMinute());
                string emailTitle = AppSettings.MailTitle;

                var mailItem = new Fanex.MailDeliver.Model.MailRecord
                {
                    MailTo = emailTo,
                    Subject = emailSubject,
                    Title = emailTitle,
                    Body = emailContent
                };

                if (!string.IsNullOrEmpty(mailItem.MailTo.Trim()))
                {
                    using (var client = new Fanex.MailDeliver.MailClient.API.MailDeliver())
                    {
                        client.SendMail(mailItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}