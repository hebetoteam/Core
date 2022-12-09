using Core.Application.ViewModels.System;
using Core.Utilities.Dtos;
using System;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IAirdropService
    {
        PagedResult<AirdropViewModel> GetAllPaging(
            string keyword, string addressPubkey, int pageIndex, int pageSize);

        int Add(AirdropViewModel Model);

        bool IsExistAirdrop(Guid userId);

        void Rejected(int id);

        Task<GenericResult> Approved(int id);

        void Save();
    }
}
