using Core.Data.Entities;
using Core.Data.IRepositories;

namespace Core.Data.EF.Repositories
{
    public class TokenPriceHistoryRepository : EFRepository<TokenPriceHistory, int>, ITokenPriceHistoryRepository
    {
        public TokenPriceHistoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
