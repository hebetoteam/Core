using Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.InvestTradingBot
{
    public class InvestTradingBotProfitHistoryViewModel
    {
        public int Id { get;set;}

        public DateTime DateCreated { get; set; }

        public decimal ProfitPercent { get; set; }

        public decimal StartPrice { get; set; }

        public decimal StopPrice { get; set; }

        public decimal Margin { get; set; }

        public InvestProfitHistoryType Type { get; set; }

        public string TypeName { get;set;}

        public bool IsWin { get; set; }

        public string Remarks { get; set; }

        public decimal ProfitAmount { get;set;}

        public string Result { get;set;}
    }
}
