using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stitch2Sprites
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string baseDir = ".";
            int resolutionX = 72;
            int resolutionY = 72;

            foreach (string subDir in Directory.EnumerateDirectories(baseDir))
            {
                Bitmap srcImage;
                int srcWidth;
                int srcHeight;
                int expectedWidth;
                int expectedHeight;

                if (subDir.ToLower().Contains ("background"))
                {
                    expectedWidth = 1264;
                    expectedHeight = 948;
                }
                else
                {
                    expectedWidth = 1200;
                    expectedHeight = 900;
                }

                foreach (string subDir2 in Directory.EnumerateDirectories(subDir))
                {
                    string srcFile = subDir2 + ".png";
                    string destDir = subDir2; // + "_Unstitched";

                    if (!File.Exists(srcFile))
                    {
                        srcFile = subDir2 + ".jpg";

                        if (!File.Exists(srcFile))
                        {
                            Console.WriteLine(subDir2 + ".png or " + subDir2 + ".jpg do not exist");
                            continue;
                        }
                    }

                    Console.WriteLine(srcFile);

                    srcImage = Image.FromFile(srcFile) as Bitmap;

                    if (srcImage.Width != expectedWidth || srcImage.Height != expectedHeight)
                    {
                        Console.WriteLine("Different image dimensions, resizing...");
                        srcImage = ResizeImage(srcImage, expectedWidth, expectedHeight);
                    }

                    srcWidth = srcImage.Width;
                    srcHeight = srcImage.Height;

                    int srcX = 0;
                    int srcY = srcHeight;
                    int previousWidth = 0;

                    IOrderedEnumerable<string> spriteFiles = Directory.EnumerateFiles(subDir2, "*.png")
                        .OrderBy(f => int.Parse(Regex.Match(f, @"\d+", RegexOptions.RightToLeft).Value));

                    if (spriteFiles.Count() < 1)
                    {
                        return;
                    }

                    if (!Directory.Exists(destDir))
                    {
                        //Directory.CreateDirectory(destDir);
                        return;
                    }

                    foreach (string spriteFile in spriteFiles)
                    {
                        Bitmap spriteBitmap = Image.FromFile(spriteFile) as Bitmap;
                        bool flipped = false;

                        if (spriteFile.ToLower().Contains("flipped"))
                        {
                            spriteBitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                            flipped = true;
                        }

                        srcY -= spriteBitmap.Height;

                        if (srcY < 0)
                        {
                            srcX += previousWidth;
                            srcY = srcHeight - spriteBitmap.Height;
                        }

                        Rectangle destRect = new Rectangle(0, 0, spriteBitmap.Width, spriteBitmap.Height);
                        Rectangle srcRect = new Rectangle(srcX, srcY, spriteBitmap.Width, spriteBitmap.Height);

                        using (Bitmap destBitmap = new Bitmap(destRect.Width, destRect.Height, srcImage.PixelFormat))
                        {
                            using (Graphics g = Graphics.FromImage(destBitmap))
                            {
                                g.DrawImage(srcImage, destRect, srcRect, GraphicsUnit.Pixel);
                            }

                            if (flipped)
                            {
                                destBitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                            }

                            previousWidth = spriteBitmap.Width;
                            spriteBitmap.Dispose();

                            destBitmap.SetResolution(resolutionX, resolutionY);
                            destBitmap.Save(Path.Combine(destDir, Path.GetFileName(spriteFile)), ImageFormat.Png);
                        }

                        //previousWidth = spriteBitmap.Width;
                        //spriteBitmap.Dispose();
                    }

                    srcImage.Dispose();
                }
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage (Image image, int width, int height)
        {
            var destRect = new Rectangle (0, 0, width, height);
            var destImage = new Bitmap (width, height);

            destImage.SetResolution (image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage (destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes ())
                {
                    wrapMode.SetWrapMode (WrapMode.TileFlipXY);
                    graphics.DrawImage (image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
