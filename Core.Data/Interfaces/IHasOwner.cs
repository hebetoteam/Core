using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Interfaces
{
    public interface IHasOwner<T>
    {
        T OwnerId { get; set; }
    }
}
