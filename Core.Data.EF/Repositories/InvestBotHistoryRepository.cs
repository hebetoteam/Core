using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF.Repositories
{
    public class InvestProfitHistoryRepository : EFRepository<InvestProfitHistory, int>, IInvestProfitHistoryRepository
    {
        private readonly AppDbContext _context;

        public InvestProfitHistoryRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }

        public InvestProfitHistory GetPendingInvestProfitHistory()
        {
            var query = FindAll();

            query = query.Where(x=>x.Type == Enums.InvestProfitHistoryType.PENDING );

            return query.SingleOrDefault();
        }
    }
}
