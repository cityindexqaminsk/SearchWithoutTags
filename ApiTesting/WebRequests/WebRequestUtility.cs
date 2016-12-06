﻿using System.Linq;
using ApiTesting.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace ApiTesting
{
    public class WebRequestUtility
    {
        private string userName;
        private string password;
        private const string defaultPassword = @"password";

        public string TradingApiURL;
        private string TradingApiSessionURL;


        public WebRequestUtility(string userName, string password = defaultPassword)
        {
            this.userName = userName;
            this.password = password;

            TradingApiURL = Settings.Default.Transport + @"//" + Settings.Default.Domain + @"/" + "TradingApi";
            TradingApiSessionURL = TradingApiURL + @"/session";
        }

        private string session;
        public string Session
        {
            get { return session ?? (session = GetSession()); }
            set { session = value; }
        }
        
        private string GetSession()
        {
            var sessionstr = "";
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(TradingApiSessionURL);
                var webRequestBody = "{Password:\"" + password + "\",UserName : \"" + userName + "\"}";
                webRequest.Credentials = new NetworkCredential(userName, password);

                webRequest.Method = WebRequestMethods.Http.Post;
                var buffer = System.Text.Encoding.UTF8.GetBytes(webRequestBody);
                webRequest.ContentType = "application/json; charset=UTF-8";
                webRequest.Accept = "application/json";
                webRequest.ContentLength = buffer.Length;
                webRequest.KeepAlive = true;
                webRequest.Timeout = System.Threading.Timeout.Infinite;
                webRequest.ProtocolVersion = HttpVersion.Version10;
                webRequest.Headers.Add("Body", webRequestBody);
                {
                    var stream = webRequest.GetRequestStream();
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    stream.Close();
                }
                var response = webRequest.GetResponse();
                var responceStr = ConvertResponseToString(response);
                Console.WriteLine("SessionID: " + responceStr);

                sessionstr = responceStr.Split('\"')[3].Trim('\\');
            }
            return sessionstr;
        }

        public string GetJsonStringResponseFromRequestGet(string getURL)
        {
            var webGetRequest = (HttpWebRequest)WebRequest.Create(getURL);
            webGetRequest.Method = WebRequestMethods.Http.Get;
            webGetRequest.Timeout = System.Threading.Timeout.Infinite;
            webGetRequest.ProtocolVersion = HttpVersion.Version10;
            webGetRequest.Headers["Session"] = Session;
            webGetRequest.Headers["UserName"] = userName;
            var response1 = webGetRequest.GetResponse();

            var jSonString = ConvertResponseToString(response1);
            return jSonString;
        }
        public string GenerateQueryString(string url, Dictionary<string, string> argsDictionary)
        {
            var listOfParams = new Dictionary<string, string>();
            foreach (var arg in argsDictionary)
            {
                if (listOfParams.ContainsKey(arg.Key))
                    listOfParams[arg.Key] = arg.Value;
                else
                    listOfParams.Add(arg.Key, arg.Value);
            }
            return url + "?" + CreateQueryString(listOfParams);
        }
        private string CreateQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kvp =>
               string.Format("{0}={1}", kvp.Key, System.Net.WebUtility.UrlEncode(kvp.Value))));
        }
        private string ConvertResponseToString(WebResponse response)
        {
            var builder = new StringBuilder();
            using (var receiveStream = response.GetResponseStream())
            {
                var readStream = new StreamReader(receiveStream, Encoding.UTF8);
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
