using Core.Data.Entities;
using Core.Data.IRepositories;

namespace Core.Data.EF.Repositories
{
    public class WalletTransactionRepository : EFRepository<WalletTransaction, int>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
