using BeCoreApp.Data.Entities;
using BeCoreApp.Data.IRepositories;
using Core.Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCoreApp.Data.EF.Repositories
{
    public class SystemConfigRepository : EFRepository<SystemConfig, int>, ISystemConfigRepository
    {
        public SystemConfigRepository(AppDbContext context) : base(context)
        {
        }
    }
}
