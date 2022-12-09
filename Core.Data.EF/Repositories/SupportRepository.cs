using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class SupportRepository : EFRepository<Support, int>, ISupportRepository
    {
        public SupportRepository(AppDbContext context) : base(context)
        {
        }
    }
}
