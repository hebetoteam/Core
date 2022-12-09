using Core.Infrastructure.SmartContracts.FunctionMessages.ICD;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.SmartContracts
{
    public class ContractHelper
    {
        public static Task<TReturn> ExecuteQueryFunctionAsync<TReturn, TFunctionMessage>(string privateKey, string contract, TFunctionMessage functionMessage) where TFunctionMessage : FunctionMessage, new()
        {
            var account = new Account(privateKey);
            var web3 = new Web3(account, url: Constants.Config.NodeUrl);

            web3.TransactionManager.UseLegacyAsDefault = true;

            var handler = web3.Eth.GetContractQueryHandler<TFunctionMessage>();
            return handler.QueryAsync<TReturn>(contract, functionMessage);
        }

        public static Task<TransactionReceipt> ExecuteTransactionFunctionAsync<TFunctionMessage>(string privateKey, string contract, TFunctionMessage functionMessage) where TFunctionMessage : FunctionMessage, new()
        {
            var account = new Account(privateKey);
            var web3 = new Web3(account, url: Constants.Config.NodeUrl);

            web3.TransactionManager.UseLegacyAsDefault = true;

            var handler = web3.Eth.GetContractTransactionHandler<TFunctionMessage>();
            return handler.SendRequestAndWaitForReceiptAsync(contract, functionMessage);
        }
    }
}
