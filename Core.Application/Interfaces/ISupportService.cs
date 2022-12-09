using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Utilities.Dtos;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface ISupportService
    {
        PagedResult<SupportViewModel> GetAllPaging(string keyword, string appUserId, int pageIndex, int pageSize);

        SupportViewModel GetById(int id);

        void Add(SupportViewModel support);

        void Update(SupportViewModel support);

        void Delete(int id);

        void Save();
    }
}
