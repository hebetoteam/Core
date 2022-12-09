using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Enums
{
    public enum InvestProfitHistoryType
    {
        [Description("PENDING")]
        PENDING = 1,
        [Description("BUY")]
        BUY = 2,
        [Description("SELL")]
        SELL = 3
    }
}
