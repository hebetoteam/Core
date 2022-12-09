using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Implementation
{
    public class TicketTransactionService : ITicketTransactionService
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly ITicketTransactionRepository _ticketTransactionRepository;
        private readonly IBlockChainService _blockChainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfigService _configService;
        public TicketTransactionService(
            IWalletTransactionService walletTransactionService,
            UserManager<AppUser> userManager,
            IBlockChainService blockChainService,
            ITicketTransactionRepository ticketTransactionRepository,
            IUnitOfWork unitOfWork,
            IConfigService configService)
        {
            _configService = configService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _blockChainService = blockChainService;
            _walletTransactionService = walletTransactionService;
            _ticketTransactionRepository = ticketTransactionRepository;
        }

        public PagedResult<TicketTransactionViewModel> GetAllPaging(
            string keyword, string username, int pageIndex, int pageSize)
        {
            var query = _ticketTransactionRepository
                .FindAll(x => x.AppUser);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.AddressFrom.Contains(keyword)
                || x.AddressTo.Contains(keyword)
                || x.AppUser.Email.Contains(keyword)
                || x.AppUser.Sponsor.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(x => x.AppUser.UserName == username);

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new TicketTransactionViewModel()
                {
                    Id = x.Id,
                    AddressFrom = x.AddressFrom,
                    AddressTo = x.AddressTo,
                    Fee = x.Fee,
                    FeeAmount = x.FeeAmount,
                    AmountReceive = x.AmountReceive,
                    Amount = x.Amount,
                    StrAmount = x.Amount.ToString(),
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = x.AppUser.Sponsor,
                    Type = x.Type,
                    TypeName = x.Type.GetDescription(),
                    Status = x.Status,
                    StatusName = x.Status.GetDescription(),
                    UnitName = x.Unit.GetDescription(),
                    Unit = x.Unit,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated
                }).ToList();

            return new PagedResult<TicketTransactionViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public int Add(TicketTransactionViewModel model)
        {
            var transaction = new TicketTransaction()
            {
                AddressFrom = model.AddressFrom,
                AddressTo = model.AddressTo,
                Fee = model.Fee,
                FeeAmount = model.FeeAmount,
                AmountReceive = model.AmountReceive,
                Amount = model.Amount,
                AppUserId = model.AppUserId,
                Status = model.Status,
                Type = model.Type,
                Unit = model.Unit,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,

            };

            _ticketTransactionRepository.Add(transaction);

            _unitOfWork.Commit();

            return transaction.Id;
        }

        public void Rejected(int id)
        {
            //var ticket = _ticketTransactionRepository.FindById(id);

            //ticket.Status = TicketTransactionStatus.Rejected;
            //ticket.DateUpdated = DateTime.UtcNow;

            //_ticketTransactionRepository.Update(ticket);

            //_unitOfWork.Commit();

            //var wallet = _walletService.GetByTokenIdAndUserId(ticket.TokenConfigId, ticket.AppUserId);

            //if (wallet != null)
            //{
            //    wallet.Amount += ticket.Amount;
            //    _walletService.Update(wallet);

            //    _walletService.Save();
            //}
        }

        public async Task<GenericResult> Approved(int id)
        {
            var ticket = _ticketTransactionRepository.FindById(id);

            TransactionReceipt transaction = null;

            if (ticket.Unit == Unit.HBT)
            {
                var currentBalance = await _blockChainService.GetERC20Balance(
                        CommonConstants.TransferPuKey,
                        CommonConstants.TokenContract,
                        CommonConstants.TokenDecimals,
                        CommonConstants.Url);

                if (currentBalance < ticket.AmountReceive)
                    return new GenericResult(false, "Insufficient account balance");

                transaction = await _blockChainService.SendERC20Async(
                           CommonConstants.TransferPrKey,
                           ticket.AddressTo,
                           CommonConstants.TokenContract,
                           ticket.AmountReceive,
                           CommonConstants.TokenDecimals,
                           CommonConstants.Url);

                if (!transaction.Succeeded(true))
                    return new GenericResult(false, "transfer is fail");
            }
            else
            {
                var currentBalance = await _blockChainService.GetERC20Balance(
                    CommonConstants.TransferPuKey,
                    CommonConstants.USDTContract,
                    CommonConstants.USDTDecimals,
                    CommonConstants.Url);

                if (currentBalance < ticket.AmountReceive)
                    return new GenericResult(false, "Insufficient account balance");

                transaction = await _blockChainService.SendERC20Async(
                           CommonConstants.TransferPrKey,
                           ticket.AddressTo,
                           CommonConstants.USDTContract,
                           ticket.AmountReceive,
                           CommonConstants.USDTDecimals,
                           CommonConstants.Url);

                if (!transaction.Succeeded(true))
                    return new GenericResult(false, "transfer is fail");
            }


            _walletTransactionService.Add(
                new WalletTransactionViewModel
                {
                    AppUserId = ticket.AppUserId,
                    AddressFrom = CommonConstants.TransferPuKey,
                    AddressTo = ticket.AddressTo,
                    Amount = ticket.Amount,
                    FeeAmount = ticket.FeeAmount,
                    Fee = ticket.Fee,
                    AmountReceive = ticket.AmountReceive,
                    TransactionHash = transaction.TransactionHash,
                    Type = WalletTransactionType.Withdraw,
                    DateCreated = DateTime.UtcNow,
                    Unit = ticket.Unit,
                    Remarks = "WITHDRAW USING APPROVED"
                });
            _walletTransactionService.Save();


            ticket.Status = TicketTransactionStatus.Approved;
            ticket.DateUpdated = DateTime.UtcNow;
            _ticketTransactionRepository.Update(ticket);

            _unitOfWork.Commit();

            return new GenericResult(true, "Approve ticket is success", ticket);
        }

        public async Task<bool> IsPrimaryWalletEnoughBalanceAsync(int ticketTransactionId)
        {
            var ticket = _ticketTransactionRepository.FindById(ticketTransactionId);

            decimal balance = ticket.AmountReceive;

            if (ticket.Unit == Unit.HBT)
            {
                var currentBalance = await _blockChainService.GetERC20Balance(
                    CommonConstants.TransferPuKey,
                    CommonConstants.TokenContract,
                    CommonConstants.TokenDecimals,
                    CommonConstants.Url);

                if (currentBalance <= balance)
                    return false;
            }
            else
            {
                var currentBalance = await _blockChainService.GetERC20Balance(
                    CommonConstants.TransferPuKey,
                    CommonConstants.USDTContract,
                    CommonConstants.USDTDecimals,
                    CommonConstants.Url);

                if (currentBalance < balance)
                    return false;
            }

            return true;
        }

        public bool IsValidDailyWithdrawAmount(int ticketTransactionId)
        {
            var ticket = _ticketTransactionRepository.FindById(ticketTransactionId);

            var todayWithdraw = _walletTransactionService.GetTodayWithdraw(ticket.AppUserId, ticket.Unit);

            var totalWithdraw = ticket.Amount + todayWithdraw;

            decimal maxWithdraw = 0;

            if (ticket.Unit == Unit.HBT)
                maxWithdraw = CommonConstants.HBTMaxWithdraw;
            else
                maxWithdraw = CommonConstants.USDTMaxWithdraw;

            if (totalWithdraw >= maxWithdraw)
                return false;

            return true;
        }

        public bool IsValidDailyWithdrawTimes(int ticketTransactionId)
        {
            var ticket = _ticketTransactionRepository.FindById(ticketTransactionId);

            var todayTimeWithdraw = _walletTransactionService
                                        .GetTodayWithdrawTimes(ticket.AppUserId, ticket.Unit);

            if (todayTimeWithdraw >= CommonConstants.TimeWithdraw)
                return false;

            return true;
        }

        public bool IsAnyPendingWithdraw(int ticketTransactionId)
        {
            var ticket = _ticketTransactionRepository.FindById(ticketTransactionId);

            var query = _ticketTransactionRepository.FindAll(x => x.AppUserId == ticket.AppUserId);

            query = query.Where(x => x.Status == TicketTransactionStatus.Pending);

            query = query.Where(x => x.Type == TicketTransactionType.Withdraw);

            query = query.Where(x => x.Id != ticketTransactionId);

            return query.Any();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public decimal GetTotalPendingWithdrawUSDT(Guid userId)
        {
            var query = _ticketTransactionRepository.FindAll();

            query = query.Where(x => x.AppUserId == userId);

            query = query.Where(x => x.Type == TicketTransactionType.Withdraw);

            query = query.Where(x => x.Status == TicketTransactionStatus.Pending);

            return query.Sum(d => d.Amount);
        }

        public decimal GetTodayTotalCompletedWithdrawUSDT(Guid userId)
        {
            var query = _ticketTransactionRepository.FindAll();

            var todayEnd = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 11, 00, 00);
            if (DateTime.UtcNow.Hour >= 11) // passed day
                todayEnd = todayEnd.AddDays(1);
            var lastDayStart = todayEnd.AddDays(-1);

            query = query.Where(x => x.AppUserId == userId);

            query = query.Where(x => x.Type == TicketTransactionType.Withdraw);

            query = query.Where(x => x.Status == TicketTransactionStatus.Approved);

            query = query.Where(x => x.DateUpdated >= lastDayStart && x.DateUpdated <= todayEnd);

            return query.Sum(d => d.Amount);
        }
    }
}
