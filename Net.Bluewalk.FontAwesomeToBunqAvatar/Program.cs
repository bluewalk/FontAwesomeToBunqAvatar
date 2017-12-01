/**
 * MIT License
 *
 * Copyright (c) 2017 Bluewalk (D. Reijnierse)
 * 
 * Partials courtesy of CGareth Lennox (garethl@dwakn.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
 * the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
 * THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 **/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using NDesk.Options;

namespace Net.Bluewalk.FontAwesomeToBunqAvatar
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var htmlcolor = "#000000";
            string filename = null;
            var help = false;
            var showIcons = false;
            var allBunqColors = false;
            var size = 420;

            var font = "fontawesome-webfont.ttf";

            var options = new OptionSet
                {
                    {"font=", "Font file to use (default: fontawesome-webfont.ttf)", v => font = v},
                    {"htmlcolor=", "Color (HTML color code, default: #000000)", v => htmlcolor = v},
                    {"filename=", "The name of the output file. If all files are exported, it is used as a prefix", v => filename = v},
                    {"size=", "Size of image (default: 420)", (int v) => size = v},
                    {"showIcons", "Show available icons and exit", v => showIcons = v != null},
                    {"allBunqColors", "Generate an icon for every bunq color", v => allBunqColors = v != null},
                    {"h|?|help", "Displays this help text", v => help = v != null},
                };
            var icons = options.Parse(args);

            Console.Error.WriteLine("FontAwesomeToBunqAvatar v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            Console.Error.WriteLine();

            if (help || args.Length == 0)
            {
                Console.Error.WriteLine("Usage: ");
                Console.Error.WriteLine(@"FontAwesomeToBunqAvatar     [-h] [--color COLOR] [--filename FILENAME]
                            [--font FONT] [--showIcons] [--size SIZE]
                            icon [icon ...]");
                Console.WriteLine();
                Console.Error.WriteLine("Options: ");

                options.WriteOptionDescriptions(Console.Error);

                Console.WriteLine();
                Console.WriteLine("Example:");
                Console.WriteLine("FontAwesomeToBunqAvatar --htmlcolor=FFAA11 plane");
                Environment.ExitCode = 1;
                return;
            }
            
            var iconlib = Icons.FontAwesome;
            if (font.Contains("glyphicons"))
            {
                if (font.Contains("social"))
                    iconlib = Icons.GlyphIconsProSocial;
                else if (font.Contains("filetypes"))
                    iconlib = Icons.GlyphIconsProFileTypes;
                else if (font.Contains("halflings"))
                    iconlib = Icons.GlyphIconsProHalflings;
                else
                    iconlib = Icons.GlyphIconsPro;
            }

            if (showIcons)
            {
                Console.WriteLine("Currently icons till Font-Awesome version 4.7.0 ad GlyphIconsPro per 01-12-2017 are supported.");
                Console.WriteLine("See http://fontawesome.io/cheatsheet/ or https://linghucong.js.org/WowUI/icon_glyph_pro.html for all available icons");

                var txtIcons = $"Available icons in {font}{Environment.NewLine}";
                foreach (var icon in iconlib)
                    txtIcons += $" {icon.Key}{Environment.NewLine}";

                File.WriteAllText("icons.txt", txtIcons);

                Console.WriteLine();
                Console.WriteLine("A list of icons has been saved as icons.txt");

                return;
            }

            if (icons.Count == 0)
            {
                Console.Error.WriteLine("WARNING: No icons specified for output");
                Environment.ExitCode = 1;
                return;
            }

            var fontFamily = LoadFont(font);
            if (fontFamily == null)
                return;
            
            var colorColor = ParseColor(htmlcolor);
            if (colorColor == Color.Transparent)
                return;

            IEnumerable<string> iconsToExport = icons;
            var isSingle = false;
            if (icons.Count == 1 && "ALL".Equals(icons[0], StringComparison.OrdinalIgnoreCase))
            {
                iconsToExport = Icons.FontAwesome.Keys;
            }
            else if (icons.Count == 1)
            {
                isSingle = true;
            }

            var colors = new Dictionary<string, Color>();
            colors.Add(htmlcolor, colorColor);

            if (allBunqColors)
                BunqColors.Values.ToList().ForEach(p => colors.Add(p.Key, p.Value));

            foreach (var icon in iconsToExport)
            {
                if (iconlib.TryGetValue(icon, out var iconChar))
                {
                    colors.ToList().ForEach(p =>
                    {
                        var iconFilename = isSingle ? (filename ?? icon + $"-{p.Key}.png") : filename + icon + $"-{p.Key}.png";

                        Console.Error.WriteLine("Exporting icon \"{0}\" as {1} ({2}x{2} pixels)", icon, iconFilename,
                            size);

                        ExportIcon(fontFamily, size, iconChar, icon, iconFilename, p.Value, p.Key);
                    });
                }
                else
                {
                    Console.Error.WriteLine("ERROR: Unknown icon \"{0}\"", icon);
                }
            }
        }

        private static Color ParseColor(string color)
        {
            try
            {
                if (!color.StartsWith("#"))
                    color = $"#{color}";

                return ColorTranslator.FromHtml(color);

            }
            catch (Exception)
            {
                Console.Error.WriteLine("Unable to parse \"{0}\" as a color", color);
                Environment.ExitCode = 1;
                return Color.Transparent;
            }
        }

        private static FontFamily LoadFont(string font)
        {
            if (!File.Exists(font))
            {
                Console.Error.WriteLine("Unable to find font file {0}", font);
                Environment.ExitCode = 1;
                return null;
            }

            try
            {
                var fonts = new PrivateFontCollection();
                fonts.AddFontFile(Path.GetFullPath(font));
                return fonts.Families[0];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error loading font file: {0}", ex.Message);
                Environment.ExitCode = 1;
                return null;
            }
        }

        private static void ExportIcon(FontFamily fontFamily, int size, char icon, string name, string filename, Color color, string htmlColor)
        {
            var iconString = new string(icon, 1);
            const double multiplier = .6;
            var iconSize = (int)Math.Floor(size*multiplier);
            
            using (var image = new Bitmap(size, size, PixelFormat.Format32bppRgb))
            {
                double x = 0;
                double y = 0;
                
                using (var g = Graphics.FromImage(image))
                {
                    setGraphicProperties(g);

                    g.FillRectangle(new SolidBrush(color), 0, 0, size, size);

                    using (var font = new Font(fontFamily, iconSize * .5f))//FindFont(g, fontFamily, iconString, iconSize))
                    {
                        var iconSize2 = g.MeasureString(iconString, font);
                        g.DrawString(iconString, font, new SolidBrush(Color.White), (size - iconSize2.Width) / 2, (size - iconSize2.Height) / 2);
                    }
                    
                }
                image.Save(filename, ImageFormat.Png);
            }
        }

        private static void setGraphicProperties(Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        //private static Font FindFont(Graphics g, FontFamily fontFamily, string icon, int size)
        //{
        //    float closest = size*2;
        //    float closestSize = size;

        //    for (var i = size*.5f; i <= size; i += 1f)
        //    {
        //        using (var font = new Font(fontFamily, i))
        //        {
        //            var result = g.MeasureString(icon, font);

        //            var dX = size - result.Width;
        //            var dY = size - result.Height;

        //            if (dX < 0 || dY < 0)
        //                break;

        //            var minDiff = Math.Min(dX, dY);

        //            if (minDiff < closest)
        //            {
        //                closest = minDiff;
        //                closestSize = i;
        //            }
        //            else if (minDiff > closest)
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    return new Font(fontFamily, closestSize);
        //}
    }
}