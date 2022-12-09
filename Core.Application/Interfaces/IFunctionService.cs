using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IFunctionService : IDisposable
    {
        FunctionViewModel Add(FunctionViewModel functionVM);

        void Delete(string id);

        List<FunctionViewModel> GetAll();

        List<FunctionTreeViewModel> GetTreeAll(Status isBackEnd = Status.InActive);

        List<Function> GetAllFunctionToSetPermission();

        FunctionViewModel GetById(string id);

        void Update(FunctionViewModel function);

        void ReOrder(string sourceId, string targetId);

        void Save();

        void UpdateParentId(string id, string parentId);
    }
}
