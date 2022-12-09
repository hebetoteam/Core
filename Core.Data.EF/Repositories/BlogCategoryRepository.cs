using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class BlogCategoryRepository : EFRepository<BlogCategory, int>, IBlogCategoryRepository
    {
        public BlogCategoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
