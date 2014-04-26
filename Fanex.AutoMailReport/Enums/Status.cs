// -----------------------------------------------------------------------
// <copyright file="Status.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinLossByProducts.Enums
{
    /// <summary>
    /// Email send status.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Task is running.
        /// </summary>
        Running,

        /// <summary>
        /// The not enough data to send email.
        /// </summary>
        NotEnoughDataToSendEmail,

        /// <summary>
        /// The email was sent successful.
        /// </summary>
        SentEmailSuccessful
    }
}