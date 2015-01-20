using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TalentSpider
{
    class Program
    {
        static Assembly ass = Assembly.GetExecutingAssembly();
        static string path = Path.Combine(Path.GetDirectoryName(ass.Location), "DataFiles");
        static string URLS = "Companyurls.csv";
        static string JobsPages = "JobsPages.csv";
        static string PositionTitles = "PositionTitles.csv";

        static List<string> JobsUrls = new List<string>();

        static void Main(string[] args)
        {
            foreach (string url in File.ReadAllLines(Path.Combine(path, URLS)))
            {
                HtmlAgilityPack.HtmlDocument rootSite = getSiteContent(url);
                foreach (HtmlNode link in rootSite.DocumentNode.SelectNodes("//a[@href]"))
                {
                    // Get the value of the HREF attribute
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    hrefValue = FixRelativePaths(url, hrefValue);


                    if (ContainsJobsPages(hrefValue, link.InnerText))
                    {
                        string PositionURL = PageHasPositionTitle(url, hrefValue);
                        if (!String.IsNullOrEmpty(PositionURL))
                        {
                            ReviewPositionForSkills(PositionURL);
                        }
                        else
                        {
                            JobsUrls.Add(hrefValue);
                        }
                    }
                }
            }
        }

        private static string FixRelativePaths(string root, string hrefValue)
        {
            if (!hrefValue.StartsWith("htt"))
            {
                Uri baseUri = new Uri(root);
                Uri myUri = new Uri(baseUri, hrefValue);
                hrefValue = myUri.AbsoluteUri;
            }
            return hrefValue;
        }

        private static bool ReviewPositionForSkills(string url)
        {
            HtmlAgilityPack.HtmlDocument page = getSiteContent(url);


            return false;

        }

        private static string PageHasPositionTitle(string root, string url)
        {
            string titleURL = "";
            HtmlAgilityPack.HtmlDocument page = getSiteContent(url);

            foreach (string title in File.ReadAllLines(Path.Combine(path, PositionTitles)))
            {
                if (page.ToString().ToLower().Contains(title.ToLower()))
                {
                    if (!ReviewPositionForSkills(url))
                    {
                        foreach (HtmlNode link in page.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            if (link.InnerText.ToLower() == title.ToLower())
                            {
                                titleURL = link.GetAttributeValue("href", string.Empty);
                                titleURL = FixRelativePaths(root, titleURL);
                            }
                        }
                    }
                }
            }

            return titleURL;
        }

        public static bool ContainsJobsPages(string url, string LinkText)
        {
            foreach (string jobterm in File.ReadAllLines(Path.Combine(path, JobsPages)))
            {
                if (LinkText.ToLower().Contains(jobterm.ToLower()))
                {
                    JobsUrls.Add(url);
                    break;
                }
                else if (url.Contains(jobterm))
                {
                    return true;
                }
            }
            return false;
        }

        private static HtmlAgilityPack.HtmlDocument getSiteContent(string url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument page = new HtmlAgilityPack.HtmlDocument();

            page = hw.Load(url);
            return page;
        }



    }
}
