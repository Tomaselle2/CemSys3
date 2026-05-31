using CemSys3.DTOs.PDF;
using Microsoft.Playwright;

namespace CemSys3.Helpers.PDF
{
    public class PlaywrightPdfGenerator
    {
        private readonly IBrowser _browser;

        public PlaywrightPdfGenerator(IBrowser browser)
        {
            _browser = browser;
        }

        //    public async Task<byte[]> GenerateFromHtmlAsync(string html, PdfOptionsDto? options = null)
        //    {
        //        options ??= new PdfOptionsDto();

        //        var page = await _browser.NewPageAsync();

        //        await page.SetContentAsync(html, new PageSetContentOptions
        //        {
        //            WaitUntil = WaitUntilState.NetworkIdle
        //        });

        //        var pdf = await page.PdfAsync(new PagePdfOptions
        //        {
        //            Format = options.Format,
        //            Landscape = options.Landscape,
        //            PrintBackground = true,
        //            Margin = new Margin
        //            {
        //                Top = options.MarginTop,
        //                Bottom = options.MarginBottom,
        //                Left = options.MarginLeft,
        //                Right = options.MarginRight
        //            }
        //        });

        //        await page.CloseAsync();

        //        return pdf;
        //    }
        //}

        public async Task<byte[]> GenerateFromHtmlAsync(
    string html,
    PdfOptionsDto? options = null)
        {
            options ??= new PdfOptionsDto();

            var page = await _browser.NewPageAsync();

            await page.SetContentAsync(html, new PageSetContentOptions
            {
                WaitUntil = WaitUntilState.Load
            });

            // Esperar que termine de cargar todo
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Esperar específicamente imágenes
            await page.WaitForFunctionAsync(
                @"() => Array.from(document.images)
            .every(img => img.complete)");

            // Aplicar media print
            await page.EmulateMediaAsync(new()
            {
                Media = Media.Print
            });

            // pequeño margen de seguridad
            await page.WaitForTimeoutAsync(500);

            var pdf = await page.PdfAsync(new PagePdfOptions
            {
                Format = options.Format,
                Landscape = options.Landscape,
                PrintBackground = true,
                Margin = new Margin
                {
                    Top = options.MarginTop,
                    Bottom = options.MarginBottom,
                    Left = options.MarginLeft,
                    Right = options.MarginRight
                }
            });

            await page.CloseAsync();

            return pdf;
        }

        internal class PagePdfMargin : Margin
        {
            public string Top { get; set; }
            public string Bottom { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }
        }
    }
}
