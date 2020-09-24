﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace RGBSyncPlus
{
    public  class ProfileTriggerManager : BaseViewModel
    {
        private ObservableCollection<ProfileTriggerEntry> profileTriggers = new ObservableCollection<ProfileTriggerEntry>();

        public ObservableCollection<ProfileTriggerEntry> ProfileTriggers
        {
            get => profileTriggers;
            set => SetProperty(ref profileTriggers, value);
        }

        private List<Guid> blockedTriggers =new List<Guid>();

        public ProfileTriggerManager()
        {
            ProfileTriggers.Add(new ProfileTriggerEntry
            {
                Id = Guid.NewGuid(),
                ProfileName = "Default",
                TriggerType = ProfileTriggerTypes.RunningProccess,
                ProcessName = "Calculator",
                TriggerWhenNotFound = false
            });

            ProfileTriggers.Add(new ProfileTriggerEntry
            {
                Id = Guid.NewGuid(),
                ProfileName = "Default",
                TriggerType = ProfileTriggerTypes.TimeBased,
                Hour=0,
                Minute = 31
            });
        }

        public void CheckTriggers()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (ProfileTriggerEntry profileTriggerEntry in ProfileTriggers)
            {
                bool doit = false;
                switch (profileTriggerEntry.TriggerType)
                {
                    case ProfileTriggerTypes.RunningProccess:
                    {
                        bool foundProcess = processlist.Any(x => x.ProcessName == profileTriggerEntry.ProcessName);

                            doit = foundProcess;
                            if (profileTriggerEntry.TriggerWhenNotFound)
                            {
                                doit = !doit;
                            }

                           

                        break;
                    }

                    case ProfileTriggerTypes.TimeBased:
                    {
                        doit = (DateTime.Now.Minute == profileTriggerEntry.Minute &&
                                DateTime.Now.Hour == profileTriggerEntry.Hour);
                        break;
                    }
                }

                if (doit && blockedTriggers.All(x => x != profileTriggerEntry.Id))
                {
                    ApplicationManager.Instance.LoadProfileFromName(profileTriggerEntry.ProfileName);
                    blockedTriggers.Add(profileTriggerEntry.Id);
                }

                if (!doit && blockedTriggers.Any(x => x == profileTriggerEntry.Id))
                {
                    blockedTriggers.Remove(profileTriggerEntry.Id);
                }
            }
        }

        public class ProfileTriggerEntry : BaseViewModel
        {
            private Guid id;

            public Guid Id
            {
                get => id;
                set => SetProperty(ref id, value);
            }

            private string profileName;

            public string ProfileName
            {
                get => profileName;
                set => SetProperty(ref profileName, value);
            }

            private string triggerType;

            public string TriggerType
            {
                get => triggerType;
                set => SetProperty(ref triggerType, value);
            }

            //RunningProccess

            private string processName;

            public string ProcessName
            {
                get => processName;
                set => SetProperty(ref processName, value);
            }

            private bool triggerWhenNotFound;

            public bool TriggerWhenNotFound
            {
                get => triggerWhenNotFound;
                set => SetProperty(ref triggerWhenNotFound, value);
            }

            //TimeBased

            private int hour;

            public int Hour
            {
                get => hour;
                set => SetProperty(ref hour, value);
            }

            private int minute;

            public int Minute
            {
                get => minute;
                set => SetProperty(ref minute, value);
            }
        }

        public static class ProfileTriggerTypes
        {
            public const string RunningProccess = "Running Process";
            public const string TimeBased = "Time Based";

        }
    }
}