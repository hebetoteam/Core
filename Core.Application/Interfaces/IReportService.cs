using Core.Application.ViewModels.Report;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface IReportService
    {
        ReportViewModel GetReportInfo(string startDate, string endDate);

        Task<List<ReferralViewModel>> GetAllUserReferralAsync(Guid appUserId);

        List<Guid> GetAllBelowRef(Guid appUserId);

        decimal CalculateTotalStakingAmount(List<Guid> userIds);
    }
}
