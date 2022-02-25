
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using PuppeteerSharp;
using System;
using System.Linq;

namespace ScraperSoftware
{
    public class PuppeteerScraper
    {

        private string actionType = "Auction";
        private string sortBy = "sortBy=TimeLeft";
        private string linkPrefix = "www.tradera.com";

        private string getURL(string name, string actionType, string sortBy)
        {
            return $"https://www.tradera.com/search?q={ name }&itemType={ actionType }&{ sortBy }";
        }

        public async Task<string> StartScraper()
        {
            List<string> programmerLinks = new List<string>();

            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
            };
            var browser = await Puppeteer.LaunchAsync(options);
            var page = await browser.NewPageAsync();

            string fullURL = getURL("arkham", actionType, sortBy);
            Console.WriteLine(fullURL);
            await page.GoToAsync(fullURL);


            var asd = await page.WaitForSelectorAsync(".site-pagename-SearchResults ");

            //page down
            for (int i = 0; i < 20; i++)
            {
                await page.Keyboard.PressAsync("PageDown");
                await page.WaitForTimeoutAsync(100);
            }

            var list = await page.QuerySelectorAllAsync(".item-card-container");

            //var results = await Task.WhenAll(list.Select(item => item.GetPropertyAsync("outerHTML")));
            //var htmlList = await Task.WhenAll(results.Select(item => item.JsonValueAsync()));

            var tasks = new List<Task<InfoElement>>();
            foreach (var item in list)
            {
                var task = GetInfo(item);
                tasks.Add(task);
            }

            var result = await Task.WhenAll(tasks);

            return "";
        }

        private async Task<InfoElement> GetInfo(ElementHandle element)
        {
            var title = await element.QuerySelectorAsync("a");
            var realTitle = await title.GetPropertyAsync("title");

            var link = await element.QuerySelectorAsync("a");
            var realLink = linkPrefix + await link.GetPropertyAsync("href");

            var price = await element.QuerySelectorAsync(".item-card-details-price");
            var realPrice = await price.GetPropertyAsync("textContent");

            var info = new InfoElement(realTitle.ToString(), realLink.ToString(), realPrice.ToString());
            return info;
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

        private class InfoElement
        {
            string title, link, price;
            public InfoElement(string title, string link, string price)
            {
                this.title = title;
                this.link = link;
                this.price = price;
            }

            public string ToString() {
                return this.title + this.link + this.price;
            }
        }
    }
}