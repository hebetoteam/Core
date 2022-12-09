using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Dapp
{
    public class DAppTransactionVerifyModel
    {
        public string TransactionHash { get; set; }

        public string Address { get;set;}

        public string DappTxnHash { get; set; }

        public bool IsBNB { get;set;}
    }
}
