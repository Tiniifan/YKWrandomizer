using System.Drawing;

namespace YKWrandomizer.Tools
{
    public class Draw
    {
        public static Bitmap DrawImage(Bitmap bmp, int x, int y, Image imagePath)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(imagePath, new Point(x, y));
            }
            return bmp;
        }

        public static Bitmap DrawImage(Bitmap bmp, int x, int y, Image imagePath, int width, int height, bool drawOnTop)
        {
            Bitmap newImage = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                if (!drawOnTop)
                {
                    g.DrawImage(bmp, 0, 0);
                }

                // Calculate the proportional dimensions
                int newWidth = width;
                int newHeight = (int)((float)height / imagePath.Width * imagePath.Height);

                if (newHeight > bmp.Height)
                {
                    newHeight = bmp.Height;
                    newWidth = (int)((float)width / imagePath.Height * imagePath.Width);
                }

                g.DrawImage(imagePath, new Rectangle(x, y, newWidth, newHeight));

                if (drawOnTop)
                {
                    g.DrawImage(bmp, 0, 0);
                }
            }

            return newImage;
        }
    }
}
