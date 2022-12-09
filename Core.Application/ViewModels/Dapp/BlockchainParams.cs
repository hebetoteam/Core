using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ViewModels.Dapp
{
    public class BlockchainParams
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Value { get; set; }
        public string TransactionHex { get; set; }
        public string Gas { get; set; }
        public string GasPrice { get; set; }
        public decimal Decimals { get; set; }
    }
}
