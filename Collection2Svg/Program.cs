using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Collection2Svg
{
    class MainClass
    {
        public static void Main(string[] args)
        {
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
                    Console.WriteLine("Path is a directory: {0}", args[0]);
                    return;
                }

                Console.WriteLine("Path does not exist: {0}", args[0]);
                return;
            }

            string srcFile = Path.GetFullPath(args[0]);
            string basePath = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile));
            string srcFile2 = basePath + "_GirlPieceInfo.min.json";
            string destFile = basePath + ".svg";

            if (!File.Exists(srcFile2))
            {
                Console.WriteLine("Path does not exist: {0}", srcFile2);
                return;
            }

            List<string> svgData = new List<string>();
            List<string> svgDataUses = new List<string>();
            List<string> svgDataDefs = new List<string>();

            string[] csvLines = File.ReadAllLines(srcFile);
            string jsonText = File.ReadAllText(srcFile2, Encoding.UTF8);
            GirlPieceInfo girlPieceInfo = JsonConvert.DeserializeObject<GirlPieceInfo>(jsonText);

            Dictionary<string, SpriteInfo> spriteInfos = new Dictionary<string, SpriteInfo>();
            int unnamed = 0;

            // Skipping the first line...
            for (int i = 1; i < csvLines.Length; i++)
            {
                SpriteInfo sprite = new SpriteInfo().FromCsv(csvLines[i]);

                if (string.IsNullOrEmpty(sprite.Name))
                {
                    sprite.Name = "unnamed_" + unnamed;
                    unnamed++;
                }

                if (!spriteInfos.ContainsKey(sprite.Name))
                {
                    spriteInfos.Add(sprite.Name, sprite);
                }
            }

            foreach (PieceArt art in girlPieceInfo.PieceArt)
            {
                if (spriteInfos.ContainsKey(art.spriteName))
                {
                    Rectangle oldRect = spriteInfos[art.spriteName].Rect;

                    if (spriteInfos[art.spriteName].Flipped)
                    {
                        spriteInfos[art.spriteName].Rect = new Rectangle(art.x, art.y, oldRect.Height, oldRect.Width);
                    }
                    else
                    {
                        spriteInfos[art.spriteName].Rect = new Rectangle(art.x, art.y, oldRect.Width, oldRect.Height);
                    }
                }
            }

            foreach (KeyValuePair<string, SpriteInfo> item in spriteInfos)
            {
                svgDataUses.Add(string.Format(
                    "  <!--<use xlink:href=\"#{2}\" x=\"{0}px\" y=\"{1}px\"/>-->",
                    item.Value.Rect.X, item.Value.Rect.Y, item.Value.Name)
                );

                svgDataDefs.Add(string.Format(
                    "    <g id=\"{0}\"><image width=\"{1}px\" height=\"{2}px\" xlink:href=\"{3}.png\"/></g>",
                    item.Value.Name, item.Value.Rect.Width, item.Value.Rect.Height, item.Value.Name)
                );
            }

            if (!basePath.ToLower().Contains("background") && !basePath.ToLower().Contains("photo"))
            {
                svgDataUses.Sort();
                svgDataDefs.Sort();
            }

            svgData.Add("<?xml version=\"1.0\" standalone=\"no\"?>");
            svgData.Add("<svg width=\"1200px\" height=\"900px\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
            svgData.AddRange(svgDataUses);
            svgData.Add("  <defs>");
            svgData.AddRange(svgDataDefs);
            svgData.Add("  </defs>");
            svgData.Add("</svg>");

            File.WriteAllLines(destFile, svgData);

            //Console.ReadKey(true);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Example:\n");
            Console.WriteLine("  " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " GirlBeliSpriteCollection.csv");
        }

        // https://gist.github.com/AndrewBarfield/2557343#file-gistfile1-cs
        /*private void Base64Encode()
        {
            Image a = new Bitmap(@".../path/to/image.png");

            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                a.Save(ms, a.RawFormat);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                this.richTextBox1.Text = "<img src=\"data:image/png;base64," + Convert.ToBase64String(imageBytes) + "\"/>";
            }
        }*/
    }
}
