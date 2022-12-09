using Core.Application.ViewModels.System;
using Core.Utilities.Dtos;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface INotifyService
    {
        PagedResult<NotifyViewModel> GetAllPaging(string startDate, string endDate, string keyword, int pageIndex, int pageSize);

        List<NotifyViewModel> GetLast(int top);

        NotifyViewModel Add(NotifyViewModel blog);

        void Update(NotifyViewModel blog);

        NotifyViewModel GetById(int id);

        NotifyViewModel GetDashboard();

        void Delete(int id);

        void Save();
    }
}
