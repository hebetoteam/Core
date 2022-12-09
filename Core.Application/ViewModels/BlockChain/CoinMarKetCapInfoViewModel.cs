using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core.Application.ViewModels.BlockChain
{
    public class CoinMarKetCapInfoViewModel
    {
        public CoinMarKetCapInfoViewModel()
        {
            data = new List<CoinData>();
        }

        public List<CoinData> data { get; set; }

        public List<CoinPrice> coinPrice { get; set; }

    }

    public class CoinMarKetCapPriceViewModel
    {
        public RootData data { get;set;}
    }

    public class RootData
    {
        public List<ItemData> tokenData{get;set;}
    }

    public class ItemData
    {
        //public int id { get; set; }
        //public string name { get; set; }
        //public string symbol { get; set; }
        //public string slug { get; set; }
        //public int num_market_pairs { get; set; }
        //public DateTime? date_added { get; set; }
        //public List<object> tags { get; set; }
        //public double? max_supply { get; set; }
        //public double? circulating_supply { get; set; }
        //public double? total_supply { get; set; }
        //public CurrenyInfo platform { get; set; }
        //public int cmc_rank { get; set; }
        //public DateTime last_updated { get; set; }
        public Quote quote { get; set; }
    }

    public class CoinPrice
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }

        public CoinQuote quote { get; set; }


    }

    public class CoinData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public int num_market_pairs { get; set; }
        //public DateTime date_added { get; set; }
        public List<string> tags { get; set; }
        public decimal? max_supply { get; set; }
        public decimal circulating_supply { get; set; }
        public decimal total_supply { get; set; }
        public Platform platform { get; set; }
        public int cmc_rank { get; set; }
        public DateTime last_updated { get; set; }
        public CoinQuote quote { get; set; }
    }

    public class CoinQuote
    {
        public CoinUSD USD { get; set; }
    }

    public class Platform
    {
        public string token_address { get;set; }
    }

    public class CoinUSD
    {
        public decimal price { get; set; }
        public decimal volume_24h { get; set; }
        public decimal percent_change_1h { get; set; }
        public decimal percent_change_24h { get; set; }
        public decimal percent_change_7d { get; set; }
        public decimal market_cap { get; set; }
        public DateTime last_updated { get; set; }

        public decimal open { get;set;}

        public decimal high { get;set;}

        public decimal low { get;set;}

        public decimal close { get;set;}
    }

    public class CoinStatus
    {
        public DateTime timestamp { get; set; }
        public int error_code { get; set; }
        public string error_message { get; set; }
        public int elapsed { get; set; }
        public int credit_count { get; set; }
        public string notice { get; set; }
    }

    public class Periods
    {
        public AllTime all_time { get;set;}
    }

    public class AllTime
    {
        public PriceQuote Quote { get;set;}
    }

    public class PriceQuote
    {
        public PriceCurrency USD { get;set;}
    }

    public class PriceCurrency
    {
        public decimal open { get;set;}

        public decimal close { get; set; }

        public decimal high { get; set; }

        public decimal low { get; set; }
    }

    public class Currency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Rank { get; set; }
        public double Price { get; set; }
        public double Volume24hUsd { get; set; }
        public double MarketCapUsd { get; set; }
        public double PercentChange1h { get; set; }
        public double PercentChange24h { get; set; }
        public double PercentChange7d { get; set; }
        public DateTime LastUpdated { get; set; }
        public Double MarketCapConvert { get; set; }
        public string ConvertCurrency { get; set; }
    }


    public class CurrenyInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public string token_address { get; set; }
    }

    public class CurrenyPriceInfo
    {
        public double? price { get; set; }
        //public double? volume_24h { get; set; }
        //public double? percent_change_1h { get; set; }
        //public double? percent_change_24h { get; set; }
        //public double? percent_change_7d { get; set; }
        //public double? market_cap { get; set; }
        //public DateTime last_updated { get; set; }
    }

    public class Quote
    {
        public CurrenyPriceInfo CurrenyPriceInfo { get; set; }
    }
}
