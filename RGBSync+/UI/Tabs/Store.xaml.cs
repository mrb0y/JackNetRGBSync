﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RGBSyncPlus.Helper;
using RGBSyncPlus.Model;
using SharpCompress.Archives;
using Path = System.IO.Path;

namespace RGBSyncPlus.UI.Tabs
{
    /// <summary>
    /// Interaction logic for Store.xaml
    /// </summary>
    public partial class Store : UserControl
    {
        public Store()
        {
            InitializeComponent();
        }

        private StoreViewModel vm => (StoreViewModel) DataContext;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            vm.PluginSearch = this.PluginSearchBox.Text;
        }

        private void ToggleExperimental(object sender, RoutedEventArgs e)
        {
            vm.ShowPreRelease = !vm.ShowPreRelease;
        }

        private void InstallPlugin(object sender, RoutedEventArgs e)
        {
            
            using (new SimpleModal(mainVm, "Installing..."))
            {

                ApplicationManager.Instance.UnloadSLSProviders();


                if (((Button)sender).DataContext is PositionalAssignment.PluginDetailsViewModel bdc)
                {
                    var newest = bdc.Versions.First(x => x.Version == bdc.Version);
                    //if (!vm.ShowPreRelease)
                    //{
                    //    newest = bdc.Versions.Where(x=>x.PluginDetails.DriverProperties.IsPublicRelease).OrderByDescending(x => x.PluginDetails.Version).First();
                    //}

                    string url = $"https://github.com/SimpleLed/Store/blob/master/{newest.Id}.7z?raw=true";

                    WebClient webClient = new WebClient();
                    string destPath = Path.GetTempPath() + bdc.PluginDetails.Id + ".7z";
                    webClient.DownloadFile(url, destPath);

                    string pluginPath = ApplicationManager.SLSPROVIDER_DIRECTORY + "\\" + bdc.PluginId;
                    if (Directory.Exists(pluginPath))
                    {
                        try
                        {
                            Directory.Delete(pluginPath, true);
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        Directory.CreateDirectory(pluginPath);
                    }
                    catch
                    {
                    }

                    using (Stream stream = File.OpenRead(destPath))
                    {
                        var thingy = SharpCompress.Archives.ArchiveFactory.Open(stream);

                        foreach (var archiveEntry in thingy.Entries)
                        {

                            archiveEntry.WriteToDirectory(pluginPath);
                        }

                        try
                        {
                            File.Delete(pluginPath + "\\SimpleLed.dll");
                        }
                        catch
                        {
                        }

                    }
                }

                ApplicationManager.Instance.LoadSLSProviders();
                vm.LoadStoreAndPlugins();
            }

        }

        private MainWindowViewModel mainVm =>
            (MainWindowViewModel) ApplicationManager.Instance.ConfigurationWindow.DataContext;

        private void ReloadAllPlugins(object sender, RoutedEventArgs e)
        {
            StoreViewModel vm = (StoreViewModel)this.DataContext;
            using (new SimpleModal(mainVm, "Reloading Plugins"))
            {
                ContainingGrid.Refresh();
                ApplicationManager.Instance.UnloadSLSProviders();


                ApplicationManager.Instance.LoadSLSProviders();
                vm.LoadStoreAndPlugins();
            }

        }

        private void RefreshStore(object sender, RoutedEventArgs e)
        {
            vm.LoadStoreAndPlugins();
        }
    }
}
