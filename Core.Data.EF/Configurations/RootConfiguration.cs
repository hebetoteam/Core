using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeCoreApp.Data.EF.Configurations
{
    public class RootConfiguration 
    {
        public GameConfiguration GameConfiguration { get; } = new GameConfiguration();
    }
}
