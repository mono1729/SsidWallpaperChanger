using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Configuration.ConfigurationManager;

namespace SsidWallpaperChanger.Utilities
{
    public static class Consts
    {
        public static readonly string DefaultSsid = "(default)";
        // DpiScale must be initialized before ThumbnailSize!!!
        public static readonly float DpiScale = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;

        public static readonly Size ThumbnailSize = new Size((int)(80 * DpiScale), (int)(45 * DpiScale));
        public static string TempWallpaperPath = Path.Combine(Path.GetTempPath(), $"SwpcWallpaper.bmp");
        public static string TempRegFilePath = Path.Combine(Path.GetTempPath(), $"temp.reg");

        // HACK: To avoid current directory depending problems, use full path to read/write wallpapers.xml .
        public static readonly string ExeLocatedPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        public static string WallpapersXmlPath = Path.Combine(ExeLocatedPath, AppSettings["wallpapersXml"]);
    }
}
