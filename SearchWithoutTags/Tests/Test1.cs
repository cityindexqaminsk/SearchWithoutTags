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
        //private string User1 = "DM241228";
        private string userName = "DM709822";

        private string session;

        private string Session
        {
            get
            {
                if (session == null) return new WebRequestsMethods().GetSession(userName);
                else return session;
            }
            set { session = value; }
        }

        [Test]
        [TestCase("Query", "UK 100")]
        [TestCase("Query", "EUR")]
        public void TestMethod1(string ar1, string ar2)
        {
            var parametrs = new Dictionary<string, string>() { { ar1, ar2 } };
            
            var webReq = new WebRequestsMethods();
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
            foreach (var market in without.Market)
            {
                var marketId = market[marketIdKey];
                foreach (var m in market)
                {
                    Assert.AreEqual(m.Value,
                        with.MarketInformation.First(i => i[marketIdKey] == marketId,
                            "Can't find MarketId '" + marketId + "' in resoinse with Tags")[m.Key],
                        "Error: Market Information is wrong for MarketId +'" + marketId + "'");
                }
            }
        }
    }
}