using Newtonsoft.Json.Linq;
using QuoteLineBot1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuoteLineBot1
{
    public static class getTronscan
    {
        public static string UsdtContractAddress = "0xdac17f958d2ee523a2206206994597c13d831ec7";

        public static string Get(string address)
        {
            string url = $"https://apilist.tronscanapi.com/api/accountv2?address={address}";
            string web = Request.FetchClient(url, "8d7fe358-d8fd-4991-94ef-f8e292156172");
            string reStr = Fetch(web);
            return reStr;
        }

        private static string Fetch(string web)
        {
            string reStr = "0";
            try
            {
                JObject jObject = JObject.Parse(web);
                var jDatas = jObject.GetValue("withPriceTokens");
               // var jDataFirst = jDatas[0];

                foreach (var item in jDatas)
                {
                    var tokenId = item.SelectToken("tokenId").ToString();
                    var tokenAbbr = item.SelectToken("tokenAbbr").ToString();
                    var balance = item.SelectToken("balance").ToString();

                    if (tokenId.Trim() == "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t")
                    {
                        reStr = balance.Trim();
                    }
                }
            }
            catch (Exception)
            {
                return "0";
            }
            return reStr;
        }

        private static string FetchFormat(string web)
        {
            string reStr = "none";
            try
            {
                reStr = Fetch(web);
                reStr = Format(reStr, 6);
                Console.WriteLine(reStr);
            }
            catch (Exception)
            {
                return "none";
            }
            return reStr;
        }



        private static string Format(string balanceStr, int decimalCount)
        {
            decimal balanceInit = decimal.Parse(balanceStr);
            decimal temp = (decimal)Math.Pow(10, decimalCount);
            decimal balance = balanceInit / temp;
            string balanceStrRe = balance.ToString("N");
            return balanceStrRe;
        }

    }
}