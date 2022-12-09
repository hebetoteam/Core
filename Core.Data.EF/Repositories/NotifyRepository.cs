using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class NotifyRepository : EFRepository<Notify, int>, INotifyRepository
    {
        public NotifyRepository(AppDbContext context) : base(context)
        {
        }
    }
}
