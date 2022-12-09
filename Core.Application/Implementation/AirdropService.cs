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
    public class AirdropService : IAirdropService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAirdropRepository _airdropRepository;
        private readonly IWalletTransactionService _walletTransactionService;

        public AirdropService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IAirdropRepository airdropRepository,
            IWalletTransactionService walletTransactionService
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _airdropRepository = airdropRepository;
            _walletTransactionService = walletTransactionService;
        }

        public PagedResult<AirdropViewModel> GetAllPaging(
            string keyword, string username, int pageIndex, int pageSize)
        {
            var query = _airdropRepository.FindAll(x => x.AppUser);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.AppUser.Email.Contains(keyword)
                || x.AppUser.Sponsor.Contains(keyword)
                || x.UserFacebook.Contains(keyword)
                || x.UserTelegramChannel.Contains(keyword)
                || x.UserTelegramCommunity.Contains(keyword)
                );

            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(x => x.AppUser.UserName == username);

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new AirdropViewModel()
                {
                    Id = x.Id,
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = x.AppUser.Sponsor,
                    Status = x.Status,
                    StatusName = x.Status.GetDescription(),
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    UserFacebook = x.UserFacebook,
                    UserTelegramChannel = x.UserTelegramChannel,
                    UserTelegramCommunity = x.UserTelegramCommunity
                }).ToList();

            return new PagedResult<AirdropViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public bool IsExistAirdrop(Guid userId)
        {
            var airdrops = _airdropRepository.FindAll(x => x.AppUserId == userId);
            
            return airdrops.Any();
        }

        public int Add(AirdropViewModel model)
        {
            var airdrop = new Airdrop()
            {
                AppUserId = model.AppUserId,
                UserFacebook = model.UserFacebook,
                UserTelegramChannel = model.UserTelegramChannel,
                UserTelegramCommunity = model.UserTelegramCommunity,
                Status = model.Status,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _airdropRepository.Add(airdrop);

            _unitOfWork.Commit();

            return airdrop.Id;
        }

        public void Rejected(int id)
        {
            var airdrop = _airdropRepository.FindById(id);

            airdrop.Status = AirdropStatus.Rejected;
            airdrop.DateUpdated = DateTime.UtcNow;

            _airdropRepository.Update(airdrop);

            _unitOfWork.Commit();
        }

        public async Task<GenericResult> Approved(int id)
        {
            var airdrop = _airdropRepository.FindById(id);

            airdrop.Status = AirdropStatus.Approved;
            airdrop.DateUpdated = DateTime.UtcNow;

            _airdropRepository.Update(airdrop);

            _unitOfWork.Commit();


            var appUser = await _userManager.FindByIdAsync(airdrop.AppUserId.ToString());


            decimal airdropReceive = 0;

            if (!string.IsNullOrWhiteSpace(airdrop.UserTelegramChannel))
                airdropReceive += 3;

            if (!string.IsNullOrWhiteSpace(airdrop.UserTelegramCommunity))
                airdropReceive += 3;

            if (!string.IsNullOrWhiteSpace(airdrop.UserFacebook))
                airdropReceive += 4;


            if (airdropReceive > 0)
            {
                appUser.USDTAmount += airdropReceive;

                var updateUserAirdrop = await _userManager.UpdateAsync(appUser);

                if (updateUserAirdrop.Succeeded)
                {
                    _walletTransactionService.Add(
                        new WalletTransactionViewModel
                        {
                            AppUserId = airdrop.AppUserId,
                            AddressFrom = "Airdrop",
                            AddressTo = "Wallet USDT",
                            Amount = airdropReceive,
                            FeeAmount = 0,
                            Fee = 0,
                            AmountReceive = airdropReceive,
                            TransactionHash = Guid.NewGuid().ToString(),
                            Type = WalletTransactionType.Airdrop,
                            DateCreated = DateTime.UtcNow,
                            Unit = Unit.USDT,
                            Remarks = "Receive by event Airdrop"
                        });

                    _walletTransactionService.Save();
                }
                else
                {
                    return new GenericResult(false,
                       string.Join(",", updateUserAirdrop.Errors.Select(x => x.Description)));
                }
            }

            return new GenericResult(true,
                $"{appUser.Email} received {airdropReceive} USDT. Approve airdrop is Success.");
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
