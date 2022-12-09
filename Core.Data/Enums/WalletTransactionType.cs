using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Enums
{
    public enum WalletTransactionType
    {
        [Description("Deposit")]
        Deposit = 1,

        [Description("Withdraw")]
        Withdraw = 2,

        [Description("Transfer")]
        Transfer = 3,

        [Description("Received")]
        Received = 4,

        [Description("Staking")]
        Staking = 5,
        [Description("Staking Profit")]
        StakingProfit = 6,
        [Description("Staking Referral Direct")]
        StakingReferralDirect = 7,
        [Description("Staking Affiliate on Profit")]
        StakingAffiliateOnProfit = 8,
        [Description("Staking Leadership")]
        StakingLeadership = 9,
        
        [Description("Swap From HBT")]
        SwapFromHBT = 10,
        [Description("Swap To USDT")]
        SwapToUSDT = 11,

        [Description("Airdrop")]
        Airdrop = 12,
    }
}
