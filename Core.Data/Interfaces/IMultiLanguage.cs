using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Data.Interfaces
{
    public interface IMultiLanguage<T>
    {
        T LanguageId { get; set; }
    }
}
