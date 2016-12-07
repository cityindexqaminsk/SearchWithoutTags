using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using ApiTesting.WebRequests;

namespace ApiTesting
{
    [TestFixture]
    public class Test1
    {
        private const string userName = "DM241228";// "DM709822";
        private WebConnector webConnection = new WebConnector();
        private string _session;

        public string Session
        {
            get { return _session ?? (_session = webConnection.GetTradingApiSession(userName)); }
            set { _session = value; }
        }

        [Test, Combinatorial]
        public void CombinationOfParams(
            [Values("EUR", "UK 100")] string query,
            [Values("0")] string tagId,
            [Values("true", "false")] string searchByMarketCode,
            [Values("true", "false")] string searchByMarketName,
            [Values("true", "false")] string spreadProductType,
            [Values("true", "false")] string cfdProductType,
            [Values("true", "false")] string binaryProductType,
            [Values("true", "false")] string includeOptions,
            [Values("10")] string maxResults,
            [Values("true", "false")] string useMobileShortName)
        {
            var parametrs = new Dictionary<string, string>()
            {
                {"query", query},
                {"tagId", tagId},
                {"searchByMarketCode", searchByMarketCode},
                {"searchByMarketName", searchByMarketName},
                {"spreadProductType", spreadProductType},
                {"cfdProductType", cfdProductType},
                {"binaryProductType", binaryProductType},
                {"includeOptions", includeOptions},
                {"maxResults", maxResults},
                {"useMobileShortName", useMobileShortName}
            };

            //FullSearchWithTags
            var fullSearchWithTagsResponse = webConnection.GetFullSearchWithTagsResponse(userName, Session, parametrs);
            Assert.IsTrue(IsTagsExistInResponse(fullSearchWithTagsResponse),
                "Error: Tags should exist in responseString " + fullSearchWithTagsResponse);
            var withTags = JsonConvert.DeserializeObject<ResponseWithTags>(fullSearchWithTagsResponse);

            //SearchWithoutTags
            var searchWithoutTagsResponse = webConnection.GetSearchWithoutTagsResponse(userName, Session, parametrs);
            Assert.IsFalse(IsTagsExistInResponse(searchWithoutTagsResponse),
                "Error: Tags should not exist in responseString " + searchWithoutTagsResponse);
            var withoutTags = JsonConvert.DeserializeObject<ResponseWithoutTags>(searchWithoutTagsResponse);

            //Assert responses correct
            AssertResponses(withoutTags, withTags);
        }

        private bool IsTagsExistInResponse(string responseString)
        {
            var tagsChecker = JsonConvert.DeserializeObject<TagsChecker>(responseString);
            return tagsChecker.Tags != null;
        }
        private void AssertResponses(ResponseWithoutTags without, ResponseWithTags with)
        {
            Assert.AreEqual(without.Markets.Count, with.MarketInformation.Count,
                "Error: number of returned markets is wrong");
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