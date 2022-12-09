using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class MenuGroupRepository : EFRepository<MenuGroup, int>, IMenuGroupRepository
    {
        public MenuGroupRepository(AppDbContext context) : base(context)
        {
        }
    }
}
