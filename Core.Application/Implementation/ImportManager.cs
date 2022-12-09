using Core.Application.Helpers;
using Core.Application.Interfaces;
using Core.Application.ViewModels.ImportExport;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Implementation
{
    public class ImportManager : IImportManager
    {

        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ImportManager(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IUserService userService) { 
            _unitOfWork = unitOfWork;   
            _userManager = userManager;
            _userService=userService;
        }   

        protected virtual GoogleGISDto GetGoogleDataFromXlsx(PropertyManager<GoogleGISDto> manager,
            ExcelWorksheet worksheet, int iRow)
        {
            manager.ReadFromXlsx(worksheet, iRow);

            var store = new GoogleGISDto();

            foreach (var property in manager.GetProperties)
            {
                switch (property.PropertyName)
                {
                    case nameof(GoogleGISDto.Name):
                        store.Name = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.Fulladdress):
                        store.Fulladdress = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.Cid):
                        store.Cid = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.PlaceId):
                        store.PlaceId = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.Phone):
                        store.Phone = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.Website):
                        store.Website = property.StringValue;
                        break;
                    case nameof(GoogleGISDto.Latitude):
                        store.Latitude = property.DoubleValue;
                        break;
                    case nameof(GoogleGISDto.Longitude):
                        store.Longitude = property.DoubleValue;
                        break;
                    case nameof(GoogleGISDto.Featuredimage):
                        store.Featuredimage = property.StringValue;
                        break;
                }
            }

            return store;
        }

        protected virtual UserImportDTO GetUserShowFromXlsx(PropertyManager<UserImportDTO> manager,
            ExcelWorksheet worksheet, int iRow)
        {
            manager.ReadFromXlsx(worksheet, iRow);

            var user = new UserImportDTO();

            foreach (var property in manager.GetProperties)
            {
                switch (property.PropertyName)
                {
                    case nameof(UserImportDTO.Email):
                        user.Email = property.StringValue;
                        break;
                    case nameof(UserImportDTO.Password):
                        user.Password = property.StringValue;
                        break;
                }
            }

            return user;
        }

        public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[1, poz];

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }

        public async Task<ImportResponseResult> ImportShowUsersFromXlsx(Stream stream)
        {
            using var xlPackage = new ExcelPackage(stream);
            // get the first worksheet in the workbook
            var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new Exception("No worksheet found");

            //the columns
            var properties = GetPropertiesByExcelCells<UserImportDTO>(worksheet);

            var manager = new PropertyManager<UserImportDTO>(properties);
            var iRow = 2;
            int successCount = 0;
            var currentUsers = _userManager.Users.Select(x => x.Email).ToList();
            while (true)
            {
                var allColumnsAreEmpty = manager.GetProperties
                    .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                //get store by data in xlsx file if it possible, or create new category

                var user = GetUserShowFromXlsx(manager, worksheet, iRow);

                if (currentUsers.Any(d => d == user.Email))
                {
                    iRow++;
                    continue;
                }

                await _userService.AddbyImportAsync(user.Email,user.Password);

                successCount++;

                iRow++;
            }

            return new ImportResponseResult
            {
                IsSuccess = successCount > 0,
                ErrorMsg = $"Success {successCount}"
            };
        }
    }
}
