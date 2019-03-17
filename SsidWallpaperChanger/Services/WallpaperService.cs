using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SsidWallpaperChanger.Models;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using SsidWallpaperChanger.Utilities;

namespace SsidWallpaperChanger.Services
{
    public class WallpaperService
    {
        // singleton
        private static WallpaperService _instance = new WallpaperService();
        public static WallpaperService Instance { get { return _instance; } }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);


        private Wallpaper _currentWallpaper = null;
        private Size _currentResolution = Screen.PrimaryScreen.Bounds.Size;

        public void ApplyWallpaper(Wallpaper wallpaper)
        {
            if (_currentWallpaper != null && _currentWallpaper.Equals(wallpaper) &&
                _currentResolution == Screen.PrimaryScreen.Bounds.Size)
            {
                return;
            }
            if (!string.IsNullOrEmpty(wallpaper.ImagePath))
            {
                var originalPic = new Bitmap(wallpaper.ImagePath);
                var resizedPic = ResizePicture(originalPic, wallpaper.ResizeMode);
                resizedPic.Save(Consts.TempWallpaperPath);
                SetWallpaper(Consts.TempWallpaperPath);
            }
            else
            {
                SetWallpaper(string.Empty);
            }
            SetWallcolor(wallpaper.WallColor);
            // Wallpaper is MUTABLE!!
            _currentWallpaper = (Wallpaper)wallpaper.Clone();
        }

        private void RemoveWallpaperCache()
        {
            File.Delete(Consts.TempWallpaperPath);
        }

        private void SetWallpaper(string fileName)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue("WallpaperStyle", "0");
            key.SetValue("TileWallpaper", "0");
            // HACK: Set wallpaper twice due to prevent a mysterious behavior on wallpaper removal.
            // (confirmed on Win10-1809.)
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0,
                fileName,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0,
                fileName,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        private void SetWallcolor(Color backColor)
        {
            SetSysColors(1, new[] { 1 }, new[] { ColorTranslator.ToWin32(backColor) });
        }


        private Image ResizePicture(Image img, ResizeMode rMode)
        {
            // Rotate picture if Exif rotation property is present.
            var rImg = ApplyExifOrientation(img);
            var targetSize = CalculateSize(rImg.Size, rMode);
            var target = new Bitmap(targetSize.Width, targetSize.Height);
            var gra = Graphics.FromImage(target);
            gra.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            gra.DrawImage(rImg, 0, 0, targetSize.Width, targetSize.Height);
            return target;
        }

        private Image ApplyExifOrientation(Image img)
        {
            var ExifOrientation = 0x112;
            if (!img.PropertyIdList.Contains(ExifOrientation))
            {
                return img;
            }
            var prop = img.GetPropertyItem(ExifOrientation);
            var rotation = RotateFlipType.RotateNoneFlipNone;

            switch (prop.Value[0])
            {
                case 3:
                    rotation = RotateFlipType.Rotate180FlipNone;
                    break;
                case 6:
                    rotation = RotateFlipType.Rotate90FlipNone;
                    break;
                case 8:
                    rotation = RotateFlipType.Rotate270FlipNone;
                    break;
            }

            var rotated = (Bitmap)img.Clone();
            rotated.RotateFlip(rotation);
            return rotated;
        }

        private Size CalculateSize(Size imageSize, ResizeMode rMode)
        {
            if (rMode == ResizeMode.Original)
            {
                return imageSize;
            }

            var screenSize = Screen.PrimaryScreen.Bounds.Size;

            var screenAspectRatio = (double)screenSize.Width / screenSize.Height;
            var imageAspectRatio = (double)imageSize.Width / imageSize.Height;

            double ratio;

            var fitVertically = (screenAspectRatio > imageAspectRatio);
            if (rMode == ResizeMode.BorderlessZoom)
            {
                fitVertically = !fitVertically;
            }

            if (fitVertically)
            {
                ratio = (double)screenSize.Height / imageSize.Height;
            }
            else
            {
                ratio = (double)screenSize.Width / imageSize.Width;
            }

            return new Size((int)(imageSize.Width * ratio), (int)(imageSize.Height * ratio));
        }
    }
}
