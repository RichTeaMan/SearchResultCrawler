using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SearchResultCrawler
{
    public class Resource
    {
        public byte[] Data { get; private set; }
        public int ContentLength { get { return Data.Length; } }
        public HttpStatusCode StatusCode {get; private set;}
        public string ContentType { get; private set; }
        public WebHeaderCollection Headers;


        public Resource(string url)
        {
            //Console.WriteLine("Fetching {0}", url);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "TomBot";
            request.Proxy = null;
            request.Timeout = 10 * 1000; // 10 second timeout
            var response = (HttpWebResponse)request.GetResponse();
            StatusCode = response.StatusCode;

            var stream = response.GetResponseStream();

            MemoryStream memoryStream = new MemoryStream();
            using (Stream responseStream = request.GetResponse().GetResponseStream())
            {
                byte[] buffer = new byte[0x1000];
                int bytes;
                while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytes);
                }
            }

            Data = memoryStream.ToArray();
            ContentType = response.ContentType;
            Headers = response.Headers;
            response.Close();
        }

        public override string ToString()
        {
            if (ContentType.StartsWith("text"))
            {
                return ASCIIEncoding.ASCII.GetString(Data);
            }
            else
            {
                return ContentType;
            }
        }
    }
}
