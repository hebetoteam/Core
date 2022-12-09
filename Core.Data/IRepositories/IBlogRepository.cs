using Core.Data.Entities;
using Core.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.IRepositories
{
    public interface IBlogRepository : IRepository<Blog, int>
    {
    }
}
