using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BeCoreApp.Data.Enums
{
    public enum StakingLevel
    {
        [Description("Member")]
        Member = 0,
        [Description("Start 1")]
        Start1 = 1,
        [Description("Start 2")]
        Start2 = 2,
        [Description("Start 3")]
        Start3 = 3,
        [Description("Start 4")]
        Start4 = 4,
        [Description("Start 5")]
        Start5 = 5
    }
}