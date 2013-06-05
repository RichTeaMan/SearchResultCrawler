using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchResultCrawler
{
    public class WebPage
    {
        private ConcurrentBag<string> resourceAddresses;
        public string[] ResourceAddresses
        {
            get { return resourceAddresses.ToArray(); }
        }

        public ConcurrentDictionary<string, Resource> Resources;
        public string RootUrl { get { return rootUrl.ToString(); } }
        private Uri rootUrl;
        public string Html { get; private set; }
        public WebPage(string url)
        {
            rootUrl = new Uri(url);
            resourceAddresses = new ConcurrentBag<string>();
            Resources = new ConcurrentDictionary<string, Resource>();
            resourceAddresses.Add(url);
            var root = new Resource(url);
            if(root.StatusCode == HttpStatusCode.OK)
            {
                Html = ASCIIEncoding.ASCII.GetString(root.Data);
                var document = new HtmlDocument();
                document.LoadHtml(Html);
                FillResources(document.DocumentNode);
            }
        }

        public WebPage GetResources()
        {
            ParallelOptions op = new ParallelOptions();
            op.MaxDegreeOfParallelism = 40;
            Parallel.ForEach(resourceAddresses, op, addr =>
            {
                try
                {
                    var resource = new Resource(addr);
                    Resources.TryAdd(addr, resource);
                }
                catch { }
            });

            return this;
        }

        public CMS GetCMS()
        {
            return new CMS(Html);
        }

        public bool IsResponsive()
        {
            return Resources.Where(r => r.Value.ContentType == "text/css" || r.Value.ContentType == "text/html").Select(r => r.Value.ToString()).Any(r => r.Contains("@media"));
            
        }

        public WebHeaderCollection GetHeaders()
        {
            return Resources[rootUrl.ToString()].Headers;
        }

        public string GetServer()
        {
            var server = GetHeaders().GetValues("Server");
            if (server == null)
                return string.Empty;
            else
                return server.First();
        }

        public int GetPageWeight()
        {
            return Resources.Where(r => r.Value != null).Sum(r => r.Value.ContentLength);
        }

        private void FillResources(HtmlNode htmlNode)
        {
            HtmlAttribute attribute = null;
            switch(htmlNode.OriginalName)
            {
                case "img":
                case "script":
                    attribute = htmlNode.Attributes.FirstOrDefault(a => a.OriginalName == "src");
                    break;
                case "link":
                    var rel = htmlNode.Attributes.FirstOrDefault(a => a.OriginalName == "rel");
                    if(rel != null && rel.Value == "stylesheet")
                    {
                        attribute = htmlNode.Attributes.FirstOrDefault(a => a.OriginalName == "href");
                    }
                    break;
                default:
                    break;
            }
            if(attribute != null)
            {
                Uri uri = null;
                try
                {
                    uri = new Uri(rootUrl, attribute.Value);
                }
                catch
                {
                    try
                    {
                        uri = new Uri(attribute.Value);
                    }
                    catch { }
                }

                if (uri != null && uri.IsAbsoluteUri)
                {
                    if (!resourceAddresses.Contains(uri.ToString()))
                        resourceAddresses.Add(uri.ToString());
                }
            }
            Parallel.ForEach(htmlNode.ChildNodes, child =>
            {
                FillResources(child);
            });
        }

    }
}
