using Core.Data.Entities;
using Core.Data.IRepositories;

namespace Core.Data.EF.Repositories
{
    public class AirdropRepository : EFRepository<Airdrop, int>, IAirdropRepository
    {
        public AirdropRepository(AppDbContext context) : base(context)
        {
        }
    }
}
