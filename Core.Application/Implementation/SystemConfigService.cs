using BeCoreApp.Application.Interfaces;
using BeCoreApp.Application.ViewModels.System;
using BeCoreApp.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using System;
using System.Linq;

namespace BeCoreApp.Application.Implementation
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly ISystemConfigRepository _systemConfigRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemConfigService
            (
            ISystemConfigRepository systemConfigRepository,
            IUnitOfWork unitOfWork
            )
        {
            _systemConfigRepository = systemConfigRepository;
            _unitOfWork = unitOfWork;
        }

        public SystemConfigViewModel Get()
        {
            var query = _systemConfigRepository.FindAll().FirstOrDefault();

            if (query == null)
                return null;

            return new SystemConfigViewModel
            {
                Id = query.Id,
                DateModified = query.DateModified,
                TokenPrice = query.TokenPrice
            };
        }

        public void Update(SystemConfigViewModel Vm)
        {
            var model = _systemConfigRepository.FindById(Vm.Id);

            model.TokenPrice = Vm.TokenPrice;
            model.DateModified = DateTime.UtcNow;

            _systemConfigRepository.Update(model);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
