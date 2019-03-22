using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SsidWallpaperChanger.Utilities;
using SsidWallpaperChanger.Models;
using System.Configuration;

namespace SsidWallpaperChanger.Services
{
    public class EventService : IDisposable
    {
        private WallpaperCollection _wallpapers;

        public EventService(WallpaperCollection wallpapers)
        {
            _wallpapers = wallpapers;
        }

        public void RegisterEvent()
        {
            SystemEvents.PowerModeChanged += Event_PowerModeChanged;
            if (ConfigurationManager.AppSettings["resetWallpaperOnExit"].Equals(bool.TrueString))
            {
                System.Windows.Forms.Application.ApplicationExit += Event_ApplicationExit;
                SystemEvents.SessionEnding += Event_SessionEnding;
            }
        }

        public void Event_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    LoggerService.Instance.WriteLog("Resume detected.");
                    _watchEnable = true;
                    break;
                case PowerModes.Suspend:
                    LoggerService.Instance.WriteLog("Suspend detected.");
                    _watchEnable = false;
                    ApplyDefaultWallpaper();
                    break;
            }
        }

        private void Event_ApplicationExit(object sender, EventArgs e)
        {
            _watchEnable = false;
            ApplyDefaultWallpaper();
        }

        private void Event_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            _watchEnable = false;
            ApplyDefaultWallpaper();
        }

        public void ApplyDefaultWallpaper()
        {
            var defaultWp = GetDefaultWallpaper();
            if (defaultWp != null)
            {
                WallpaperService.Instance.ApplyWallpaper(defaultWp);
            }
        }

        private IEnumerable<Wallpaper> GetSpecificWallpapers()
        {
            return _wallpapers.Where(w => w.Ssid != Consts.DefaultSsid).ToList();
        }

        private Wallpaper GetDefaultWallpaper()
        {
            return _wallpapers.Where(w => w.Ssid == Consts.DefaultSsid).FirstOrDefault();
        }

        private bool _watchEnable = true;
        // TODO: isolate from WinForm timer component.
        public void WatchWlanNetworkChanging()
        {
            LoggerService.Instance.WriteLog("WlanNetworkEventWatch start.");
            if (!_watchEnable)
            {
                return;
            }

            var connectedSsids = WlanService.Instance.ConnectedSsids;
            var specificWp = GetSpecificWallpapers();
            var defaultWp = GetDefaultWallpaper();

            foreach(var ssid in connectedSsids)
            {
                foreach(var wp in specificWp)
                {
                    if (ssid == wp.Ssid)
                    {
                        WallpaperService.Instance.ApplyWallpaper(wp);
                        return;
                    }
                }
            }
            WallpaperService.Instance.ApplyWallpaper(defaultWp);

            LoggerService.Instance.WriteLog("WlanNetworkEventWatch finished.");
        }


        private IEnumerable<string> GetConnectedSsids()
        {
            return WlanService.Instance.ConnectedSsids;
        }

        public void Dispose()
        {
            // DO NOT FORGET?
            SystemEvents.PowerModeChanged -= Event_PowerModeChanged;
            SystemEvents.SessionEnding -= Event_SessionEnding;
        }
    }


    delegate void WlanNetworkAvailabilityChangedEventHandler(object sender, WlanNetworkAvailabilityChangedEventArgs e);
    public class WlanNetworkAvailabilityChangedEventArgs : EventArgs
    {
        public bool IsAvailable { get; set; }
    }

    
}
