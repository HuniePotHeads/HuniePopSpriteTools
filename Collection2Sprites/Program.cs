using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Collection2Sprites
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int resolutionX = 72;
            int resolutionY = 72;
            bool unflippedAtlas = false;

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
            string textureFile = basePath + ".png";

            if (!File.Exists(textureFile))
            {
                textureFile = basePath + "_Unflipped.png";

                if (!File.Exists(textureFile))
                {
                    Console.WriteLine("Texture file not found:  " + textureFile);
                    return;
                }

                unflippedAtlas = true;
            }

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            Bitmap srcImage = Image.FromFile(textureFile) as Bitmap;

            if (unflippedAtlas)
            {
                srcImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            string[] lines = File.ReadAllLines(csvFile);
            Dictionary<string, List<SpriteInfo>> spriteSets = new Dictionary<string, List<SpriteInfo>>();
            int unnamed = 0;
            Dictionary<string, int> allRects = new Dictionary<string, int>();

            // Skipping the first line...
            for (int i = 1; i < lines.Length; i++)
            {
                SpriteInfo sprite = new SpriteInfo().FromCsv(lines[i]);

                if (string.IsNullOrEmpty(sprite.Name))
                {
                    sprite.Name = "unnamed_" + unnamed;
                    sprite.Unnamed = true;
                    unnamed++;
                }

                if (!spriteSets.ContainsKey(sprite.Name))
                {
                    spriteSets.Add(sprite.Name, new List<SpriteInfo>());
                }

                spriteSets[sprite.Name].Add(sprite);

                string rectString = string.Concat(sprite.Rect.X, "x", sprite.Rect.Y, ",", sprite.Rect.Width, "x", sprite.Rect.Height);
                if (!allRects.ContainsKey(rectString))
                {
                    allRects.Add(rectString, 1);
                }
                else
                {
                    //if (!sprite.Unnamed)
                    //{
                        allRects[rectString]++;
                    //    Console.WriteLine(string.Concat("Reused sprite at ", rectString, " (", sprite.Name, ")"));
                    //}
                }
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
                    Directory.CreateDirectory(outputDir);
                }

                foreach (SpriteInfo sprite in item.Value)
                {
                    string outputFile;

                    if (item.Value.Count > 1)
                    {
                        outputFile = Path.Combine(outputDir, sprite.Name) + "_" + sprite.UVIndex + ".png";
                    }
                    else
                    {
                        outputFile = Path.Combine(outputDir, sprite.Name) + ".png";
                    }

                    using (Bitmap bmp = GetCrop(srcImage, new Rectangle(sprite.Rect.X, sprite.Rect.Y, sprite.Rect.Width, sprite.Rect.Height)))
                    {
                        if (sprite.Flipped)
                        {
                            bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                        else
                        {
                            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }

                        bmp.SetResolution(resolutionX, resolutionY);
                        bmp.Save(outputFile, ImageFormat.Png);
                    }
                }
            }

            int identical = 0;

            foreach (KeyValuePair<string, int> item in allRects)
            {
                if (item.Value > 1)
                {
                    identical += item.Value;
                }
            }

            if (identical > 0)
            {
                Console.WriteLine(string.Concat( identical, " identical sprites were found!"));
                //Console.ReadKey(true);
            }
        }

        private static Bitmap GetCrop(Bitmap original, Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, original.PixelFormat);
            Rectangle destRect = new Rectangle(0, 0, rect.Width, rect.Height);
            Rectangle srcRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(original, destRect, srcRect, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:\n");
            Console.WriteLine("  " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " GirlBeliSpriteCollection.csv");
            Console.ReadKey(true);
        }
    }
}
