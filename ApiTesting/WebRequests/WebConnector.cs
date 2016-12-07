using System;
using System.Collections.Generic;
using System.Configuration;

namespace ApiTesting.WebRequests
{
    public class WebConnector
    {
        private const string searchWithoutTagsString = "/market/searchwithouttags";
        private const string fullSearchWithTagsString = "/market/fullsearchwithtags";
        private string userName;
        private string password;
        private string domain;
        private string transport;
        private string TradingApiURL;
        private string TradingApiSessionURL;
        private WebRequestUtility webRequestUtility;


        public WebConnector(string domain, string userName, string password = "password")
        {
            webRequestUtility = new WebRequestUtility();
            this.userName = userName;
            this.domain = domain;
            this.password = password;
            transport = ConfigurationSettings.AppSettings["transport"];

            TradingApiURL = transport + @"//" + domain + @"/" + "TradingApi";
            TradingApiSessionURL = TradingApiURL + @"/session";
        }
        
        private string session;
        public string Session
        {
            get { return session ?? (session = GetTradingApiSession()); }
            set { session = value; }
        }

        public string GetTradingApiSession()
        {
            return webRequestUtility.GetSession(TradingApiSessionURL, userName, password);
        }

        public string GetFullSearchWithTagsResponse(Dictionary<string, string> argsDictionary)
        {
            var requestUri = webRequestUtility.GenerateQueryString(TradingApiURL + fullSearchWithTagsString,
                argsDictionary);
            return webRequestUtility.GetResponseStringFromRequestGet(requestUri,
                userName, Session);
        }

        internal string GetSearchWithoutTagsResponse(Dictionary<string, string> argsDictionary)
        {
            var requestUri = webRequestUtility.GenerateQueryString(TradingApiURL + searchWithoutTagsString,
                   argsDictionary);
            return webRequestUtility.GetResponseStringFromRequestGet(requestUri,
                userName, Session);
        }
    }
}
