using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Dtos;
using System;
using System.Linq;

namespace Core.Application.Implementation
{
    public class TokenPriceHistoryService : ITokenPriceHistoryService
    {
        private ITokenPriceHistoryRepository _tokenPriceHistoryRepository;
        private IUnitOfWork _unitOfWork;

        public TokenPriceHistoryService(ITokenPriceHistoryRepository tokenPriceHistoryRepository,
            IUnitOfWork unitOfWork)
        {
            _tokenPriceHistoryRepository = tokenPriceHistoryRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(decimal price)
        {
            _tokenPriceHistoryRepository.Add(new TokenPriceHistory
            {
                DateCreated = DateTime.UtcNow,
                Price = price
            });
            _unitOfWork.Commit();
        }

        public PagedResult<TokenPriceHistoryModel> GetAllPaging()
        {
            var query = _tokenPriceHistoryRepository.FindAll();

            int totalRow = query.Count();
            var data = query.OrderBy(x => x.Id).Skip(0)
                .Take(10).Select(x => new TokenPriceHistoryModel()
                {
                    Id = x.Id,
                    DateCreated = x.DateCreated,
                    Price = x.Price
                }).ToList();

            var paginationSet = new PagedResult<TokenPriceHistoryModel>()
            {
                Results = data,
                RowCount = totalRow
            };

            return paginationSet;
        }
    }
}
