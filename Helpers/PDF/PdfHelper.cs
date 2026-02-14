using System.Reflection.Metadata;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using static System.Net.Mime.MediaTypeNames;

namespace CemSys3.Helpers.PDF
{
    public class PdfHelper
    {
        public static byte[] ImagenComoPdf(byte[] imagenBytes)
        {
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);

            var imageData = ImageDataFactory.Create(imagenBytes);
            var img = new iText.Layout.Element.Image(imageData);
            img.SetAutoScale(true); // Ajusta sin distorsionar

            document.Add(img);
            document.Close();

            return ms.ToArray();
        }
    }
}
