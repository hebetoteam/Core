using Core.Application.ViewModels.System;
using Core.Data.Enums;
using Core.Utilities.Dtos;
using System.Collections.Generic;

namespace Core.Application.Interfaces
{
    public interface IWalletTransferService
    {
        PagedResult<WalletTransferViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize);

        void Add(WalletTransferViewModel Model);

        void Save();

        decimal GetTotalTransactionAmount();
    }
}
