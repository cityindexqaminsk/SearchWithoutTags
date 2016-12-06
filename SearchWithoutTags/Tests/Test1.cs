using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NUnit.Framework;
using SearchWithoutTags.WebRequests;

namespace SearchWithoutTags
{
    [TestFixture]
    public class Test1
    {
        private const string userName = "DM241228";// "DM709822";

        private WebServerConnector webConnection = new WebServerConnector(userName);
        
        [Test]
        [TestCase("query", "UK 100")]
        [TestCase("query", "EUR")]
        [TestCase("maxResults", "30")]
        public void AssertResponse(string ar1, string ar2)
        {
            var parametrs = new Dictionary<string, string>() { { ar1, ar2 } };
            
            var requestParams = webConnection.GetChangedDefaultParams(parametrs);
            
            var responseWithString = webConnection.FullSearchWithTags(requestParams);
            var withTags = JsonConvert.DeserializeObject<ResponseWithTags>(responseWithString);

            var responseWithoutString = webConnection.SearchWithoutTags(requestParams);
            var withoutTags = JsonConvert.DeserializeObject<ResponseWithoutTags>(responseWithoutString);

            AssertResponses(withoutTags, withTags);
        }


        [Test, Combinatorial]
        public void CombinationOfParams(
            [Values("USD", "EUR", "UK 100")] string query,
            [Values("0")] string tagId,
            [Values("true", "false")] string searchByMarketCode,
            [Values("true", "false")] string searchByMarketName,
            [Values("true", "false")] string spreadProductType,
            [Values("10")] string maxResults)
        {
            var parametrs = new Dictionary<string, string>()
            {
                { "query", query },
                { "tagId", tagId },
                { "searchByMarketCode", searchByMarketCode },
                { "searchByMarketName", searchByMarketName },
                { "spreadProductType", spreadProductType },
                {"maxResults",maxResults}
            };

            var requestParams = webConnection.GetParams(parametrs);

            var responseWithString = webConnection.FullSearchWithTags(requestParams);
            var withTags = JsonConvert.DeserializeObject<ResponseWithTags>(responseWithString);

            var responseWithoutString = webConnection.SearchWithoutTags(requestParams);
            var withoutTags = JsonConvert.DeserializeObject<ResponseWithoutTags>(responseWithoutString);

            AssertResponses(withoutTags, withTags);
        }



        private void AssertResponses(ResponseWithoutTags without, ResponseWithTags with)
        {
            string marketIdKey = "MarketId";
            foreach (var market in without.Markets)
            {
                var marketIdValue = market[marketIdKey].ToString();
                foreach (var m in market)
                {
                    if (m.Value == null)
                    {
                        var withValue = with.MarketInformation.First(i => i[marketIdKey].ToString() == marketIdValue,
                                "Can't find MarketId '" + marketIdValue + "' in response with Tags")[m.Key];
                        Assert.IsNull(withValue, "Error: Market Information is wrong for MarketId +'" + marketIdValue + "'");
                    }
                    else
                    {
                        string withoutValue = m.Value.ToString();
                        string withValue = with.MarketInformation.First(i => i[marketIdKey].ToString() == marketIdValue,
                                "Can't find MarketId '" + marketIdValue + "' in response with Tags")[m.Key].ToString();
                        Assert.AreEqual(withoutValue, withValue,
                            "Error: Market Information is wrong for MarketId +'" + marketIdValue + "'");
                    }
                }
            }
        }
    }
}