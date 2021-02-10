﻿using Newtonsoft.Json;
using RGBSyncStudio.Model;
using System.Collections.Generic;

namespace RGBSyncStudio.UI
{
    public class StoreHandler
    {
        public List<PositionalAssignment.PluginDetails> Plugins = new List<PositionalAssignment.PluginDetails>();

        public List<PositionalAssignment.PluginDetails> DownloadStoreManifest()
        {
            string contents;
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                contents = wc.DownloadString("https://raw.githubusercontent.com/SimpleLed/Store/master/manifest.json");
                return (JsonConvert.DeserializeObject<List<PositionalAssignment.PluginDetails>>(contents));
            }
        }
    }
}
