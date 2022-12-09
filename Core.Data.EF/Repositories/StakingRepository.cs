using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class StakingRepository : EFRepository<Staking, int>, IStakingRepository
    {
        AppDbContext _context;
        public StakingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
