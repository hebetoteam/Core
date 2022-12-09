using Core.Data.Entities;
using Core.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Repositories
{
    public class BlogTagRepository : EFRepository<BlogTag, int>, IBlogTagRepository
    {
        public BlogTagRepository(AppDbContext context) : base(context)
        {
        }
    }
}
