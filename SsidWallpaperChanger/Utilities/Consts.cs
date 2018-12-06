using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsidWallpaperChanger.Utilities
{
    public static class Consts
    {
        public static readonly string DefaultSsid = "(default)";
        public static readonly Size ThumbnailSize = new Size(80, 45);
        public static string TempWallpaperPath = Path.Combine(Path.GetTempPath(), $"wallpaper{DateTime.Now.Ticks.ToString()}.bmp");
    }
}
