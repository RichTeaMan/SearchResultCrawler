using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SearchResultCrawler
{
    public class CMS
    {
        public enum CMSName
        {
            Drupal,
            Joomla,
            Typo3,
            Wordpress,
            DotNetNuke,
            Sitecore,
            Umbraco,
            Unknown
        }

        public enum CMSLanguage
        {
            Net,
            PHP,
            Java,
            Python,
            Unknown
        }

        public string Version { get; private set; }
        public CMSName Name { get; private set; }
        public CMSLanguage Language { get; private set; }

        public CMS(string html)
        {
            // check Drupal
            Version = "Unknown";

            if (html.Contains("<meta name=\"Generator\" content=\"Drupal 7 (http://drupal.org)\">"))
            {
                Name = CMSName.Drupal;
                Language = CMSLanguage.PHP;
            }
            else if (html.Contains("<meta name=\"generator\" content=\"WordPress"))
            {
                Name = CMSName.Wordpress;
                Language = CMSLanguage.PHP;
                Regex regex = new Regex("@(?<=\\<meta name=\\\"generator\\\" content=\\\"WordPress )[\\d\\.]+");
                var match = regex.Match(html);
                if (match.Success)
                    Version = match.Value;
            }
            else if (html.Contains("<meta id=\"MetaGenerator\" name=\"GENERATOR\" content=\"DotNetNuke \">"))
            {
                Name = CMSName.DotNetNuke;
                Language = CMSLanguage.Net;
            }
            else
                Name = CMSName.Unknown;
        }

        public override string ToString()
        {
            return String.Format("{0} Version: {1}", System.Enum.GetName(typeof(CMSName), Name), Version);
        }
    }
}
