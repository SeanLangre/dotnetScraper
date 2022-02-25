using System;
using System.Threading.Tasks;
using ScraperSoftware;

namespace WebScraping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // var scraper = new NoPuppeteerScraper();
            var scraper = new PuppeteerScraper();
            var asd = await scraper.StartScraper();
            Console.WriteLine("-Done-");


        }
    }
}
