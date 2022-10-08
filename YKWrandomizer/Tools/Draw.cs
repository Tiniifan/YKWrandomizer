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
    }
}
