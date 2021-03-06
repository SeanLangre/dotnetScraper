
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using PuppeteerSharp;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

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

            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "data.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                json = json.Trim();
                var items2 = json;
                var items3 = JsonConvert.SerializeObject(new ItemHolder() { items2 = new Item[] { new Item(), new Item(), new Item() } });
                var itemHolder = JsonConvert.DeserializeObject<ItemHolder>(json);
                Console.Write("");
            }

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

            var tasks = new List<Task<InfoElement>>();
            foreach (var item in list)
            {
                var task = GetInfo(item);
                tasks.Add(task);
            }

            var result = await Task.WhenAll(tasks);

            WriteToCsv(result.Select(r => r.ToString()).ToList());

            return "";
        }

        private async Task<InfoElement> GetInfo(ElementHandle element)
        {
            var title = await element.QuerySelectorAsync("a");
            var realTitle = (await title.GetPropertyAsync("title")).RemoteObject.Value.ToString();

            var link = await element.QuerySelectorAsync("a");
            var realLink = (await link.GetPropertyAsync("href")).RemoteObject.Value.ToString();

            var price = await element.QuerySelectorAsync(".item-card-details-price");
            var realPrice = (await price.GetPropertyAsync("textContent")).RemoteObject.Value.ToString();

            var info = new InfoElement(realTitle, realLink, realPrice);
            return info;
        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText($"links.csv", sb.ToString());
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

            public string ToString()
            {
                return $"{this.title} {this.link} {this.price}";
            }
        }

        [Serializable]
        public class ItemHolder
        {
            public Item[] items1;
            public Item[] items2;
        }

        [Serializable]
        public class Item
        {
            public string searchterm;
            public string[] keywords;
        }
    }
}