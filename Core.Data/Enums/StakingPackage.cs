using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core.Data.Enums
{
    public enum StakingPackage
    {
        [Description("Dragon")]
        Package1000 = 1000,

        [Description("Phoenix")]
        Package2000 = 2000,

        [Description("Hydra")]
        Package5000 = 5000,

        [Description("Lycanthrope")]
        Package10000 = 10000,

        [Description("Leviathan")]
        Package20000 = 20000,

        [Description("Kraken")]
        Package50000 = 50000,

        [Description("Pegasus")]
        Package100000 = 100000,

        [Description("Centaur")]
        Package200000 = 200000,

        [Description("Griffin")]
        Package500000 = 500000,

        [Description("Cerberus")]
        Package1000000 = 1000000
    }
}