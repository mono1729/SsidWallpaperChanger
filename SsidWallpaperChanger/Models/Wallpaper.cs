using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SsidWallpaperChanger.Utilities;
using SsidWallpaperChanger.Services;

namespace SsidWallpaperChanger.Models
{
    public class Wallpaper : IEquatable<Wallpaper>, ICloneable
    {
        public string Ssid { get; set; }
        public string ImagePath { get; set; }
        public ResizeMode ResizeMode { get; set; }
        [XmlElement(Type = typeof(XmlColor))]
        public Color WallColor { get; set; }

        public Wallpaper()
        {
            Ssid = Consts.DefaultSsid;
            ImagePath = null;
            ResizeMode = ResizeMode.BorderlessZoom;
            WallColor = Color.Black;
        }

        public void ApplyWallpaper()
        {
            // TODO: DI
            WallpaperService.Instance.ApplyWallpaper(this);
        }

        public bool Equals(Wallpaper other)
        {
            return
                this.Ssid == other.Ssid &&
                this.ImagePath == other.ImagePath &&
                this.ResizeMode == other.ResizeMode &&
                this.WallColor == other.WallColor;
        }

        public object Clone()
        {
            var clone = new Wallpaper();
            clone.Ssid = Ssid;
            clone.ImagePath = ImagePath;
            clone.ResizeMode = ResizeMode;
            clone.WallColor = WallColor;
            return clone;
        }
    }

    public enum ResizeMode
    {
        Original,
        Zoom,
        BorderlessZoom
    };
}
