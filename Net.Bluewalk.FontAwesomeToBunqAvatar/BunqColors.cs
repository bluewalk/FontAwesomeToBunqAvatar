using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Bluewalk.FontAwesomeToBunqAvatar
{
    public static class BunqColors
    {
        public static readonly Dictionary<string, Color> Values =
            new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
            {
                {"#ffcd00", Color.FromArgb(255, 205, 0)},
                {"#ff9600", Color.FromArgb(255, 150, 0)},
                {"#fe2851", Color.FromArgb(254, 40, 81)},
                {"#997cf7", Color.FromArgb(153, 124, 247)},
                {"#5856d6", Color.FromArgb(88, 86, 214)},
                {"#0076ff", Color.FromArgb(0, 118, 255)},
                {"#54c7fc", Color.FromArgb(84, 199, 252)},
                {"#00cdc0", Color.FromArgb(0, 205, 192)},
                {"#8ce329", Color.FromArgb(140, 227, 41)},
                {"#44db5e", Color.FromArgb(68, 219, 94)},
                {"#bdbdbd", Color.FromArgb(189, 189, 189)},
                {"#8f8e94", Color.FromArgb(143, 142, 148)}
            };
    }
}
