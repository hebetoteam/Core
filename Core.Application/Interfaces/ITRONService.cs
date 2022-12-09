using Core.Application.ViewModels.TronBlockChain;
using Core.Utilities.Dtos;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface ITRONService
    {
        Task<Generateaddress> GenerateAddress();

        Task<BaseResponse> GetBalanceByAddress(string address);

        Task<BaseResponse> GetTRC20BalanceByAddress(
            string address, string assetAddress);

        Task<BaseResponse> ValidateAddress(string address);

        Task<BaseResponse> EasyTransferByPrivate(
            string privateKey, string toAddress, BigInteger amount);

        Task<BaseResponse> EasyTransferAssetByPrivate(
            string privateKey, string toAddress, string assetAddress, BigInteger amount);
    }
}
