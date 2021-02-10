﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RGBSyncStudio.UI;
using RGBSyncStudio.UI.Tabs;
using RGBSyncStudio.Helper;
using RGBSyncStudio.Model;
using SimpleLed;

namespace RGBSyncStudio.Services
{

    public class LedService
    {
        public System.Timers.Timer SLSTimer;
        public const string SLSPROVIDER_DIRECTORY = "SLSProvider";
        public bool PauseSyncing { get; set; } = false;
        Dictionary<string, string> pathfun = new Dictionary<string, string>();
        public RSSBackgroundDevice RssBackgroundDevice = new RSSBackgroundDevice();

        public ObservableCollection<ControlDevice> SLSDevices = new ObservableCollection<ControlDevice>();
        public ObservableCollection<DeviceMappingModels.DeviceOverrides> DeviceOverrides = new ObservableCollection<DeviceMappingModels.DeviceOverrides>();

        public LedService()
        {

        }

        public ControlDevice DeviceBeingAligned;

        private readonly ControlDevice virtualAlignmentDevice = new ControlDevice
        {
            LEDs = new ControlDevice.LedUnit[64]
            {
                new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)}, new ControlDevice.LedUnit{Color = new LEDColor(255,0,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)}, new ControlDevice.LedUnit{Color = new LEDColor(0,255,0)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},
                new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)},new ControlDevice.LedUnit{Color = new LEDColor(0,0,255)}
            }
        };


        private void SLSUpdate(object state)
        {
            var CurrentProfile = ServiceManager.Instance.ProfileService.CurrentProfile;


            if (PauseSyncing)
            {
                return;
            }

            if (CurrentProfile == null || CurrentProfile.DeviceProfileSettings == null)
            {
                return;
            }

            List<ControlDevice> devicesToPull = new List<ControlDevice>();

            foreach (DeviceMappingModels.NGDeviceProfileSettings currentProfileDeviceProfileSetting in CurrentProfile.DeviceProfileSettings.ToList())
            {
                try
                {
                    ControlDevice cd = ServiceManager.Instance.LedService.SLSDevices.ToList().FirstOrDefault(x =>
                        x.Name == currentProfileDeviceProfileSetting.SourceName &&
                        x.Driver.Name() == currentProfileDeviceProfileSetting.SourceProviderName &&
                        x.ConnectedTo == currentProfileDeviceProfileSetting.SourceConnectedTo);

                    if (cd != null)
                    {
                        if (!devicesToPull.Contains(cd))
                        {
                            devicesToPull.Add(cd);
                        }
                    }
                }
                catch
                {
                }
            }

            foreach (ControlDevice controlDevice in devicesToPull)
            {
                if (controlDevice.Driver.GetProperties().SupportsPull)
                {
                    controlDevice.Pull();
                }
            }

            List<PushListItem> pushMe = new List<PushListItem>();
            foreach (DeviceMappingModels.NGDeviceProfileSettings currentProfileDeviceProfileSetting in CurrentProfile.DeviceProfileSettings.ToList())
            {
                ControlDevice cd = ServiceManager.Instance.LedService.SLSDevices.FirstOrDefault(x =>
                    x.Name == currentProfileDeviceProfileSetting.SourceName &&
                    x.Driver.Name() == currentProfileDeviceProfileSetting.SourceProviderName &&
                    x.ConnectedTo == currentProfileDeviceProfileSetting.SourceConnectedTo);

                ControlDevice dest = ServiceManager.Instance.LedService.SLSDevices.FirstOrDefault(x =>
                    x.Name == currentProfileDeviceProfileSetting.Name &&
                    x.Driver.Name() == currentProfileDeviceProfileSetting.ProviderName &&
                    x.ConnectedTo == currentProfileDeviceProfileSetting.ConnectedTo);

                if (cd != null && dest != null)
                {
                    string key = currentProfileDeviceProfileSetting.SourceName +
                                 currentProfileDeviceProfileSetting.SourceProviderName +
                                 currentProfileDeviceProfileSetting.SourceConnectedTo +
                                 currentProfileDeviceProfileSetting.Name +
                                 currentProfileDeviceProfileSetting.ProviderName +
                                 currentProfileDeviceProfileSetting.ConnectedTo;

                    //if (!isMapping.ContainsKey(key) || isMapping[key] == false)
                    {
                        if (!isMapping.ContainsKey(key))
                        {
                            isMapping.Add(key, true);
                        }

                        dest.MapLEDs(cd);
                        pushMe.Add(new PushListItem
                        {
                            Device = dest,
                            Driver = dest.Driver,
                            Key = key
                        });


                        isMapping[key] = true;
                    }


                }

            }

            foreach (IGrouping<ISimpleLed, PushListItem> gp in pushMe.GroupBy(x => x.Driver))
            {
                Task.Run(async () =>
                {
                    foreach (PushListItem t in gp.ToList())
                    {
                        try
                        {
                            gp.Key.Push(t.Device);
                            await Task.Delay(0);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }

                    foreach (PushListItem t in gp.ToList())
                    {
                        try
                        {
                            isMapping[t.Key] = false;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
               );
            }

        }

        public Dictionary<string, bool> isMapping = new Dictionary<string, bool>();

        public class PushListItem
        {
            public ISimpleLed Driver { get; set; }
            public ControlDevice Device { get; set; }
            public string Key { get; set; }

        }


        public DeviceMappingModels.Device ToDevice(ControlDevice device)
        {
            ObservableCollection<DeviceMappingModels.NGDeviceProfileSettings> temp = ServiceManager.Instance.ProfileService.CurrentProfile?.DeviceProfileSettings;

            DeviceMappingModels.NGDeviceProfileSettings thingy = temp.FirstOrDefault(x =>
                x.Name == device.Name && x.ConnectedTo == device.ConnectedTo &&
                x.ProviderName == device.Driver.Name());

            DriverProperties props = device.Driver.GetProperties();

            var overrides = GetOverride(device);

            BitmapImage bmp = null;

            try
            {
                if (overrides?.CustomDeviceSpecification?.Bitmap != null)
                {
                    bmp = overrides.CustomDeviceSpecification.Bitmap.ToBitmapImage();
                }
                else
                {
                    bmp = device.ProductImage.ToBitmapImage();
                }
            }
            catch
            {
            }

            var tmp = new DeviceMappingModels.Device
            {
                SunkTo = thingy?.SourceName ?? "",
                ControlDevice = device,
                Image = bmp,
                Name = device.Name,
                ProviderName = device.Driver.Name(),
                SupportsPull = props.SupportsPull,
                SupportsPush = props.SupportsPush,
                DriverProps = props,
                Title = string.IsNullOrWhiteSpace(device.TitleOverride)
                    ? device.Driver.Name()
                    : device.TitleOverride,
                ConnectedTo = device.ConnectedTo,
                Overrides = GetOverride(device)

            };

            return tmp;
        }





        public bool OverridesDirty = false;
        public void SetOverride(DeviceMappingModels.DeviceOverrides overrider)
        {
            DeviceMappingModels.DeviceOverrides existing = DeviceOverrides.FirstOrDefault(x =>
                x.Name == overrider.Name && x.ConnectedTo == overrider.ConnectedTo &&
                x.ProviderName == overrider.ProviderName);

            if (existing != null)
            {
                DeviceOverrides.Remove(existing);
            }

            DeviceOverrides.Add(overrider);
            OverridesDirty = true;

        }


        public DeviceMappingModels.DeviceOverrides GetOverride(ControlDevice cd)
        {
            DeviceMappingModels.DeviceOverrides existing = DeviceOverrides.ToList().FirstOrDefault(x =>
                x.Name == cd.Name && x.ConnectedTo == cd.ConnectedTo &&
                x.ProviderName == cd.Driver.Name());

            if (existing == null)
            {
                existing = GenerateOverride(cd);
            }

            return existing;
        }

        public DeviceMappingModels.DeviceOverrides GenerateOverride(ControlDevice cd)
        {
            var existing = new DeviceMappingModels.DeviceOverrides
            {
                Name = cd.Name,
                ConnectedTo = cd.ConnectedTo,
                ProviderName = cd.Driver?.Name(),
                TitleOverride = string.IsNullOrWhiteSpace(cd.TitleOverride) ? cd.Driver.Name() : cd.TitleOverride,
                ChannelOverride = cd.ConnectedTo,
                SubTitleOverride = cd.Name,
                CustomDeviceSpecification = new CustomDeviceSpecification()
            };
            DeviceOverrides.Add(existing);
            OverridesDirty = true;

            return existing;
        }

        public ControlDevice GetControlDeviceFromName(string providerName, string name)
        {
            return SLSDevices.FirstOrDefault(x => x.Name == name && x.Driver.Name() == providerName);
        }
        
        public SolidColorDriver SolidColorDevice { get; set; }
        public GradientDriver GradientDriver { get; set; }



        public void UnloadSLSProviders()
        {
            ServiceManager.Instance.SLSManager.Drivers.ToList().ForEach(x =>
            {
                try
                {
                    x.Dispose();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }


                try
                {

                    x = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });

            try
            {
                if (ServiceManager.Instance.SLSManager.Drivers == null)
                {
                    ServiceManager.Instance.SLSManager.Drivers = new ObservableCollection<ISimpleLed>();
                }

                ServiceManager.Instance.SLSManager.Drivers.Clear();
            }
            catch
            {
            }

            GC.Collect(); // collects all unused memory
            GC.WaitForPendingFinalizers(); // wait until GC has finished its work
            GC.Collect();
            Thread.Sleep(1000);
        }

        public void LoadOverrides()
        {
            if (File.Exists("NGOverrides.json"))
            {
                string json = File.ReadAllText("NGOverrides.json");
                var tmp = JsonConvert.DeserializeObject<List<DeviceMappingModels.DeviceOverrides>>(json);
                DeviceOverrides = new ObservableCollection<DeviceMappingModels.DeviceOverrides>(tmp);
            }
        }

        public void LoadSLSProviders()
        {
            UnloadSLSProviders();

            string deviceProvierDir =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? string.Empty,
                        SLSPROVIDER_DIRECTORY);

            if (!Directory.Exists(deviceProvierDir)) return;
            string[] pluginFolders = Directory.GetDirectories(deviceProvierDir);
            ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Loading SLS plugins";
            ServiceManager.Instance.ApplicationManager.LoadingSplash.ProgressBar.Maximum = pluginFolders.Length;

            int ct = 0;
            foreach (string pluginFolder in pluginFolders)
            {
                //LoadingSplash.Activate();
                ct++;

                ServiceManager.Instance.ApplicationManager.LoadingSplash.ProgressBar.Value = ct;
                ServiceManager.Instance.ApplicationManager.LoadingSplash.ProgressBar.Refresh();
                LoadPlungFolder(pluginFolder);


            }
            
            var type = typeof(ISimpleLed);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToList();

            Assembly asm = Assembly.GetExecutingAssembly();
            var typos = asm.GetTypesWithInterface();

            SolidColorDevice = new SolidColorDriver();
            GradientDriver = new GradientDriver();



            ServiceManager.Instance.SLSManager.Drivers.Add(RssBackgroundDevice);
            ServiceManager.Instance.SLSManager.Drivers.Add(SolidColorDevice);
            ServiceManager.Instance.SLSManager.Drivers.Add(GradientDriver);



            RssBackgroundDevice.DeviceAdded += SlsDriver_DeviceAdded;
            RssBackgroundDevice.DeviceRemoved += SlsDriver_DeviceRemoved;

            SolidColorDevice.DeviceAdded += SlsDriver_DeviceAdded;
            SolidColorDevice.DeviceRemoved += SlsDriver_DeviceRemoved;

            GradientDriver.DeviceAdded += SlsDriver_DeviceAdded;
            GradientDriver.DeviceRemoved += SlsDriver_DeviceRemoved;

            SolidColorDevice.Configure(new DriverDetails());
            GradientDriver.Configure(new DriverDetails());

            //HarnessDriver(new CUEDriver());

            //SLSManager.RescanRequired += Rescan;
            ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Updating SLS devices";
            UpdateSLSDevices();
        }

        private void HarnessDriver(ISimpleLed driver)
        {
            ServiceManager.Instance.SLSManager.Drivers.Add(driver);

            driver.DeviceAdded += SlsDriver_DeviceAdded;
            driver.DeviceRemoved += SlsDriver_DeviceRemoved;

            driver.Configure(new DriverDetails());

        }
        
        public void UpdateSLSDevices()
        {
            ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Loading Configs";
            foreach (ISimpleLed drv in ServiceManager.Instance.SLSManager.Drivers)
            {
                if (drv is ISimpleLedWithConfig cfgdrv)
                {
                    try
                    {
                        ServiceManager.Instance.SLSManager.LoadConfig(cfgdrv);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }


            ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Getting devices";
            //SLSDevices = SLSManager.GetDevices();
            ServiceManager.Instance.ApplicationManager.LoadingSplash.ProgressBar.Value = 0;
            ServiceManager.Instance.ApplicationManager.LoadingSplash.ProgressBar.Maximum = ServiceManager.Instance.SLSManager.Drivers.Count;

        }

        public void LoadPlung(string file)
        {
            string filename = file.Split('\\').Last();
            string justPath = file.Substring(0, file.Length - filename.Length);
            if (filename.ToLower().StartsWith("driver") || filename.ToLower().StartsWith("source") || filename.ToLower().StartsWith("gameintegration"))
            {
                try
                {
                    ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Loading " + file.Split('\\').Last().Split('.').First();
                    ServiceManager.Instance.Logger.Debug("Loading provider " + file);

                    ISimpleLed slsDriver = TypeLoaderExtensions.LoadDll(justPath, filename);

                    if (slsDriver != null)
                    {
                        try
                        {
                            if (ServiceManager.Instance.SLSManager.Drivers.All(p => p.GetProperties().Id != slsDriver.GetProperties().Id))
                            {
                                //slsDriver.Configure(null);
                                Debug.WriteLine("We got one! " + "Loading " + slsDriver.Name());
                                ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Text = "Loading " + slsDriver.Name();
                                ServiceManager.Instance.ApplicationManager.LoadingSplash.UpdateLayout();
                                ServiceManager.Instance.ApplicationManager.LoadingSplash.Refresh();
                                ServiceManager.Instance.ApplicationManager.LoadingSplash.LoadingText.Refresh();
                                ServiceManager.Instance.SLSManager.Drivers.Add(slsDriver);
                                // driversAdded.Add(slsDriver.GetProperties().Id);
                                Debug.WriteLine("all loaded: " + slsDriver.Name());
                                slsDriver.DeviceAdded += SlsDriver_DeviceAdded;
                                slsDriver.DeviceRemoved += SlsDriver_DeviceRemoved;
                                if (pathfun.ContainsKey(slsDriver.GetProperties().Id.ToString()))
                                {
                                    pathfun[slsDriver.GetProperties().Id.ToString()] = justPath;
                                }
                                else
                                {
                                    pathfun.Add(slsDriver.GetProperties().Id.ToString(), justPath);
                                }
                                try
                                {
                                    Debug.WriteLine("Lets config it!");

                                    IAsyncResult result;
                                    bool complete = false;

                                    slsDriver.Configure(new DriverDetails() { HomeFolder = justPath });
                                    complete = true;

                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                Debug.WriteLine("Have Initialized: " + slsDriver.Name());

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public void LoadPlungFolder(string pluginFolder)
        {

            string[] files = Directory.GetFiles(pluginFolder, "*.dll");


            List<Guid> driversAdded = new List<Guid>();
            foreach (string file in files)
            {
                Debug.WriteLine("Checking " + file);
                LoadPlung(file);
            }


        }

        private void SlsDriver_DeviceRemoved(object sender, Events.DeviceChangeEventArgs e)
        {
            SLSDevices.Remove(e.ControlDevice);
        }

        private void SlsDriver_DeviceAdded(object sender, Events.DeviceChangeEventArgs e)
        {
            Debug.WriteLine("" +
                            "Adding device" +
                            ": " + e.ControlDevice.Name);

            if (e.ControlDevice.Name == "matrix")
            {
                Debug.WriteLine("Here we go!");
            }


            //DispatcherTimer t = new DispatcherTimer();
            //t.Interval = TimeSpan.FromMilliseconds(200);
            //t.Tick+= (o, args) =>
            //{

            if (e.ControlDevice.Name == "matrix")
            {
                Debug.WriteLine("Here we go!");
            }

            //   t.Stop();
            var overriden = GetOverride(e.ControlDevice);
            if (overriden != null)
            {
                var props = e.ControlDevice.Driver.GetProperties();
                if (e.ControlDevice.OverrideSupport != OverrideSupport.None)
                {
                    if (props?.SetDeviceOverride != null && overriden.CustomDeviceSpecification.LedCount > 0)
                    {
                        props.SetDeviceOverride(e.ControlDevice, overriden.CustomDeviceSpecification);
                        e.ControlDevice.CustomDeviceSpecification = overriden.CustomDeviceSpecification;
                    }

                    e.ControlDevice.GridWidth = overriden.CustomDeviceSpecification.GridWidth;
                    e.ControlDevice.GridHeight = overriden.CustomDeviceSpecification.GridHeight;

                }
            }

            SLSDevices.Add(e.ControlDevice);
            // };

            //  t.Start();
        }

        public void SetUpdateRate(double tmr2)
        {
            if (SLSTimer != null)
            {
                SLSTimer.Stop();
                SLSTimer.Dispose();
                SLSTimer = null;
            }

            Debug.WriteLine("Setting up timer with ms of " + tmr2);
            SLSTimer = new System.Timers.Timer(tmr2);
            SLSTimer.AutoReset = true;
            SLSTimer.Elapsed += (sender, args) => SLSUpdate(null);
            SLSTimer.Start();

        }
    }
}
