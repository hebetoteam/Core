using Core.Application.Interfaces;
using Core.Application.ViewModels.Report;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Implementation
{
    public class ReportService : IReportService
    {
        private readonly ITicketTransactionRepository _ticketTransactionRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IStakingRepository _stakingRepository;
        private readonly IBlockChainService _blockChainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ReportService(
                             ITicketTransactionRepository ticketTransactionRepository,
                             IWalletTransactionRepository walletTransactionRepository,
                             IBlockChainService blockChainService,
                             IUnitOfWork unitOfWork,
                             UserManager<AppUser> userManager,
                             IStakingRepository stakingRepository)
        {
            _ticketTransactionRepository = ticketTransactionRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _blockChainService = blockChainService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _stakingRepository = stakingRepository; 
        }

        public ReportViewModel GetReportInfo(string startDate, string endDate)
        {
            DateTime now = DateTime.UtcNow.Date;

            var appUsers = _userManager.Users;

            var model = new ReportViewModel
            {
                TotalMember = appUsers.Count(),
                TodayMember = appUsers.Count(x => x.DateCreated.Date == now),
                TotalMemberInVerifyEmail = appUsers.Count(x => x.EmailConfirmed == false),
                TotalMemberVerifyEmail = appUsers.Count(x => x.EmailConfirmed == true)
            };

            return model;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public async Task<List<ReferralViewModel>> GetAllUserReferralAsync(Guid appUserId)
        {
            List<ReferralViewModel> refs = new();

            #region F1

            var user = await _userManager.FindByIdAsync(appUserId.ToString());

            var f1Id = Guid.Empty;
            var f2Id = Guid.Empty;

            if (user.ReferralId != null)
            {

                f1Id = user.ReferralId.Value;
                refs.Add(new ReferralViewModel
                {
                    Id = user.ReferralId.Value,
                    Level = 1
                });
            }

            if (f1Id != Guid.Empty)
            {
                var f1User = await _userManager.FindByIdAsync(f1Id.ToString());
                if (f1User.ReferralId != null)
                {
                    refs.Add(new ReferralViewModel
                    {
                        Id = f2Id,
                        Level = 2
                    });
                    f2Id = f1User.ReferralId.Value;
                }



            }

            #endregion

            #region F2

            if (f2Id != Guid.Empty)
            {
                var f2User = await _userManager.FindByIdAsync(f2Id.ToString());
                if (f2User.ReferralId != null)
                {
                    refs.Add(new ReferralViewModel
                    {
                        Id = f2User.ReferralId.Value,
                        Level = 3
                    });
                }
            }

            #endregion

            return refs;
        }

        public List<Guid> GetAllBelowRef(Guid appUserId)
        {

            List<Guid> f1s = new();

            Queue<Guid> f1Q = new();

            f1Q.Enqueue(appUserId);

            Guid toCheck = Guid.Empty;

            while (f1Q.TryDequeue(out toCheck))
            {
                var query = _userManager.Users;

                query = query.Where(x => x.ReferralId == toCheck);

                query = query.Where(x => !x.IsShowOff);

                var f1 = query.ToList();

                if (f1.Count == 0)
                    continue;

                foreach (var u in f1)
                {
                    f1s.Add(u.Id);

                    f1Q.Enqueue(u.Id);
                }
            }


            return f1s;
        }

       

        decimal CalculateTotalRefAmount(List<Guid> userIds, string unit, WalletTransactionType type, DateTime fromDate,
            DateTime toDate)
        {
            var query = _walletTransactionRepository.FindAll(x => userIds.Contains(x.AppUserId));

            query = query.Where(x => x.DateCreated >= fromDate && x.DateCreated <= toDate);

            query = query.Where(x => x.Type == type);

            query = query.Where(x => x.Unit.Equals(unit.ToUpper()));

            var amount = query.Sum(x => x.Amount);

            return amount;

        }

        decimal CalculateTotalRefAmount(List<Guid> userIds, WalletTransactionType type, DateTime fromDate,
            DateTime toDate)
        {
            var query = _walletTransactionRepository.FindAll(x => userIds.Contains(x.AppUserId));

            query = query.Where(x => x.DateCreated >= fromDate && x.DateCreated <= toDate);

            query = query.Where(x => x.Type == type);

            return query.Sum(x => x.Amount);

        }

        decimal IReportService.CalculateTotalStakingAmount(List<Guid> userIds)
        {
            var query = _stakingRepository.FindAll();

            query = query.Where(x => x.Type == StakingType.Process);

            query = query.Where(x => userIds.Contains(x.AppUserId));

            return query.Sum(d => d.StakingAmount);
        }
    }
}
