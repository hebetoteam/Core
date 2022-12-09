using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Saving
{
    public  class SavingViewModel
    {
        public int Id { get; set; } 

        public string TokenName { get; set; }

        public string TokenCode { get; set; }

        public string TokenImage { get; set; }

        public decimal SavingAmount { get; set; }

        public int TimeLine { get; set; }

        public DateTime SavingDate { get; set; }

        public DateTime EndDate { get;set;}

        public decimal USDAmount { get; set; }

        public decimal InterestedRate { get; set; }

        public decimal ExpectedInterested { get; set; }

        public string Status { get;set;}

        public DateTime ValueDate { get; set; }

        public string Sponsor { get;set; }

        public string UserName { get;set;}
    }
}
