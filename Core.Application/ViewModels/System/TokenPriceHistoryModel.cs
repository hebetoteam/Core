using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.System
{
    public class TokenPriceHistoryModel
    {
        public DateTime DateCreated { get;set;}

        public decimal Price { get;set;}

        public int Id { get;set;}
    }
}
