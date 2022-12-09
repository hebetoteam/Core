using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Interfaces
{
    public interface IHasSoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
