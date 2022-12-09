using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Dtos.Datatables
{
    public class KTDataTablePagination
    {
        public string Query { get; set; }
        public int Page { get; set; } = 1;
        public int Pages { get; set; } = 10;
        public int Perpage { get; set; }
        public int Total { get; set; }
        public string Sort { get; set; } = "asc";
        public string Field { get; set; }
        public int Start
        {
            get { return ((Page - 1) * Perpage) + 1; }
            set { }
        }
    }

    public class DataTableResponse<T>
    {
        public KTDataTablePagination meta { get; set; }
        public List<T> data { get; set; }
    }
}
