using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core.Data.Enums
{
    public enum StakingType
    {
        [Description("Process")]
        Process = 1,
        [Description("Finish")]
        Finish = 2,
        [Description("Cancel")]
        Cancel = 3,
        [Description("Max Out Profit")]
        MaxOutProfit = 4
    }
}