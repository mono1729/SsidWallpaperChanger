using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SsidWallpaperChanger.Models;
using SsidWallpaperChanger.Services;
using SsidWallpaperChanger.Utilities;

namespace SsidWallpaperChanger.ViewModels
{
    public class WallpaperViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Wallpaper _wallpaper;

        public WallpaperViewModel(Wallpaper wallpaper)
        {
            _wallpaper = wallpaper;
            LoadThumbnail();
        }

        public Wallpaper GetModel()
        {
            return _wallpaper;
        }

        public string Ssid
        {
            get
            {
                return _wallpaper.Ssid;
            }
            set
            {
                _wallpaper.Ssid = value;
                RaisePropertyChanged();
            }
        }

        public string ImagePath
        {
            get { return _wallpaper.ImagePath; }
            set
            {
                _wallpaper.ImagePath = value;
                LoadThumbnail();
                RaisePropertyChanged();
            }
        }

        public string ResizeMode
        {
            get { return _wallpaper.ResizeMode.ToString(); }
            set
            {
                ResizeMode mode;
                if (Enum.TryParse<ResizeMode>(value, out mode))
                {
                    _wallpaper.ResizeMode = mode;
                    RaisePropertyChanged();
                }
            }
        }

        public string WallColor
        {
            get
            {
                return ColorTranslator.ToHtml(_wallpaper.WallColor);
            }
            set
            {
                _wallpaper.WallColor = ColorTranslator.FromHtml(value);
            }
        }

        private Image _thumbnail;
        public Image Thumbnail
        {
            get { return _thumbnail; }
        }

        public bool IsConnected
        {
            get { return WlanService.Instance.IsConnected(_wallpaper.Ssid); }
        }


        private void LoadThumbnail()
        {
            try
            {
                var originalImage = new Bitmap(_wallpaper.ImagePath);
                _thumbnail = new Bitmap(originalImage, Consts.ThumbnailSize);
            }
            catch (Exception)
            {
                _thumbnail = null;
            }
            finally
            {
                RaisePropertyChanged(nameof(Thumbnail));
            }
        }
    }
}
