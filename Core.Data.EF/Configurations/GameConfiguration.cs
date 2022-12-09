using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeCoreApp.Data.EF.Configurations
{
    public class GameConfiguration
    {
        public string RegisterUrl { get; set; }

        public string AppUserTable { get;set; } 

        public string SeedGameKey { get; set; } 

        public string TokenName { get; set; }
    }
}
