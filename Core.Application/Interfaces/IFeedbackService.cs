using Core.Application.ViewModels.Common;
using Core.Utilities.Dtos;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface IFeedbackService
    {
        PagedResult<FeedbackViewModel> GetAllPaging(string keyword, int page, int pageSize);

        void Add(FeedbackViewModel feedbackVm);

        void UpdateType(int id, string modifiedBy);

        void Delete(int id);

        FeedbackViewModel GetById(int id);

        void SaveChanges();
    }
}
