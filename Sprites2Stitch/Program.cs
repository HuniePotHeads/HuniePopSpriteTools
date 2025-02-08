using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sprites2Stitch
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*if (args.Length < 1)
            {
                Console.WriteLine("No directory specified");
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Directory does not exist");
                return;
            }

            string sourceDir = args[0];*/

            string baseDir = ".";
            int resolutionX = 72;
            int resolutionY = 72;

            foreach (string subDir in Directory.EnumerateDirectories(baseDir))
            {
                Bitmap destBitmap;
                int destWidth;
                int destHeight;

                if (subDir.ToLower().Contains("background"))
                {
                    destWidth = 1264;
                    destHeight = 948;
                }
                else
                {
                    destWidth = 1200;
                    destHeight = 900;
                }

                foreach (string subDir2 in Directory.EnumerateDirectories(subDir))
                {
                    int destX = 0;
                    int destY = destHeight;
                    int previousWidth = 0;
                    List<string> sourceFiles;

                    try
                    {
                        sourceFiles = Directory.EnumerateFiles(subDir2, "*.png")
                            .OrderBy(f => int.Parse(Regex.Match(f, @"\d+", RegexOptions.RightToLeft).Value)).ToList();
                    }
                    catch (FormatException)
                    {
                        sourceFiles = Directory.EnumerateFiles(subDir2, "*.png").ToList();
                    }

                    if (sourceFiles.Count < 1)
                    {
                        continue;
                    }

                    Console.WriteLine(subDir2 + ".png");
                    destBitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
                    //destBitmap = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb);

                    foreach (string srcFile in sourceFiles)
                    {
                        Bitmap srcBitmap = (Bitmap)Image.FromFile(srcFile);

                        if (srcFile.ToLower().Contains("flipped"))
                        {
                            srcBitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                        }

                        destY -= srcBitmap.Height;

                        if (destY < 0)
                        {
                            destX += previousWidth;
                            destY = destHeight - srcBitmap.Height;
                        }

                        previousWidth = srcBitmap.Width;

                        using (Graphics g = Graphics.FromImage(destBitmap))
                        {
                            var destRect = new Rectangle(destX, destY, srcBitmap.Width, srcBitmap.Height);
                            var srcRect = new Rectangle(0, 0, srcBitmap.Width, srcBitmap.Height);
                            g.DrawImage(srcBitmap, destRect, srcRect, GraphicsUnit.Pixel);
                        }

                        srcBitmap.Dispose();
                    }

                    destBitmap.SetResolution(resolutionX, resolutionY);
                    destBitmap.Save(subDir2 + ".png", ImageFormat.Png);
                    destBitmap.Dispose();
                }
            }
        }
    }
}
