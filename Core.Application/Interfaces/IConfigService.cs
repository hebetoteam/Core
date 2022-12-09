using Core.Application.ViewModels.System;
using Core.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IConfigService
    {
        PagedResult<ConfigViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize);

        void Add(ConfigViewModel config);

        void Update(ConfigViewModel config);

        void Delete(int id);

        ConfigViewModel GetById(int id);

        decimal GetTokenPrice();
    }
}
