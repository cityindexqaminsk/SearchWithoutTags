using System.Collections.Generic;
using ApiTesting.Properties;

namespace ApiTesting.WebRequests
{
    public class WebConnector
    {
        private const string searchWithoutTagsString = "/market/searchwithouttags";
        private const string fullSearchWithTagsString = "/market/fullsearchwithtags";
        public string TradingApiURL = Settings.Default.Transport + @"//" + Settings.Default.Domain + @"/" + "TradingApi";

        private string TradingApiSessionURL = Settings.Default.Transport + @"//" + Settings.Default.Domain + @"/" +
                                              "TradingApi/session";

        private WebRequestUtility webRequestUtility;

        public WebConnector()
        {
            webRequestUtility = new WebRequestUtility();
        }

        public string GetTradingApiSession(string userName, string password = "password")
        {
            return webRequestUtility.GetSession(TradingApiSessionURL, userName, password);
        }

        public string GetFullSearchWithTagsResponse(string userName, string session, Dictionary<string, string> argsDictionary)
        {
            var requestUri = webRequestUtility.GenerateQueryString(TradingApiURL + fullSearchWithTagsString,
                argsDictionary);
            return webRequestUtility.GetResponseStringFromRequestGet(requestUri,
                userName, session);
        }

        internal string GetSearchWithoutTagsResponse(string userName, string session, Dictionary<string, string> argsDictionary)
        {
            var requestUri = webRequestUtility.GenerateQueryString(TradingApiURL + searchWithoutTagsString,
                   argsDictionary);
            return webRequestUtility.GetResponseStringFromRequestGet(requestUri,
                userName, session);
        }
    }
}
