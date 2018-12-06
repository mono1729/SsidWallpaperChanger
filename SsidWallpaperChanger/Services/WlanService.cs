using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWifi;
using SimpleWifi.Win32;

namespace SsidWallpaperChanger.Services
{
    public class WlanService
    {
        // singleton
        private static WlanService _instance = new WlanService();
        public static WlanService Instance
        {
            get { return _instance; }
        }

        public IEnumerable<string> Ssids
        {
            get
            {
                return RetrieveProfileSsids();
            }
        }
        public IEnumerable<string> ConnectedSsids
        {
            get
            {
                return RetrieveConnectedSsids();
            }
        }


        private Wifi _wifi;
        private WlanClient _wlanClient = new WlanClient();

      
        private WlanService()
        {
            _wifi = new Wifi();
        }


        private IEnumerable<string> RetrieveProfileSsids()
        {
            LoggerService.Instance.WriteLog("RetrieveProfileSsids begin.");
            if (_wlanClient.Interfaces == null)
            {
                return Enumerable.Empty<string>();
            }

            var ssids = new List<string>();

            foreach (var intf in _wlanClient.Interfaces)
            {
                foreach(var prof in intf.GetProfiles())
                {
                    ssids.Add(prof.profileName);
                }
            }
            LoggerService.Instance.WriteLog($"RetrieveProfileSsids end. Data:{string.Join(",",ssids)}");
            return ssids;
        }


        private IEnumerable<string> RetrieveConnectedSsids()
        {
            LoggerService.Instance.WriteLog("RetrieveConnectedSsids begin.");
            var ssids = new List<string>();
            foreach(var ap in _wifi.GetAccessPoints())
            {
                if (ap.IsConnected)
                {
                    ssids.Add(ap.Name);
                }
            }
            LoggerService.Instance.WriteLog($"RetrieveConnectedSsids end. Data:{string.Join(",",ssids)}");
            return ssids;
        }

        public bool IsConnected(string ssid)
        {
            return ConnectedSsids.Contains(ssid);
        }
    }
}
