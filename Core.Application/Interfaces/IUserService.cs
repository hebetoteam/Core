using Core.Application.ViewModels.System;
using Core.Application.ViewModels.Valuesshare;
using Core.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IUserService
    {
        #region Customer

        StatementUserViewModel GetStatementUser(string keyword, int type);

        PagedResult<AppUserViewModel> GetAllCustomerPagingAsync(string keyword, int page, int pageSize);

        Task<NetworkViewModel> GetNetworkInfo(string id);

        Task<NetworkViewModel> GetTotalNetworkInfo(string id);

        PagedResult<AppUserViewModel> GetCustomerReferralPagingAsync(string customerId, int refIndex, string keyword, int page, int pageSize);

        #endregion Customer

        #region User system
        PagedResult<AppUserViewModel> GetAllUser(string keyword);

        PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        List<AppUserTreeViewModel> GetTreeAll();

        Task<AppUserViewModel> GetById(string id);

        Task<AppUserViewModel> GetByUserName(string userName);

        Task<bool> AddAsync(AppUserViewModel userVm);

        Task UpdateAsync(AppUserViewModel userVm);

        Task DeleteAsync(string id);

        #endregion User system

        List<AppUserTreeViewModelAjax> GetMemberTreeAll(string parentId);

        AppUserViewModel GetUserByRefCode(string refCode);

        string GenerateReferralCode();

        Task<bool> AddbyImportAsync(string username, string password);

        PagedResult<AppUserViewModel> GetAllUserShowPagingAsync(string keyword, int page, int pageSize);

        List<AppUserTreeViewModelAjax> GetTreeAll(string parentId);



    }
}
