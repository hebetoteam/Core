using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class MenuItemRepository : EFRepository<MenuItem, int>, IMenuItemRepository
    {
        public MenuItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
