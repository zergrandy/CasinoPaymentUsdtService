using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace QuoteLineBot1
{
    public static class Request
    {
        public static string FetchClient(string url, string apiKey)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("TRON-PRO-API-KEY", apiKey);

            //"https://apilist.tronscanapi.com/api/block"
            HttpResponseMessage response = client.GetAsync(url).Result;

            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            return responseBody;
        }

        public static string GetWebContent(string Url)
        {
            string strResult = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //聲明一個HttpWebRequest請求
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ServicePoint.Expect100Continue = false;//加快载入速度
            //request.ServicePoint.UseNagleAlgorithm = false;//禁止Nagle算法加快载入速度
            //request.AllowWriteStreamBuffering = false;//禁止缓冲加快载入速度
            //request.AllowAutoRedirect = false;//禁止自动跳转
            //設置連接逾時時間
            request.Headers.Set("Pragma", "no-cache");
            request.Headers.Set("Accept-Language", "en-US,en;q=0.8");
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamReceive = response.GetResponseStream();
            Encoding encoding = Encoding.GetEncoding(65001);
            StreamReader streamReader = new StreamReader(streamReceive, encoding);
            strResult = streamReader.ReadToEnd();
            response.Close();
            streamReceive.Close();
            streamReader.Close();

            return strResult;
        }

        public static string Post(string url, string pd)
        {
            //  ASCIIEncoding encoding = new ASCIIEncoding();
            Encoding encoding = Encoding.GetEncoding(65001);
            string postData = pd;
            byte[] data = encoding.GetBytes(postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ServicePoint.Expect100Continue = false;//加快载入速度
            request.ServicePoint.UseNagleAlgorithm = false;//禁止Nagle算法加快载入速度
                                                           //request.AllowWriteStreamBuffering = false;//禁止缓冲加快载入速度
            request.AllowAutoRedirect = false;//禁止自動跳轉
                                              //request.KeepAlive = true;//啟用長連接
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            WebResponse response = request.GetResponse();
            stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, encoding);
            string web = sr.ReadToEnd().ToString();
            response.Close();
            stream.Close();
            sr.Close();
            return web;
        }

    }
    
}