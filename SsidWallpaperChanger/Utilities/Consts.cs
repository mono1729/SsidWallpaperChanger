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
        // DpiScale must be initialized before ThumbnailSize!!!
        public static readonly float DpiScale = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;
        public static readonly Size ThumbnailSize = new Size((int)(80 * DpiScale), (int)(45 * DpiScale));
        public static string TempWallpaperPath = Path.Combine(Path.GetTempPath(), $"wallpaper{DateTime.Now.Ticks.ToString()}.bmp");
    }
}
