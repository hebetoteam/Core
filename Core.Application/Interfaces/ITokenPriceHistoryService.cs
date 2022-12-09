using Core.Application.ViewModels.System;
using Core.Data.Enums;
using Core.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface ITokenPriceHistoryService
    {
        PagedResult<TokenPriceHistoryModel> GetAllPaging();

        void Add(decimal price);
    }
}
