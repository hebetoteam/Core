using Core.Application.Interfaces;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Dtos;
using System.Globalization;
using System.Linq;

namespace Core.Application.Implementation
{

    public class ConfigService : IConfigService
    {

        private readonly IConfigRepository _configRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfigService(
            IConfigRepository configRepository,
            IUnitOfWork unitOfWork
            )
        {
            _configRepository = configRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(ConfigViewModel config)
        {
            var entity = new Config
            {
                Remarks = config.Remarks,
                Name = config.Name,
                Value = config.Value
            };

            _configRepository.Add(entity);

            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            var entity = _configRepository.FindById(id);

            _configRepository.Remove(entity);

            _unitOfWork.Commit();
        }

        public decimal GetTokenPrice()
        {
            var entity = _configRepository.FindById(3);

            return decimal.Parse(entity.Value, CultureInfo.InvariantCulture);
        }

        public PagedResult<ConfigViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize)
        {
            var query = _configRepository.FindAll();

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new ConfigViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Remarks = x.Remarks
                }).ToList();

            return new PagedResult<ConfigViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public ConfigViewModel GetById(int id)
        {
            var entity = _configRepository.FindById(id);

            return new ConfigViewModel
            {
                Id = entity.Id,
                Remarks = entity.Remarks,
                Name = entity.Name,
                Value = entity.Value
            };
        }



        public void Update(ConfigViewModel config)
        {
            var entity = _configRepository.FindById(config.Id);

            entity.Value = config.Value;

            _configRepository.Update(entity);

            _unitOfWork.Commit();
        }
    }
}
