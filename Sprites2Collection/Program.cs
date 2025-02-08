using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sprites2Collection
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int resolutionX = 72;
            int resolutionY = 72;
            bool saveAtlasUnflipped = true;

            if (args.Length < 1)
            {
                Console.WriteLine("No path specified.");
                ShowUsage();
                return;
            }

            if (!File.Exists(args[0]))
            {

                if (Directory.Exists(args[0]))
                {
                    Console.WriteLine("Path is a directory");
                    return;
                }

                Console.WriteLine("Path does not exist");
                return;
            }

            string csvFile = Path.GetFullPath(args[0]);
            string basePath = Path.Combine(Path.GetDirectoryName(csvFile), Path.GetFileNameWithoutExtension(csvFile));
            string originalFile = basePath + ".png";
            string destFile = saveAtlasUnflipped ? basePath + "_NEW_Unflipped.png" : basePath + "_NEW.png";
            Bitmap destImage;

            if (!File.Exists(originalFile))
            {
                if (!File.Exists(basePath + "_Unflipped.png"))
                {
                    Console.WriteLine("Texture file not found:  " + originalFile);
                    Console.WriteLine("Texture file not found:  " + basePath + "_Unflipped.png");
                    return;
                }

                originalFile = basePath + "_Unflipped.png";
            }

            if (!Directory.Exists(basePath))
            {
                //Directory.CreateDirectory(basePath);
                Console.WriteLine("Directory not found:  " + basePath);
                return;
            }

            using (Bitmap original = (Bitmap)Image.FromFile(originalFile))
            {
                destImage = new Bitmap(original.Width, original.Height, original.PixelFormat);
            }

            string[] lines = File.ReadAllLines(csvFile);
            Dictionary<string, List<SpriteInfo>> spriteSets = new Dictionary<string, List<SpriteInfo>>();
            int unnamed = 0;

            // Skipping the first line...
            for (int i = 1; i < lines.Length; i++)
            {
                SpriteInfo sprite = new SpriteInfo().FromCsv(lines[i]);

                if (string.IsNullOrEmpty(sprite.Name))
                {
                    sprite.Name = "unnamed_" + unnamed;
                    unnamed++;
                }

                if (!spriteSets.ContainsKey(sprite.Name))
                {
                    spriteSets.Add(sprite.Name, new List<SpriteInfo>());
                }

                spriteSets[sprite.Name].Add(sprite);
            }

            foreach (KeyValuePair<string, List<SpriteInfo>> item in spriteSets)
            {
                string outputDir;

                if (item.Value.Count > 1)
                {
                    outputDir = Path.Combine(basePath, item.Key);
                    Console.WriteLine(string.Concat(item.Value[0].CollectionName, @"\", item.Key, ": ", item.Value.Count, " pieces"));
                }
                else
                {
                    outputDir = basePath;
                    Console.WriteLine(string.Concat(item.Value[0].CollectionName, @"\", item.Key));
                }

                if (!Directory.Exists(outputDir))
                {
                    //Directory.CreateDirectory(outputDir);
                    Console.WriteLine("Directory not found:  " + outputDir);
                    return;
                }

                foreach (SpriteInfo sprite in item.Value)
                {
                    string srcFile;

                    if (item.Value.Count > 1)
                    {
                        srcFile = Path.Combine(outputDir, sprite.Name) + "_" + sprite.UVIndex + ".png";
                    }
                    else
                    {
                        srcFile = Path.Combine(outputDir, sprite.Name) + ".png";
                    }

                    if (!File.Exists(srcFile))
                    {
                        Console.WriteLine("File not found:  " + srcFile);
                        return;
                    }

                    using (Bitmap srcImage = (Bitmap)Image.FromFile(srcFile))
                    {
                        if (sprite.Flipped)
                        {
                            srcImage.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                        else
                        {
                            srcImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }

                        string name = item.Value[0].CollectionName.ToLower();

                        if (name.Contains("background") || name.Contains("photo"))
                        {
                            using (Bitmap fillerImage = ResizeImage(srcImage, sprite.Rect.Width + 2, sprite.Rect.Height + 2))
                            {
                                Paste(fillerImage, destImage, new Rectangle(sprite.Rect.X - 1, sprite.Rect.Y - 1, fillerImage.Width, fillerImage.Height));
                            }
                        }

                        Paste(srcImage, destImage, new Rectangle(sprite.Rect.X, sprite.Rect.Y, sprite.Rect.Width, sprite.Rect.Height));
                    }
                }
            }

            if (saveAtlasUnflipped)
            {
                destImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            destImage.SetResolution(resolutionX, resolutionY);
            destImage.Save(destFile, ImageFormat.Png);
        }

        private static void Paste(Bitmap original, Bitmap dest, Rectangle destRect)
        {
            Rectangle srcRect = new Rectangle(0, 0, destRect.Width, destRect.Height);

            using (Graphics g = Graphics.FromImage(dest))
            {
                g.DrawImage(original, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("  " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " GirlBeliSpriteCollection.csv");
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
