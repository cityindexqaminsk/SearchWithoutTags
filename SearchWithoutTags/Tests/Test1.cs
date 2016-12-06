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
        private string userName = "DM241228";// "DM709822";

        private WebRequestsMethods webReq = new WebRequestsMethods();
        
        
        private string session;
        private string Session
        {
            get
            {
                if (session == null) 
                    session = webReq.GetSession(userName);
                return session;
            }
            set { session = value; }
        }

        [Test]
        [TestCase("query", "UK 100")]
        [TestCase("query", "EUR")]
        [TestCase("maxResults", "30")]
        public void TestMethod1(string ar1, string ar2)
        {
            var parametrs = new Dictionary<string, string>() { { ar1, ar2 } };
            
            var requestParams = webReq.GetParams(parametrs);
            
            var responseWithString = webReq.FullSearchWithTags(userName, Session, requestParams);
            var withTags = JsonConvert.DeserializeObject<ResponseWithTags>(responseWithString);

            var responseWithoutString = webReq.SearchWithoutTags(userName, Session, requestParams);
            var withoutTags = JsonConvert.DeserializeObject<ResponseWithoutTags>(responseWithoutString);

            AssertResponses(withoutTags, withTags);
        }

        public void AssertResponses(ResponseWithoutTags without, ResponseWithTags with)
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