// -----------------------------------------------------------------------
// <copyright file="IWinLossByProductsRepository.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WinLossByProducts.Repositories.Interface
{
    using System.Data;
    using WinLossByProducts.Criteria;
    using WinLossByProducts.Enums;
    using WinLossByProducts.Model;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IWinLossByProductsRepository
    {
        int Insert(TaskType type);

        void UpdateStatus(int id, Status status);

        DataTable GetLogNotEnoughDataToSendEmail();

        WinLossByProductsInfo GetWinLossByProductsData(GetWinLossByProductsCriteria criteria);

        bool HasTaskAlreadyRunToday();
    }
}