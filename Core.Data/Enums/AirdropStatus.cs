using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Enums
{
    public enum AirdropStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Rejected")]
        Rejected = 2,
        [Description("Approved")]
        Approved = 3,
    }
}
