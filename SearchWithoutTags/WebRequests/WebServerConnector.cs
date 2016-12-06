﻿using System.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace SearchWithoutTags
{
    public class WebServerConnector
    {
        private string userName;
        private string password;
        private string domain;

        private const string transport = @"http:";
        private const string defaultDomain = @"pkh-dev-merc1";//@"ciapi.cityindex.com";
        private const string defaultPassword = @"password";

        private string TradingApi;
        private string TradingApiSessionURL;


        public WebServerConnector(string userName, string password = defaultPassword, string domain = defaultDomain)
        {
            this.userName = userName;
            this.password = password;
            this.domain = domain;

            TradingApi = transport + @"//" + domain + @"/" + "TradingApi";
            TradingApiSessionURL = transport + @"//" + domain + @"/TradingApi/session"; 
        }

        private string session;
        public string Session
        {
            get { return session ?? (session = GetSession(userName)); }
            set { session = value; }
        }
        
        private string GetSession(string userName, string psw = "")
        {
            if (psw == "") psw = password;
            var sessionstr = "";
            var postUrl = TradingApiSessionURL;
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(postUrl);
                var str = "{Password:\"" + psw + "\",UserName : \"" + userName + "\"}";
                webRequest.Credentials = new NetworkCredential(userName, psw);

                webRequest.Method = "POST";
                var buffer = System.Text.Encoding.UTF8.GetBytes(str);
                webRequest.ContentType = "application/json; charset=UTF-8";
                webRequest.Accept = "application/json";
                webRequest.ContentLength = buffer.Length;
                webRequest.KeepAlive = true;
                webRequest.Timeout = System.Threading.Timeout.Infinite;
                webRequest.ProtocolVersion = HttpVersion.Version10;
                webRequest.Headers.Add("Body", str);

                var stream = webRequest.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();

                var response = webRequest.GetResponse();
                var responceStr = GetHtmlFromResponce(response);
                Console.WriteLine(responceStr);

                sessionstr = responceStr.Split('\"')[3].Trim('\\');
            }
            return sessionstr;
        }

        public string SearchWithoutTags(string userName, string parameters = "")
        {
            string searchWithoutTagsUrl =
                TradingApi + @"/market/searchwithouttags" + parameters;

            var webGetRequest = (HttpWebRequest)WebRequest.Create(searchWithoutTagsUrl);
            //webGetRequest.Credentials = new NetworkCredential(userName);
            webGetRequest.Method = "GET";
            //webGetRequest.KeepAlive = true;
            webGetRequest.Timeout = System.Threading.Timeout.Infinite;
            webGetRequest.ProtocolVersion = HttpVersion.Version10;
            //webGetRequest.Referer = "https://ciapi.cityindex.com/TradingApi/";
            webGetRequest.Headers["Session"] = Session;
            webGetRequest.Headers["UserName"] = userName;
            var response1 = webGetRequest.GetResponse();

            var jSonString = GetHtmlFromResponce(response1);
            return jSonString;
        }

        public string FullSearchWithTags(string userName, string parameters = "")
        {
            var getUrl =
                TradingApi + @"/market/fullsearchwithtags" + parameters;

            var webGetRequest = (HttpWebRequest) WebRequest.Create(getUrl);
            //webGetRequest.Credentials = new NetworkCredential(UserName, Password);
            webGetRequest.Method = "GET";
            //webGetRequest.KeepAlive = true;
            webGetRequest.Timeout = System.Threading.Timeout.Infinite;
            webGetRequest.ProtocolVersion = HttpVersion.Version10;
            webGetRequest.Referer = "https://ciapi.cityindex.com/TradingApi/";
            webGetRequest.Headers["Session"] = Session;
            webGetRequest.Headers["UserName"] = userName;
            var response1 = webGetRequest.GetResponse();

            var jSonString = GetHtmlFromResponce(response1);

            return jSonString;
            /*var responseD = JsonConvert.DeserializeObject<Dictionary<string, List<MarketInformation>>>(jSonString);
            var findedMarkets = responseD["MarketInformation"];
            return findedMarkets.First(m => m.Name == marketName, "Can't find market '" + marketName + "' in response.");*/
        }

        public string GetParams(Dictionary<string, string> argsDictionary)
        {
            string defaultParams =@"?query=USD&tagId=0&searchByMarketCode=true&searchByMarketName=true&spreadProductType=true&cfdProductType=true&binaryProductType=true&includeOptions=true&maxResults=10&useMobileShortName=false";
            var listOfParams = ParseQueryString(defaultParams);
            foreach (var arg in argsDictionary)
            {
                if (listOfParams.ContainsKey(arg.Key))
                    listOfParams[arg.Key] = arg.Value;
                else
                    listOfParams.Add(arg.Key, arg.Value);
            }
            return "?" + CreateQueryString(listOfParams);
        }


        private Dictionary<string, string> ParseQueryString(string paramsString)
        {
            Dictionary<String, String> queryDict = new Dictionary<string, string>();
            foreach (String token in paramsString.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                    queryDict[parts[0].Trim()] = System.Net.WebUtility.UrlDecode(parts[1]).Trim();
                else
                    queryDict[parts[0].Trim()] = "";
            }
            return queryDict;
        }
        private string CreateQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kvp =>
               string.Format("{0}={1}", kvp.Key, System.Net.WebUtility.UrlEncode(kvp.Value))));
        }
        private string GetHtmlFromResponce(WebResponse response)
        {
            var builder = new StringBuilder();
            using (var receiveStream = response.GetResponseStream())
            {
                var encode = System.Text.Encoding.GetEncoding("utf-8");
                var readStream = new StreamReader(receiveStream, encode);
                var read = new Char[256];
                var count = readStream.Read(read, 0, 256);

                while (count > 0)
                {
                    var str = new String(read, 0, count);
                    builder.Append(str);
                    count = readStream.Read(read, 0, 256);
                }
                readStream.Close();
                response.Close();
            }

            return builder.ToString();
        }
    }
}
