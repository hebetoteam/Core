using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Core.Data.EF
{
    public class ModelBulkInsert<TEntity> : IModelBulkInsert<TEntity>

    {

        public void BulkInsert(IEnumerable<TEntity> items, string tableName, string connectionString)
        {

            using var connection = new SqlConnection(connectionString);

            using var dataTable = new DataTable();
            var columns = typeof(TEntity).GetProperties().Select(p => new DataColumn(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)).ToArray();

            dataTable.Columns.AddRange(columns);
            dataTable.AcceptChanges();

            connection.Open();
            var bulkCopy = new SqlBulkCopy(connection)
            {
                DestinationTableName = tableName
            };

            foreach (DataColumn col in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
            }

            var rows = items.Select(r =>
            {
                var row = dataTable.NewRow();
                Array.ForEach(r.GetType().GetProperties().ToArray(), (p) =>
                {
                    row[p.Name] = p.GetValue(r) ?? DBNull.Value;
                });
                return row;
            });
            foreach (var row in rows)
            {
                dataTable.Rows.Add(row);
            }
            dataTable.AcceptChanges();
            if (dataTable.Rows.Count > 0)
            {
                bulkCopy.WriteToServer(dataTable);
            }
            connection.Close();
        }


    }
}
