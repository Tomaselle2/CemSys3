using CemSys3.DTOs.PDF;
using Microsoft.Playwright;

namespace CemSys3.Helpers.PDF
{
    public class PlaywrightPdfGenerator
    {
        public async Task<byte[]> GenerateFromHtmlAsync(
            string html,
            PdfOptionsDto? options = null)
        {
            options ??= new PdfOptionsDto();

            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true });

            var page = await browser.NewPageAsync();

            await page.SetContentAsync(html, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            return await page.PdfAsync(new PagePdfOptions
            {
                Format = options.Format,
                Landscape = options.Landscape,
                Margin = new PagePdfMargin
                {
                    Top = options.MarginTop,
                    Bottom = options.MarginBottom,
                    Left = options.MarginLeft,
                    Right = options.MarginRight
                },
                PrintBackground = true
            });
        }
    }

    internal class PagePdfMargin : Margin
    {
        public string Top { get; set; }
        public string Bottom { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
    }
}
