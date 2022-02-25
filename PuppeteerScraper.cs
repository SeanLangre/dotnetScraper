
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using PuppeteerSharp;


namespace ScraperSoftware
{
    public class PuppeteerScraper
    {
        public async Task<string> StartScraper()
        {
            string fullUrl = "https://en.wikipedia.org/wiki/List_of_programmers";

            List<string> programmerLinks = new List<string>();

            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
            };
            var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(fullUrl);
            var links = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
            var urls = await page.EvaluateExpressionAsync<string[]>(links);


            foreach (string url in urls)
            {
                programmerLinks.Add(url);
            }

            WriteToCsv(programmerLinks);

            return "";
        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText("links.csv", sb.ToString());
        }
    }
}