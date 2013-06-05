using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SearchResultCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            // should make http web requests faster. I'm unconvinced
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.CheckCertificateRevocationList = false;

            Console.WriteLine("Beginning Google Search.");
            var results = GoogleSearchQuery.GetSearchResult("wordpress");
            Console.WriteLine("{0} results returned.", results.Count());
            foreach (var res in results)
            {
                Console.WriteLine("Requesting {0}:", res);
                var page = new WebPage(res);

                Console.WriteLine("Page weight: {0}", page.GetResources().GetPageWeight());
                if (page.IsResponsive())
                    Console.WriteLine("Is responsive.");
                else
                    Console.WriteLine("Is not resonsive.");

                Console.WriteLine("Server: {0}", page.GetServer());
                Console.WriteLine(page.GetCMS());

            }
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}
