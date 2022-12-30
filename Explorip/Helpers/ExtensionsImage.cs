using System.Drawing;
using System.Drawing.Imaging;

namespace Explorip.Helpers
{
    public static class ExtensionsImage
    {
        public static Bitmap ChangeCouleur(this Bitmap image, Color ancienneCouleur, Color nouvelleColeur)
        {
            Bitmap retour = new(image.Width, image.Height);
            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap()
            {
                OldColor = ancienneCouleur,
                NewColor = nouvelleColeur,
            };
            ImageAttributes attr = new();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new(0, 0, image.Width, image.Height);
            Graphics g = Graphics.FromImage(retour);
            g.DrawImage(image, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);
            return retour;
        }
    }
}
