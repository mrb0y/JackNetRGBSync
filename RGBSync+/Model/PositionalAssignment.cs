﻿using Newtonsoft.Json;
using SimpleLed;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace RGBSyncStudio.Model
{
    public class PositionalAssignment
    {
        public enum DevicePosition
        {
            Front,
            Back,
            Top,
            Bottom,
            PCI,
            Mouse,
            Keyboard,
        }

        public class PluginDetails
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public Guid PluginId { get; set; }

            public DriverProperties DriverProperties { get; set; }
            public ReleaseNumber Version { get; set; }
            public string Author { get; set; }
            public string Repo { get; set; }

            public PluginDetails()
            {

            }

            public PluginDetails(DriverProperties dp)
            {
                Name = dp.Name;
                Id = dp.Id.ToString();
                PluginId = dp.ProductId;
                DriverProperties = dp;
                Version = dp.CurrentVersion;
                Author = dp.Author;
                Repo = dp.GitHubLink;
            }
        }

        public class PluginVersionDetails : BaseViewModel
        {
            private bool isExperimental;

            public bool IsExperimental
            {
                get => isExperimental;
                set => SetProperty(ref isExperimental, value);
            }

            private ReleaseNumber releaseNumber;
            public ReleaseNumber ReleaseNumber
            {
                get => releaseNumber;
                set => SetProperty(ref releaseNumber, value);
            }

            private bool isInstalled;
            public bool IsInstalled
            {
                get => isInstalled;
                set => SetProperty(ref isInstalled, value);
            }
        }

        public class PluginDetailsViewModel : BaseViewModel
        {
            private ObservableCollection<PluginVersionDetails> versionsAvailable = new ObservableCollection<PluginVersionDetails>();

            public ObservableCollection<PluginVersionDetails> VersionsAvailable
            {
                get => versionsAvailable;
                set => SetProperty(ref versionsAvailable, value);
            }

            private BitmapImage image;

            public BitmapImage Image
            {
                get => image;
                set => SetProperty(ref image, value);
            }


            private string name;
            public string Name { get => name; set => SetProperty(ref name, value); }

            private string author;
            public string Author
            {
                get => author;
                set => SetProperty(ref author, value);
            }

            private string version;
            public string Version
            {
                get => version;
                set => SetProperty(ref version, value);
            }

            private string newestPublicVersion;
            public string NewestPublicVersion
            {
                get => newestPublicVersion;
                set => SetProperty(ref newestPublicVersion, value);
            }

            private string newestPreReleaseVersion;
            public string NewestPreReleaseVersion
            {
                get => newestPreReleaseVersion;
                set => SetProperty(ref newestPreReleaseVersion, value);
            }

            private string installedVersion;
            public string InstalledVersion
            {
                get => installedVersion;
                set => SetProperty(ref installedVersion, value);
            }


            private string blurb;
            public string Blurb
            {
                get => blurb;
                set => SetProperty(ref blurb, value);
            }

            private Guid pluginId;

            public Guid PluginId
            {
                get => pluginId;
                set => SetProperty(ref pluginId, value);
            }

            private string id;

            public string Id
            {
                get => id;
                set => SetProperty(ref id, value);
            }

            public PluginDetails PluginDetails;

            private int releases;
            public int Releases
            {
                get => releases;
                set => SetProperty(ref releases, value);
            }

            private bool preRelease;

            public bool PreRelease
            {
                get => preRelease;
                set => SetProperty(ref preRelease, value);
            }

            private bool installed;

            public bool Installed
            {
                get => installed;
                set => SetProperty(ref installed, value);
            }

            private bool installedButOutdated;

            public bool InstalledButOutdated
            {
                get => installedButOutdated;
                set => SetProperty(ref installedButOutdated, value);
            }


            private bool visible;

            public bool Visible
            {
                get => visible;
                set => SetProperty(ref visible, value);
            }


            public ObservableCollection<PluginDetailsViewModel> Versions { get; set; } = new ObservableCollection<PluginDetailsViewModel>();

            private bool isHovered;

            [JsonIgnore]
            public bool IsHovered
            {
                get => isHovered;
                set => SetProperty(ref isHovered, value);
            }

            private PluginVersionDetails installedVersionModel;
            public PluginVersionDetails InstalledVersionModel
            {
                get => installedVersionModel;
                set => SetProperty(ref installedVersionModel, value);
            }

            public PluginDetailsViewModel()
            {
            }

            public PluginDetailsViewModel(PluginDetails inp, bool dontChild = false)
            {
                string versionAsString = inp.Version != null ? inp.Version.ToString() : "0.0.0.0";

                Name = inp.Name;
                Author = inp.Author;
                Version = versionAsString;
                Blurb = inp.DriverProperties.Blurb;
                PluginDetails = inp;
                PluginId = inp.PluginId;
                Id = inp.Id;
                if (!dontChild)
                {
                    Versions.Add(new PluginDetailsViewModel(inp, true));
                }

                Releases = 1;
            }

            //public PluginDetailsViewModel(DriverProperties inp, bool dontChild = false)
            //{
            //    string versionAsString = inp.CurrentVersion != null ? inp.CurrentVersion.ToString() : "0.0.0.0";

            //    Name = inp.Name;
            //    Author = inp.Author;
            //    Version = versionAsString;
            //    Blurb = inp.Blurb;
            //    PluginDetails = inp;
            //    PluginId = inp.ProductId;
            //    Id = inp.Id.ToString();
            //    if (!dontChild)
            //    {
            //        Versions.Add(new PluginDetailsViewModel(inp, true));
            //    }

            //    Releases = 1;
            //}
        }
    }
}
