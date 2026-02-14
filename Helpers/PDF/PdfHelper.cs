using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;

namespace CemSys3.Helpers.PDF
{
    public class PdfHelper
    {
        public static byte[] ImagenComoPdf(byte[] imagenBytes)
        {
            //1) Corregir orientación EXIF automáticamente
            using var image = SixLabors.ImageSharp.Image.Load(imagenBytes);

            image.Mutate(x => x.AutoOrient());

            using var correctedStream = new MemoryStream();

            image.Save(correctedStream, new JpegEncoder
            {
                Quality = 90
            });

            var imagenCorregida = correctedStream.ToArray();

            //2) Crear PDF
            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var imageData = ImageDataFactory.Create(imagenCorregida);
            var img = new iText.Layout.Element.Image(imageData);

            img.SetAutoScale(true);

            document.Add(img);
            document.Close();

            return ms.ToArray();
        }
    }
}
