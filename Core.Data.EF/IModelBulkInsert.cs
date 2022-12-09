using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.EF
{
    public interface IModelBulkInsert<TEntity>
    {
        void BulkInsert(IEnumerable<TEntity> items, string tableName, string connectionString);
    }
}
