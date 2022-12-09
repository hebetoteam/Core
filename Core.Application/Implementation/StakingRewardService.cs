using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using System;
using System.Linq;

namespace Core.Application.Implementation
{
    public class StakingRewardService : IStakingRewardService
    {
        private readonly IStakingRewardRepository _stakingRewardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StakingRewardService(
            IStakingRewardRepository stakingRewardRepository,
            IUnitOfWork unitOfWork)
        {
            _stakingRewardRepository = stakingRewardRepository;
            _unitOfWork = unitOfWork;
        }

        public PagedResult<StakingRewardViewModel> GetAllPaging(string keyword, string appUserId, int pageIndex, int pageSize)
        {
            var query = _stakingRewardRepository.FindAll(x => x.AppUser, st => st.Staking);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.AppUser.Email.Contains(keyword)
                || x.AppUser.Sponsor.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(appUserId))
                query = query.Where(x => x.AppUserId.ToString() == appUserId);

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new StakingRewardViewModel()
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Package = x.Staking.Package,
                    PackageName = x.Staking.Package.GetDescription(),
                    StakingAmount = x.Staking.StakingAmount,
                    StakingId = x.StakingId,
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = x.AppUser.Sponsor,
                    DateCreated = x.DateCreated
                }).ToList();

            return new PagedResult<StakingRewardViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public decimal GetTotalStakingCommission()
        {
            var query = _stakingRewardRepository.FindAll(x => x.AppUser);

            var total = query.Sum(x => x.Amount);

            return total;
        }

        public decimal GetTodayStakingCommission()
        {
            var today = DateTime.UtcNow.Date;

            var query = _stakingRewardRepository.FindAll(x => x.AppUser);

            query = query.Where(x => x.DateCreated >= today);

            var total = query.Sum(x => x.Amount);

            return total;
        }

        public StakingReward Add(StakingRewardViewModel model)
        {
            var transaction = new StakingReward()
            {
                Amount = model.Amount,
                InterestRate = model.InterestRate,
                StakingId = model.StakingId,
                AppUserId = model.AppUserId,
                DateCreated = model.DateCreated,
            };

            _stakingRewardRepository.Add(transaction);

            return transaction;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
