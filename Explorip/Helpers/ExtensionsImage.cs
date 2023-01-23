using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Explorip.Helpers
{
    public static class ExtensionsImage
    {
        public static Bitmap ChangeCouleur(this Bitmap image, Color ancienneCouleur, Color nouvelleColeur)
        {
            Bitmap retour = new(image.Width, image.Height);
            ColorMap[] colorMap = new ColorMap[2];
            colorMap[0] = new ColorMap()
            {
                OldColor = ancienneCouleur,
                NewColor = nouvelleColeur,
            };
            colorMap[1] = new ColorMap()
            {
                OldColor = Color.Transparent,
                NewColor = WindowsSettings.IsWindowsApplicationInDarkMode() ? Color.Black : Color.White,
            };
            ImageAttributes attr = new();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new(0, 0, image.Width, image.Height);
            Graphics g = Graphics.FromImage(retour);
            g.DrawImage(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            return retour;
        }

        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            using MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            System.Windows.Media.Imaging.BitmapImage result = new();
            result.BeginInit();
            result.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();
            return result;
        }
    }
}
