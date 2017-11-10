using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Web
{
    /// <summary>
    /// web http helper
    /// </summary>
    public static class WebHttpHelper
    {
        /// <summary>
        /// Async get http data.
        /// </summary>
        /// <param name="url">request url.</param>
        /// <param name="encoding">encoding type.</param>
        /// <returns>Get url data result is string</returns>
        public static async Task<string> HttpGetAsync(string url, Encoding encoding = null)
        {
            HttpClient httpClient = new HttpClient();
            var data = await httpClient.GetByteArrayAsync(url);
            encoding = encoding ?? Encoding.UTF8;
            var ret = encoding.GetString(data);
            return ret;
        }

        /// <summary>
        /// Get http data.
        /// </summary>
        /// <param name="url">request url.</param>
        /// <param name="encoding">encoding type.</param>
        /// <returns>Get url data result is string</returns>
        public static string HttpGet(string url, Encoding encoding = null)
        {
            HttpClient httpClient = new HttpClient();
            var data = httpClient.GetByteArrayAsync(url);
            encoding = encoding ?? Encoding.UTF8;
            var ret = encoding.GetString(data.Result);
            return ret;
        }

        /// <summary>
        /// Async post data to url.
        /// </summary>
        /// <param name="url">request url</param>
        /// <param name="formData">formData,the key is string ,value is object,but the best choose if int or string</param>
        /// <param name="encoding">encoding default(UTF8)</param>
        /// <param name="timeOut">http request timeout.</param>
        /// <returns>response data of string</returns>
        public static async Task<string> HttpPostAsync(string url, Dictionary<string, object> formData = null,
            Encoding encoding = null, int timeOut = 3000)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            MemoryStream ms = new MemoryStream();
            //填充formData数据
            formData.FillFormDataStream(ms);
            HttpContent hc = new StreamContent(ms);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            hc.Headers.Add("UserAgent",
                "Mozilla/5.0(Window NT 6.1;WOW64) AppleWebKit/573.36 (KHTML,like Gecko) Chrome/31.0.1650.57 Safari/537.36");
            hc.Headers.Add("TimeOut", timeOut.ToString());
            hc.Headers.Add("KeepAlive", "true");

            var r = await client.PostAsync(url, hc);
            byte[] tmp = await r.Content.ReadAsByteArrayAsync();
            return (encoding ?? Encoding.UTF8).GetString(tmp);
        }

        /// <summary>
        /// Post data to url.
        /// </summary>
        /// <param name="url">request url</param>
        /// <param name="formData">formData,the key is string ,value is object,but the best choose if int or string</param>
        /// <param name="encoding">encoding default(UTF8)</param>
        /// <param name="timeOut">http request timeout.</param>
        /// <returns>response data of string</returns>
        public static string HttpPost(string url, Dictionary<string, object> formData = null,
            Encoding encoding = null, int timeOut = 3000)
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            MemoryStream ms = new MemoryStream();
            //填充formData数据
            formData.FillFormDataStream(ms);
            HttpContent hc = new StreamContent(ms);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            hc.Headers.Add("UserAgent",
                "Mozilla/5.0(Window NT 6.1;WOW64) AppleWebKit/573.36 (KHTML,like Gecko) Chrome/31.0.1650.57 Safari/537.36");
            hc.Headers.Add("TimeOut", timeOut.ToString());
            hc.Headers.Add("KeepAlive", "true");

            var r = client.PostAsync(url, hc);
            r.Wait();
            var tmp = r.Result.Content.ReadAsByteArrayAsync();
            return (encoding ?? Encoding.UTF8).GetString(tmp.Result);
        }

        /// <summary>
        /// <para>组装QueryString的方法</para>
        /// <para>参数之间用and连接，首位没有符号，如：a=1 and b=2 and c=3</para>
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        private static string GetQueryString(this Dictionary<string, object> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 填充表单信息的Stream
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="stream"></param>
        private static void FillFormDataStream(this Dictionary<string, object> formData, Stream stream)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : Encoding.UTF8.GetBytes(dataString);
            stream.Write(formDataBytes, 0, formDataBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);//设置指针读取位置
        }

        /// <summary>
        /// Post https request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string HttpsPost(string url, string postData)
        {
            HttpWebRequest request;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return string.Empty;
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request.ProtocolVersion = HttpVersion.Version11;
                // 这里设置了协议类型。
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;// SecurityProtocolType.Tls1.2; 
                request.KeepAlive = false;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = null;
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.Accept = "*/*";

            byte[] data = Encoding.UTF8.GetBytes(postData);
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            //获取网页响应结果
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            if (stream == Stream.Null || stream == null) return string.Empty;
            string result;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// Validation check method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
