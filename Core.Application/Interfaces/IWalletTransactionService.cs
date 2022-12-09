using Core.Application.ViewModels.BlockChain;
using Core.Application.ViewModels.Report;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IWalletTransactionService
    {
        PagedResult<WalletTransactionViewModel> GetAllPaging(
            string keyword, Guid? userId, int pageIndex, int pageSize, int transactionId);

        void Add(WalletTransactionViewModel Model);

        Task LeaderShip(Guid appUserId, decimal stakingToken);

        void Save();

        void AddTransaction(Guid appUserId, decimal amount, decimal amountReceive,
            WalletTransactionType type, string addressFrom, string addressTo,
            Unit unit, decimal fee, decimal feeAmount,
            string transactionHash, string remarks = "");

        decimal GetTodayWithdraw(Guid appUserId, Unit unit);

        int GetTodayWithdrawTimes(Guid appUserId, Unit unit);

        bool IsStaked(Guid appUserId);

        IQueryable<WalletTransaction> GetAllByUserId(Guid appUserId);
    }
}
