using Core.Data.Enums;
using Core.Data.Interfaces;
using Core.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Entities
{
    [Table("InvestProfitHistory")]
    public class InvestProfitHistory : DomainEntity<int>
    {
        public DateTime DateCreated { get; set; }

        public decimal ProfitPercent { get;set;}

        public decimal StartPrice { get;set;}


        public decimal StopPrice { get; set; }

        public decimal Margin { get; set; }

        public InvestProfitHistoryType Type { get; set; }

        public bool IsWin { get; set; }

        public string Remarks { get;set;}

        public decimal ProfitAmount { get;set;}
    }
}
