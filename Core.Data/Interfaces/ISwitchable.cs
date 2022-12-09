using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Interfaces
{
    public interface ISwitchable
    {
        Status Status { get; set; }
    }
}
