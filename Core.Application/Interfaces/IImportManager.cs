using Core.Application.ViewModels.ImportExport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IImportManager
    {
        Task<ImportResponseResult> ImportShowUsersFromXlsx(Stream stream);
    }
}
