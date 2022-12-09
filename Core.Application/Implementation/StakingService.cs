using BeCoreApp.Data.Enums;
using Core.Application.Interfaces;
using Core.Application.ViewModels.BotTelegram;
using Core.Application.ViewModels.Staking;
using Core.Application.ViewModels.System;
using Core.Data.Entities;
using Core.Data.Enums;
using Core.Data.IRepositories;
using Core.Infrastructure.Interfaces;
using Core.Infrastructure.Telegram;
using Core.Utilities.Constants;
using Core.Utilities.Dtos;
using Core.Utilities.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Implementation
{
    public class StakingService : IStakingService
    {
        private readonly IStakingRepository _stakingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StakingService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStakingAffiliateService _stakingAffiliateService;
        private readonly IStakingRewardService _stakingRewardService;
        private readonly IConfiguration _configuration;
        private readonly IBlockChainService _blockChainService;
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IConfigService _configService;
        private readonly TelegramBotWrapper _botTelegramService;

        public StakingService(
            IConfigService configService,
            IBlockChainService blockChainService,
            IConfiguration configuration,
            IStakingRewardService stakingRewardService,
            IStakingAffiliateService stakingAffiliateService,
            UserManager<AppUser> userManager,
            ILogger<StakingService> logger,
            IStakingRepository stakingRepository,
            IWalletTransactionService walletTransactionService,
            IUnitOfWork unitOfWork,
            TelegramBotWrapper botTelegramService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _userManager = userManager;
            _configService = configService;
            _stakingRepository = stakingRepository;
            _blockChainService = blockChainService;
            _stakingRewardService = stakingRewardService;
            _stakingAffiliateService = stakingAffiliateService;
            _walletTransactionService = walletTransactionService;
            _botTelegramService = botTelegramService;
        }

        public PagedResult<StakingViewModel> GetAllPaging(string keyword, string appUserId,
            DateTime? fromDate,
            DateTime? toDate,
            StakingTimeLine timeline,
            int pageIndex,
            int pageSize)
        {
            var query = _stakingRepository.FindAll(x => x.AppUser);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                x.AppUser.Email.Contains(keyword)
                || x.AppUser.PublishKey.Contains(keyword)
                || x.PublishKey.Contains(keyword)
                || x.TransactionHash.Contains(keyword)
                );
            }

            if (fromDate != null && toDate != null)
                query = query.Where(x => x.DateCreated >= fromDate.Value && x.DateCreated <= toDate.Value);

            if (timeline > 0)
                query = query.Where(x => x.TimeLine == timeline);

            if (!string.IsNullOrWhiteSpace(appUserId))
                query = query.Where(x => x.AppUserId.ToString() == appUserId);

            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(x => new StakingViewModel()
                {
                    Id = x.Id,
                    Package = x.Package,
                    PackageName = x.Package.GetDescription(),
                    InterestRate = x.InterestRate,
                    ReceiveAmount = x.ReceiveAmount,
                    ReceiveLatest = x.ReceiveLatest,
                    ReceiveTimes = x.ReceiveTimes,
                    StakingAmount = x.StakingAmount,
                    StakingTimes = x.StakingTimes,
                    TimeLine = x.TimeLine,
                    TimeLineName = x.TimeLine.GetDescription(),
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = x.AppUser.Sponsor,
                    DateCreated = x.DateCreated,
                    Type = x.Type,
                    TypeName = x.Type.GetDescription(),
                    PublishKey = x.PublishKey,
                    TransactionHash = x.TransactionHash
                }).ToList();

            return new PagedResult<StakingViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public List<StakingViewModel> GetLeaderboard(int top)
        {
            var query = _stakingRepository.FindAll(x => x.AppUser);


            var totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Take(top)
                .Select(x => new StakingViewModel()
                {
                    Id = x.Id,
                    Package = x.Package,
                    PackageName = x.Package.GetDescription(),
                    InterestRate = x.InterestRate,
                    ReceiveAmount = x.ReceiveAmount,
                    ReceiveLatest = x.ReceiveLatest,
                    ReceiveTimes = x.ReceiveTimes,
                    StakingAmount = x.StakingAmount,
                    StakingTimes = x.StakingTimes,
                    TimeLine = x.TimeLine,
                    TimeLineName = x.TimeLine.GetDescription(),
                    AppUserId = x.AppUserId,
                    AppUserName = x.AppUser.UserName,
                    Sponsor = x.AppUser.Sponsor,
                    DateCreated = x.DateCreated,
                    Type = x.Type,
                    TypeName = x.Type.GetDescription(),
                    PublishKey = x.PublishKey,
                    TransactionHash = x.TransactionHash
                }).ToList();

            return data;
        }

        public IQueryable<Staking> GetAllByUserId(Guid appUserId)
        {
            var query = _stakingRepository.FindAll(x => x.AppUserId == appUserId);
            return query;
        }

        public decimal GetTotalPackage(Guid userId, StakingType? type)
        {
            decimal totalStaking = 0;
            var staking = _stakingRepository.FindAll(x => x.AppUserId == userId);

            if (type != null)
                staking = staking.Where(x => x.Type == type);

            totalStaking = staking.Sum(x => x.StakingAmount);

            return totalStaking;
        }

        public async Task<GenericResult> StakingToken(string modelJson, string userId)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<BuyStakingViewModel>(modelJson);

                model.AmountPayment = (int)model.Package;

                var appUser = await _userManager.FindByIdAsync(userId);
                if (appUser == null)
                    return new GenericResult(false, "Account does not exist");

                var paymentRet = PaymentStakingToken(model, appUser);
                if (!paymentRet.Success)
                    return paymentRet;

                decimal interestRate = 0;

                if (model.AmountPayment >= 100000)
                {
                    interestRate = 12;
                }
                else if (model.AmountPayment >= 10000 && model.AmountPayment < 100000)
                {
                    interestRate = 10;
                }
                else
                {
                    interestRate = 8;
                }

                var wallet = _blockChainService.CreateAccount();

                var staking = new StakingViewModel
                {
                    TimeLine = StakingTimeLine.TwelveMonth,
                    Package = model.Package,
                    AppUserId = appUser.Id,
                    InterestRate = interestRate,
                    StakingTimes = (int)StakingTimeLine.TwelveMonth * 30,
                    StakingAmount = model.AmountPayment,
                    ReceiveTimes = 0,
                    ReceiveAmount = 0,
                    Type = StakingType.Process,
                    ReceiveLatest = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    PublishKey = wallet.Address,
                    PrivateKey = wallet.PrivateKey
                };

#if RELEASE
                var transaction = await _blockChainService.SendERC20Async(
                                           CommonConstants.TransferPrKey,
                                           staking.PublishKey,
                                           CommonConstants.TokenContract,
                                           staking.StakingAmount,
                                           CommonConstants.TokenDecimals,
                                           CommonConstants.Url);

                if (transaction.Succeeded(true))
                {
                    staking.TransactionHash = transaction.TransactionHash;

                    _logger.LogInformation($"StakingService_StakingToken [UserName]:{appUser.UserName} [PrivateKey]:{staking.PrivateKey}");
                }
#endif

                Add(staking);

                _unitOfWork.Commit();

                #region  LeaderShip 
                LeaderShip(appUser, model.AmountPayment);
                #endregion



                await SendChannelTeleGroup(staking, appUser);

                return new GenericResult(true, "Staking is successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError("StakingController_BuyStaking: {0}", ex.Message);
                return new GenericResult(false, ex.Message);
            }
        }

        private GenericResult PaymentStakingToken(BuyStakingViewModel model, AppUser appUser)
        {
            decimal amountPayment = model.AmountPayment;

            if (model.Unit == Unit.HBT)
            {
                if (amountPayment > appUser.HBTAmount)
                    return new GenericResult(false,
                       $"Your HBT balance is not enough {amountPayment} HBT to make a transaction");

                appUser.HBTAmount -= amountPayment;
            }
            else
            {
                var tokenPrice = _configService.GetTokenPrice();

                amountPayment = Math.Round(amountPayment * tokenPrice, 4);

                if (amountPayment > appUser.USDTAmount)
                    return new GenericResult(false,
                        $"Your USDT balance is not enough {amountPayment} USDT to make a transaction");

                appUser.USDTAmount -= amountPayment;
            }

            appUser.StakingAmount += model.AmountPayment;

            var updateUserPayment = _userManager.UpdateAsync(appUser).Result;

            if (updateUserPayment.Succeeded)
            {
                string txt = Guid.NewGuid().ToString();
                var transaction = new WalletTransactionViewModel()
                {
                    AddressFrom = $"Wallet {model.Unit.GetDescription()}",
                    AddressTo = "System",
                    Amount = amountPayment,
                    Fee = 0,
                    FeeAmount = 0,
                    AmountReceive = amountPayment,
                    AppUserId = appUser.Id,
                    DateCreated = DateTime.UtcNow,
                    TransactionHash = txt,
                    Type = WalletTransactionType.Staking,
                    Unit = model.Unit
                };

                _walletTransactionService.Add(transaction);
                _walletTransactionService.Save();

                ReferralDirect(appUser, model.AmountPayment, txt);
            }
            else
            {
                return new GenericResult(false,
                    string.Join(",", updateUserPayment.Errors.Select(x => x.Description)));
            }

            return new GenericResult(true, "success");
        }

        private void ReferralDirect(AppUser appUser, decimal amountPayment, string txt)
        {
            #region Referral Direct (only 1 level)

            var referralF1 = _userManager.FindByIdAsync(appUser.ReferralId.ToString()).Result;

            if (referralF1 != null && referralF1.IsSystem == false)
            {
                var maxoutSummary = GetMaxProfitSummary(referralF1.Id.ToString());

                //var isMaxOutProfit =   CheckMaxOutProfit(referralF1.Id.ToString());

                if (!maxoutSummary.IsMaxOut)
                {
                    var amountAffiliate = Math.Round(amountPayment * CommonConstants.StakingReferralDirect / 100, 4);

                    string remarks = string.Empty;

                    if (amountAffiliate > maxoutSummary.RemainProfit)
                    {
                        remarks = $"Origin Profit = {amountAffiliate} - Max Profit remain = {maxoutSummary.RemainProfit}";

                        amountAffiliate = maxoutSummary.RemainProfit;
                    }

                    if (amountAffiliate > 0)
                    {
                        referralF1.HBTAmount += amountAffiliate;

                        var updateF1Affiliate = _userManager.UpdateAsync(referralF1).Result;

                        if (updateF1Affiliate.Succeeded)
                        {
                            _walletTransactionService.Add(new WalletTransactionViewModel()
                            {
                                AddressFrom = "Staking",
                                AddressTo = "Wallet HBT",
                                Amount = amountAffiliate,
                                AmountReceive = amountAffiliate,
                                AppUserId = referralF1.Id,
                                DateCreated = DateTime.UtcNow,
                                Fee = 0,
                                FeeAmount = 0,
                                TransactionHash = txt,
                                Type = WalletTransactionType.StakingReferralDirect,
                                Unit = Unit.HBT,
                                Remarks = remarks
                            });

                            _walletTransactionService.Save();
                        }
                    }
                }
            }
            #endregion
        }

        private void LeaderShip(AppUser appUser, decimal stakingToken)
        {
            if (appUser.ReferralId.HasValue == true && appUser.IsSystem == false)
            {
                bool isContinue = true;

                while (isContinue)
                {
                    appUser = _userManager.FindByIdAsync(appUser.ReferralId.Value.ToString()).Result;

                    if (appUser != null && appUser.IsSystem == false)
                    {
                        appUser.StakingAffiliateAmount += stakingToken;

                        string remark = string.Empty;

                        decimal leadership = 0;

                        var maxoutSummary = GetMaxProfitSummary(appUser.Id.ToString());

                        if (!maxoutSummary.IsMaxOut)
                        {
                            decimal stakingTotal = appUser.StakingAffiliateAmount;

                            if (appUser.StakingLevel < StakingLevel.Start1)
                            {
                                if (stakingTotal >= 500000)
                                {
                                    leadership = 1000;
                                    appUser.StakingLevel = StakingLevel.Start1;
                                }
                            }

                            if (appUser.StakingLevel < StakingLevel.Start2)
                            {
                                if (stakingTotal >= 1000000)
                                {
                                    leadership = 2000;
                                    //appUser.HBTAmount += leadership;
                                    appUser.StakingLevel = StakingLevel.Start2;
                                }
                            }

                            if (appUser.StakingLevel < StakingLevel.Start3)
                            {
                                if (stakingTotal >= 2000000)
                                {
                                    leadership = 5000;
                                    appUser.StakingLevel = StakingLevel.Start3;
                                }
                            }

                            if (appUser.StakingLevel < StakingLevel.Start4)
                            {
                                if (stakingTotal >= 5000000)
                                {
                                    leadership = 15000;
                                    appUser.StakingLevel = StakingLevel.Start4;
                                }
                            }

                            if (appUser.StakingLevel < StakingLevel.Start5)
                            {
                                if (stakingTotal >= 10000000)
                                {
                                    leadership += 50000;
                                    appUser.StakingLevel = StakingLevel.Start5;
                                }
                            }

                            if (leadership > maxoutSummary.RemainProfit)
                            {
                                remark = $"Origin Profit = {leadership}- Remain Profit = {maxoutSummary.RemainProfit}";

                                leadership = maxoutSummary.RemainProfit;
                            }

                            if (leadership > 0)
                            {
                                appUser.HBTAmount += leadership;
                            }
                        }

                        var updateLeaderShip = _userManager.UpdateAsync(appUser).Result;

                        if (updateLeaderShip.Succeeded)
                        {
                            if (leadership > 0)
                            {
                                _walletTransactionService.Add(new WalletTransactionViewModel
                                {
                                    AddressFrom = "System",
                                    AddressTo = "Wallet HBT",
                                    Amount = leadership,
                                    AmountReceive = leadership,
                                    AppUserId = appUser.Id,
                                    Fee = 0,
                                    FeeAmount = 0,
                                    DateCreated = DateTime.UtcNow,
                                    TransactionHash = "System",
                                    Type = WalletTransactionType.StakingLeadership,
                                    Unit = Unit.HBT,
                                    Remarks = remark
                                });

                                _walletTransactionService.Save();
                            }
                        }
                    }
                    else
                    {
                        isContinue = false;
                    }
                }
            }
        }

        public void PaymentProfit()
        {
            try
            {
                _logger.LogInformation("PaymentProfit_Started");

                var queryStakings = _stakingRepository.FindAll(x =>
                        x.Type == StakingType.Process
                        && x.StakingTimes != x.ReceiveTimes
                        && x.DateCreated.Date != DateTime.UtcNow.Date
                        && x.ReceiveLatest.Date != DateTime.UtcNow.Date
                        ,
                        au => au.AppUser).ToList();

                _logger.LogInformation($"PaymentProfit_Count: {queryStakings.Count()}");

                foreach (var staking in queryStakings)
                {
                    _logger.LogInformation($"PaymentProfit_Id: {staking.Id}");

                    var appUser = staking.AppUser;
                    if (appUser == null)
                        continue;

                    if (appUser.LockoutEnabled && appUser.LockoutEnd != null)
                    {
                        _logger.LogInformation($"PaymentProfit User {appUser} is locked , dont payment profit");
                        continue;
                    }

                    var maxoutSummary = GetMaxProfitSummary(appUser.Id.ToString());

                    if (maxoutSummary.IsMaxOut)
                    {
                        staking.Type = StakingType.MaxOutProfit;
                        _stakingRepository.Update(staking);
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        var dayAmount = Math.Round(staking.StakingAmount / 30, 2);

                        var profit = Math.Round(dayAmount * (staking.InterestRate / 100), 4);

                        var profitAffiliate = profit;

                        string remark = string.Empty;

                        if (profit > maxoutSummary.RemainProfit)
                        {
                            remark = $"Origin Profit = {profit}- Remain Profit = {maxoutSummary.RemainProfit}";

                            profit = maxoutSummary.RemainProfit;
                        }

                        appUser.HBTAmount += profit;

                        var updateProfit = _userManager.UpdateAsync(appUser).Result;

                        if (updateProfit.Succeeded)
                        {
                            staking.ReceiveAmount += profit;
                            staking.ReceiveTimes += 1;
                            staking.ReceiveLatest = DateTime.UtcNow;

                            if (staking.StakingTimes == staking.ReceiveTimes)
                                staking.Type = StakingType.Finish;

                            _stakingRepository.Update(staking);

                            _unitOfWork.Commit();

                            var stakingReward = _stakingRewardService.Add(
                                new StakingRewardViewModel
                                {
                                    AppUserId = appUser.Id,
                                    InterestRate = staking.InterestRate,
                                    DateCreated = DateTime.UtcNow,
                                    Amount = profit,
                                    StakingId = staking.Id
                                });

                            _walletTransactionService.Add(
                                new WalletTransactionViewModel()
                                {
                                    AddressFrom = "System",
                                    AddressTo = "Wallet HBT",
                                    Amount = profit,
                                    AmountReceive = profit,
                                    AppUserId = appUser.Id,
                                    DateCreated = DateTime.UtcNow,
                                    Fee = 0,
                                    FeeAmount = 0,
                                    TransactionHash = "System",
                                    Type = WalletTransactionType.StakingProfit,
                                    Unit = Unit.HBT
                                });

                            _unitOfWork.Commit();

                            if (appUser.IsSystem == false && appUser.ReferralId.HasValue)
                            {
                                #region affiliate On Profit (7 Level and where)

                                var referralF1 = _userManager.FindByIdAsync(appUser.ReferralId.ToString()).Result;
                                if (referralF1 != null && referralF1.IsSystem == false)
                                {
                                    AffiliateOnProfit(referralF1, 3, profitAffiliate, CommonConstants.StakingAffiliateF1, 1, stakingReward);


                                    var referralF2 = _userManager.FindByIdAsync(referralF1.ReferralId.ToString()).Result;
                                    if (referralF2 != null && referralF2.IsSystem == false)
                                    {
                                        AffiliateOnProfit(referralF2, 4, profitAffiliate, CommonConstants.StakingAffiliateF2, 2, stakingReward);


                                        var referralF3 = _userManager.FindByIdAsync(referralF2.ReferralId.ToString()).Result;
                                        if (referralF3 != null && referralF3.IsSystem == false)
                                        {
                                            AffiliateOnProfit(referralF3, 5, profitAffiliate, CommonConstants.StakingAffiliateF3, 3, stakingReward);


                                            var referralF4 = _userManager.FindByIdAsync(referralF3.ReferralId.ToString()).Result;
                                            if (referralF4 != null && referralF4.IsSystem == false)
                                            {
                                                AffiliateOnProfit(referralF4, 6, profitAffiliate, CommonConstants.StakingAffiliateF4, 4, stakingReward);


                                                var referralF5 = _userManager.FindByIdAsync(referralF4.ReferralId.ToString()).Result;
                                                if (referralF5 != null && referralF5.IsSystem == false)
                                                {
                                                    AffiliateOnProfit(referralF5, 7, profitAffiliate, CommonConstants.StakingAffiliateF5, 5, stakingReward);


                                                    var referralF6 = _userManager.FindByIdAsync(referralF5.ReferralId.ToString()).Result;
                                                    if (referralF6 != null && referralF6.IsSystem == false)
                                                    {
                                                        AffiliateOnProfit(referralF6, 8, profitAffiliate, CommonConstants.StakingAffiliateF6, 6, stakingReward);


                                                        var referralF7 = _userManager.FindByIdAsync(referralF6.ReferralId.ToString()).Result;
                                                        if (referralF7 != null && referralF7.IsSystem == false)
                                                        {
                                                            AffiliateOnProfit(referralF7, 9, profitAffiliate, CommonConstants.StakingAffiliateF7, 7, stakingReward);


                                                            var referralF8 = _userManager.FindByIdAsync(referralF7.ReferralId.ToString()).Result;
                                                            if (referralF8 != null && referralF8.IsSystem == false)
                                                            {
                                                                AffiliateOnProfit(referralF8, 10, profitAffiliate, CommonConstants.StakingAffiliateF8, 8, stakingReward);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            _logger.LogError($"PaymentProfit_UpdateProfit - UserName:{appUser.UserName}, Profit:{profit}");

                            _logger.LogError("PaymentProfit_UpdateProfit: {0}", updateProfit.Errors.Select(x => x.Description));
                        }
                    }
                }

                _logger.LogInformation("PaymentProfit_Finished");
            }
            catch (Exception ex)
            {
                _logger.LogError("PaymentProfit: {0}", ex);
            }
        }

        private void AffiliateOnProfit(AppUser referralFn, int conditionChildren,
            decimal profitAffiliate, decimal percentAmount, int affiliateIndex, StakingReward stakingReward
            )
        {
            try
            {
                var maxoutSummary = GetMaxProfitSummary(referralFn.Id.ToString());

                if (!maxoutSummary.IsMaxOut)
                {
                    var referralFnChildren = _userManager.Users
                            .Where(x => x.ReferralId == referralFn.Id && x.StakingAmount >= 2000);

                    if (referralFnChildren.Count() >= conditionChildren)
                    {
                        decimal affiliateFn = profitAffiliate * percentAmount / 100;

                        string remark = string.Empty;

                        if (affiliateFn > maxoutSummary.RemainProfit)
                        {
                            remark = $"Origin Profit = {affiliateFn}- Remain Profit = {maxoutSummary.RemainProfit}";

                            affiliateFn = maxoutSummary.RemainProfit;
                        }

                        referralFn.HBTAmount += affiliateFn;

                        var updateAffiliate = _userManager.UpdateAsync(referralFn).Result;

                        if (updateAffiliate.Succeeded)
                        {
                            _stakingAffiliateService.Add(new StakingAffiliateViewModel
                            {
                                AppUserId = referralFn.Id,
                                DateCreated = DateTime.UtcNow,
                                Amount = affiliateFn,
                                StakingRewardId = stakingReward.Id,
                                Remarks = $"Received {affiliateFn} HBT ( {percentAmount}% of {profitAffiliate} HBT ), " +
                                $"affiliate on profit from F{affiliateIndex} ( {stakingReward.AppUser.UserName} ) with investment " +
                                $"package ( {stakingReward.Staking.Package.GetDescription()}/{stakingReward.Staking.StakingAmount} HBT ) " +
                                $" {remark}"
                            });

                            _walletTransactionService.Add(new WalletTransactionViewModel()
                            {
                                AddressFrom = "Staking",
                                AddressTo = $"Wallet {Unit.HBT.GetDescription()}",
                                Amount = affiliateFn,
                                AmountReceive = affiliateFn,
                                AppUserId = referralFn.Id,
                                DateCreated = DateTime.UtcNow,
                                Fee = 0,
                                FeeAmount = 0,
                                TransactionHash = "System",
                                Type = WalletTransactionType.StakingAffiliateOnProfit,
                                Unit = Unit.HBT,
                                Remarks = remark
                            });

                            _unitOfWork.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("PaymentProfit_AffiliateOnProfit: {0}", ex);
            }
        }

        public StakingViewModel GetById(int id)
        {
            var model = _stakingRepository.FindById(id);
            if (model == null)
                return null;

            var staking = new StakingViewModel()
            {
                Id = model.Id,
                Package = model.Package,
                PackageName = model.Package.GetDescription(),
                InterestRate = model.InterestRate,
                ReceiveAmount = model.ReceiveAmount,
                ReceiveLatest = model.ReceiveLatest,
                ReceiveTimes = model.ReceiveTimes,
                StakingAmount = model.StakingAmount,
                StakingTimes = model.StakingTimes,
                TimeLine = model.TimeLine,
                TimeLineName = model.TimeLine.GetDescription(),
                AppUserId = model.AppUserId,
                AppUserName = model.AppUser.UserName,
                Sponsor = model.AppUser.Sponsor,
                DateCreated = model.DateCreated,
                Type = model.Type,
                TypeName = model.Type.GetDescription()
            };

            return staking;
        }

        public void Update(StakingViewModel model)
        {
            var staking = _stakingRepository.FindById(model.Id);

            staking.Package = model.Package;
            staking.InterestRate = model.InterestRate;
            staking.TimeLine = model.TimeLine;
            staking.StakingTimes = model.StakingTimes;
            staking.StakingAmount = model.StakingAmount;
            staking.ReceiveTimes = model.ReceiveTimes;
            staking.ReceiveAmount = model.ReceiveAmount;
            staking.ReceiveLatest = model.ReceiveLatest;
            staking.AppUserId = model.AppUserId;
            staking.DateCreated = model.DateCreated;
            staking.Type = model.Type;

            _stakingRepository.Update(staking);
        }

        public void Add(StakingViewModel model)
        {
            var transaction = new Staking()
            {
                Package = model.Package,
                InterestRate = model.InterestRate,
                TimeLine = model.TimeLine,
                StakingTimes = model.StakingTimes,
                StakingAmount = model.StakingAmount,
                ReceiveTimes = model.ReceiveTimes,
                ReceiveAmount = model.ReceiveAmount,
                ReceiveLatest = model.ReceiveLatest,
                AppUserId = model.AppUserId,
                DateCreated = model.DateCreated,
                Type = model.Type,
                PrivateKey = model.PrivateKey,
                PublishKey = model.PublishKey,
                TransactionHash = model.TransactionHash
            };

            _stakingRepository.Add(transaction);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public decimal GetTotalStaking()
        {
            var query = _stakingRepository.FindAll(x => x.AppUser);

            var total = query.Sum(x => x.StakingAmount);

            return total;
        }

        public decimal GetTodayStaking()
        {
            var today = DateTime.UtcNow.Date;

            var query = _stakingRepository.FindAll(x => x.AppUser);

            query = query.Where(x => x.DateCreated >= today);

            var total = query.Sum(x => x.StakingAmount);

            return total;
        }

        public decimal GetMaxout(string appUserId)
        {
            var appUser = _userManager.FindByIdAsync(appUserId).Result;

            decimal totalStaking = appUser.StakingAmount;

            if (totalStaking == 0)
                return 0;

            var transactions = _walletTransactionService.GetAllByUserId(appUser.Id);

            var profitTransactions = transactions
                 .Where(x => x.Type == WalletTransactionType.StakingLeadership
                 || x.Type == WalletTransactionType.StakingProfit
                 || x.Type == WalletTransactionType.StakingAffiliateOnProfit
                 || x.Type == WalletTransactionType.StakingReferralDirect);

            decimal totalProfit = profitTransactions.Sum(x => x.Amount);


            decimal maxOutProfit = (3 - (totalProfit / totalStaking)) * 100;

            return maxOutProfit;

        }

        //public bool CheckMaxOutProfit(string appUserId)
        //{
        //    var appUser = _userManager.FindByIdAsync(appUserId).Result;

        //    decimal totalStaking = appUser.StakingAmount;

        //    if (totalStaking == 0)
        //        return true;

        //    var transactions = _walletTransactionService.GetAllByUserId(appUser.Id);

        //    var profitTransactions = transactions
        //         .Where(x => x.Type == WalletTransactionType.StakingLeadership
        //         || x.Type == WalletTransactionType.StakingProfit
        //         || x.Type == WalletTransactionType.StakingAffiliateOnProfit
        //         || x.Type == WalletTransactionType.StakingReferralDirect);

        //    decimal totalProfit = profitTransactions.Sum(x => x.Amount);

        //    decimal maxOutProfit = totalStaking * 3;

        //    return maxOutProfit <= totalProfit;
        //}

        public MaxProfitViewModel GetMaxProfitSummary(string appUserId)
        {
            MaxProfitViewModel response = new() { };

            var appUser = _userManager.FindByIdAsync(appUserId).Result;

            var transactions = _walletTransactionService.GetAllByUserId(appUser.Id);

            var profitTransactions = transactions
                 .Where(x => x.Type == WalletTransactionType.StakingLeadership
                 || x.Type == WalletTransactionType.StakingProfit
                 || x.Type == WalletTransactionType.StakingAffiliateOnProfit
                 || x.Type == WalletTransactionType.StakingReferralDirect);

            decimal totalProfit = profitTransactions.Sum(x => x.Amount);


            decimal totalStaking = appUser.StakingAmount;

            decimal maxOutProfit = totalStaking * 3;

            if (totalStaking == 0)
                response.IsMaxOut = true;
            else
                response.IsMaxOut = maxOutProfit <= totalProfit;

            response.RemainProfit = maxOutProfit - totalProfit;

            return response;

        }

        public async Task<int> ReferralStakingLevelSummary(string appUserId)
        {

            var referralF1 = await _userManager.FindByIdAsync(appUserId);

            var f1Count = _userManager.Users.Count(x => x.StakingAmount >= 2000
                && !x.IsSystem
                && x.ReferralId == referralF1.Id);

            var referralLevel = 0;

            for (int i = 3; i < 11; i++)
            {
                if (f1Count >= i)
                    referralLevel++;
            }

            return referralLevel;

        }

        public async Task SendChannelTeleGroup(StakingViewModel staking, AppUser appUser)
        {
            var sponsorEmail = string.Empty;
            if (appUser.ReferralId != null)
            {
                var tmp = await _userManager.FindByIdAsync(appUser.ReferralId.ToString());
                if (tmp != null)
                {
                    sponsorEmail = tmp.Email;
                }
            }
            var message = TelegramBotHelper.BuildReportStakingMessage
                (new DepositMessageParam
                {
                    Amount = staking.StakingAmount,
                    CreatedDate = staking.DateCreated,
                    Currency = "HBT",
                    Email = appUser.Email,
                    SponsorEmail = sponsorEmail,
                    WalletFrom = CommonConstants.TransferPuKey,
                    WalletTo = staking.PublishKey,
                }
           );

            await _botTelegramService.SendMessageAsyncWithSendingBalance(TelegramBotActionType.Deposit, message, TelegramBotHelper.DepositGroup);

        }
    }
}
