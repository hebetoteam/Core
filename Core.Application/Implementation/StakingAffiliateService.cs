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
    public class StakingAffiliateService : IStakingAffiliateService
    {
        private readonly IStakingAffiliateRepository _stakingAffiliateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StakingAffiliateService(
            IStakingAffiliateRepository stakingAffiliateRepository,
            IUnitOfWork unitOfWork)
        {
            _stakingAffiliateRepository = stakingAffiliateRepository;
            _unitOfWork = unitOfWork;
        }

        public PagedResult<StakingAffiliateViewModel> GetAllPaging(string keyword, string appUserId, int pageIndex, int pageSize)
        {
            var query = _stakingAffiliateRepository.FindAll(x => x.AppUser);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.AppUser.Email.Contains(keyword)
                || x.AppUser.Sponsor.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(appUserId))
                query = query.Where(x => x.AppUserId.ToString() == appUserId);

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new StakingAffiliateViewModel()
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    StakingRewardId = x.StakingRewardId,
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = $"{ x.AppUser.Sponsor}",
                    DateCreated = x.DateCreated,
                    Remarks = x.Remarks
                }).ToList();

            return new PagedResult<StakingAffiliateViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public void Add(StakingAffiliateViewModel model)
        {
            var transaction = new StakingAffiliate()
            {
                Amount = model.Amount,
                StakingRewardId = model.StakingRewardId,
                AppUserId = model.AppUserId,
                DateCreated = model.DateCreated,
                Remarks = model.Remarks
            };

            _stakingAffiliateRepository.Add(transaction);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
