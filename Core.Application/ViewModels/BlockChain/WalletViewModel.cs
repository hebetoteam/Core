using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.ViewModels.BlockChain
{
    public class WalletViewModel
    {
        public string AuthenticatorCode { get; set; }
        public bool Enabled2FA { get; set; }
        public string PrivateKey { get; set; }
        public string PublishKey { get; set; }

        public decimal USDTAmount { get; set; }
        public decimal HBTAmount { get; set; }

        public decimal StakingAmount { get; set; }
    }
}
