using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class WalletTransferRepository : EFRepository<WalletTransfer, int>, IWalletTransferRepository
    {
        public WalletTransferRepository(AppDbContext context) : base(context)
        {
        }
    }
}
