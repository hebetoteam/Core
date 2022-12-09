using Core.Data.EF.Extensions;
using Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.EF.Configurations
{
    public class BlogConfiguration : DbEntityConfiguration<Blog>
    {
        public override void Configure(EntityTypeBuilder<Blog> entity)
        {
        }
    }
}
