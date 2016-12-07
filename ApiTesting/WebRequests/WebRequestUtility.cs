using System.Linq;
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
        public string GetSession(string sessionUrl, string userName, string password)
        {
            var sessionstr = "";
            {
                var webRequestBody = "{Password:\"" + password + "\",UserName : \"" + userName + "\"}";
                var response = GetResponseFromRequestPost(sessionUrl, webRequestBody, userName, password);
                var responceStr = ConvertResponseToString(response);
                Console.WriteLine("SessionID: " + responceStr);

                sessionstr = responceStr.Split('\"')[3].Trim('\\');
            }
            return sessionstr;
        }

        public WebResponse GetResponseFromRequestPost(string requestUri, string requestBody, string userName = "",
            string password = "")
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            if (userName != "" || password != "")
                webRequest.Credentials = new NetworkCredential(userName, password);

            webRequest.Method = WebRequestMethods.Http.Post;
            var buffer = System.Text.Encoding.UTF8.GetBytes(requestBody);
            webRequest.ContentType = "application/json; charset=UTF-8";
            webRequest.Accept = "application/json";
            webRequest.ContentLength = buffer.Length;
            webRequest.KeepAlive = true;
            webRequest.Timeout = System.Threading.Timeout.Infinite;
            webRequest.ProtocolVersion = HttpVersion.Version10;
            webRequest.Headers.Add("Body", requestBody);
            {
                var stream = webRequest.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
            }
            return webRequest.GetResponse();
        }

        private WebResponse GetResponseFromRequestGet(string requestUri, string userName = "", string session = "")
        {
            var webGetRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            webGetRequest.Method = WebRequestMethods.Http.Get;
            webGetRequest.Timeout = System.Threading.Timeout.Infinite;
            webGetRequest.ProtocolVersion = HttpVersion.Version10;
            if (session != "")
                webGetRequest.Headers["Session"] = session;
            if (userName != "")
                webGetRequest.Headers["UserName"] = userName;
            return webGetRequest.GetResponse();
        }

        public string GetResponseStringFromRequestGet(string requestUri, string userName = "", string session = "")
        {
            var response = GetResponseFromRequestGet(requestUri, userName, session);
            return ConvertResponseToString(response);
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
        public string ConvertResponseToString(WebResponse response)
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
