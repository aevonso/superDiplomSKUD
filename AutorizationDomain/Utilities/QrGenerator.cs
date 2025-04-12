using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace AutorizationDomain.Utilities
{
    public class QrGenerator
    {
        public static string Generate(string data)
        {
            var options = new QrCodeEncodingOptions
            {
                Height = 250, 
                Width = 250,  
                Margin = 1    
            };

            // Инициализируем BarcodeWriter
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            // Генерируем пиксельные данные для QR-кода
            var pixelData = writer.Write(data);

            // Создаем Bitmap и копируем пиксельные данные
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                                 ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
