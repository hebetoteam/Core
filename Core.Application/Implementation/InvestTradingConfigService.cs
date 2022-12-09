using Core.Application.Interfaces;
using Core.Application.ViewModels.InvestTradingBot;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Dtos;
using System.Linq;

namespace Core.Application.Implementation
{
    public class InvestTradingConfigService : IInvestTradingConfigService
    {
        private IInvestBotConfigRepository _investBotConfigRepository;
        private IUnitOfWork _unitOfWork;
        public InvestTradingConfigService(IInvestBotConfigRepository investBotConfigRepository,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _investBotConfigRepository = investBotConfigRepository;
        }

        public PagedResult<InvestTradingConfigsViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize)
        {
            var query = _investBotConfigRepository.FindAll();

           
            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new InvestTradingConfigsViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Name = x.ConfigName,
                    Value = x.ConfigValue
                }).ToList();

            return new PagedResult<InvestTradingConfigsViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public InvestTradingConfigsViewModel GetById(int id)
        {
            var query = _investBotConfigRepository.FindById(id);

            return new InvestTradingConfigsViewModel
            {
                Description = query.Description,
                Id = query.Id,
                Name = query.ConfigName,
                Value = query.ConfigValue
            };
        }

        public void SaveConfig(InvestTradingConfigsViewModel model)
        {
            var entity = _investBotConfigRepository.FindById(model.Id);

            entity.ConfigValue = model.Value;

            _investBotConfigRepository.Update(entity);

            _unitOfWork.Commit();


        }
    }
}
