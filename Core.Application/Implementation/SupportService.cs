using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Application.Interfaces;
using Core.Application.ViewModels.Blog;
using Core.Application.ViewModels.Common;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Helpers;

namespace Core.Application.Implementation
{
    public class SupportService : ISupportService
    {
        private readonly ISupportRepository _supportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupportService(ISupportRepository supportRepository, IUnitOfWork unitOfWork)
        {
            _supportRepository = supportRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(SupportViewModel supportVm)
        {
            var support = new Support
            {
                Name = supportVm.Name,
                AppUserId = supportVm.AppUserId,
                RequestContent = supportVm.RequestContent,
                Type = SupportType.New,
                DateModified = DateTime.UtcNow,
                DateCreated = DateTime.UtcNow
            };

            _supportRepository.Add(support);
        }

        public void Delete(int id) => _supportRepository.Remove(id);

        public PagedResult<SupportViewModel> GetAllPaging(string keyword, string appUserId, int pageIndex, int pageSize)
        {
            var query = _supportRepository.FindAll(x => x.AppUser);

            if (!string.IsNullOrWhiteSpace(appUserId))
                query = query.Where(x => x.AppUserId == new Guid(appUserId));

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Name.ToLower().Contains(keyword.ToLower()));

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Type).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new SupportViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    RequestContent = x.RequestContent,
                    ResponseContent = x.ResponseContent,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    Type = x.Type
                }).ToList();

            return new PagedResult<SupportViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public SupportViewModel GetById(int id)
        {
            var model = _supportRepository.FindById(id, x => x.AppUser);
            if (model == null)
                return null;

            return new SupportViewModel
            {
                Id = model.Id,
                Name = model.Name,
                AppUserId = model.AppUserId,
                AppUserName = model.AppUser.UserName,
                RequestContent = model.RequestContent,
                ResponseContent = model.ResponseContent,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Type = model.Type
            };
        }

        public void Save() => _unitOfWork.Commit();

        public void Update(SupportViewModel blogVm)
        {
            var model = _supportRepository.FindById(blogVm.Id);
            model.ResponseContent = blogVm.ResponseContent;
            model.Type = SupportType.Responded;
            model.DateModified = DateTime.UtcNow;

            _supportRepository.Update(model);
        }
    }
}
