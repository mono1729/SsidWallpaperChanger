using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SsidWallpaperChanger.Models
{
    public class WallpaperCollection : ICollection<Wallpaper>
    {
        private List<Wallpaper> _list;

        public static WallpaperCollection ReadXml()
        {
            var xmlPath = Utilities.Consts.WallpapersXmlPath;
            if (!System.IO.File.Exists(xmlPath))
            {
                return GetInitialCollection();
            }

            using (var f = new System.IO.StreamReader(xmlPath))
            {
                var xmlser = new XmlSerializer(typeof(WallpaperCollection));
                return (WallpaperCollection)xmlser.Deserialize(f);
            }
        }

        public void WriteXml()
        {
            var xmlPath = Utilities.Consts.WallpapersXmlPath;
            using (var f = new System.IO.StreamWriter(xmlPath))
            {
                var xmlser = new XmlSerializer(typeof(WallpaperCollection));
                xmlser.Serialize(f, this);
            }
        }

        static private WallpaperCollection GetInitialCollection()
        {
            var wc = new WallpaperCollection();
            wc.Add(new Wallpaper());
            return wc;
        }

        public WallpaperCollection()
        {
            _list = new List<Wallpaper>();
        }

        public bool ContainsSsid(string ssid) => _list.Any(w => w.Ssid == ssid);


        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public void Add(Wallpaper item) => _list.Add(item);
        public void Clear() => _list.Clear();
        public bool Contains(Wallpaper item) => _list.Contains(item);
        public void CopyTo(Wallpaper[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<Wallpaper> GetEnumerator() => _list.GetEnumerator();
        public bool Remove(Wallpaper item) => _list.Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
