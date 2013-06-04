using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SearchResultCrawler
{
    public class GoogleSearchQuery
    {
        public static string[] GetSearchResult(string searchTerm)
        {
            string requestString = String.Format("https://www.googleapis.com/customsearch/v1?key={0}&q={1}&cx={2}",
                Keys.GOOGLE_API_KEY,
                searchTerm,
                Keys.SEARCH_ENGINE_ID);
            
            WebClient client = new WebClient();
            var data = client.DownloadString(requestString);
            var results = JObject.Parse(data);
            List<string> links = new List<string>();
            foreach (var item in results["items"])
            {
                string link = (string)item["link"];
                links.Add(link);
            }
            return links.ToArray();
        }
    }
}
