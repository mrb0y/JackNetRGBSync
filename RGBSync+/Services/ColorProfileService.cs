﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleLed;

namespace RGBSyncStudio.Services
{
    public class ColorProfileService
    {
        public List<ColorProfile> GetColorProfiles()
        {
            if (!Directory.Exists("ColorProfiles"))
            {
                Directory.CreateDirectory("ColorProfiles");
            }

            var dir = Directory.GetFiles("ColorProfiles");
            var result = dir.Select(s => JsonConvert.DeserializeObject<ColorProfile>(File.ReadAllText(s))).ToList();
            if (result.Count == 0)
            {
                result = new List<ColorProfile>
                {
                    new ColorProfile
                    {
                        Id = Guid.Empty,
                        ProfileName = "Default",
                        ColorBanks = new ObservableCollection<ColorBank>()
                        {
                            new ColorBank()
                            {
                                BankName = "Basics",
                                Colors = new ObservableCollection<ColorObject>
                                {
                                    new ColorObject {Color = new ColorModel(255,0,0)},
                                    new ColorObject {Color = new ColorModel(255,255,0)},
                                    new ColorObject {Color = new ColorModel(0,255,0)},
                                    new ColorObject {Color = new ColorModel(0,0,255)},
                                    new ColorObject {Color = new ColorModel(255,255,255)},
                                }
                            },
                            new ColorBank()
                            {
                                BankName = "Rainbow",
                                Colors = new ObservableCollection<ColorObject>
                                {
                                    new ColorObject {Color = new ColorModel(255,0,0)},
                                    new ColorObject {Color = new ColorModel(255,153,0)},
                                    new ColorObject {Color = new ColorModel(204,255,0)},
                                    new ColorObject {Color = new ColorModel(51, 255, 0)},
                                    new ColorObject {Color = new ColorModel(0, 255, 102)},
                                    new ColorObject {Color = new ColorModel(0, 255, 255)},
                                    new ColorObject {Color = new ColorModel(0, 102, 255)},
                                    new ColorObject {Color = new ColorModel(51,0,255)},
                                    new ColorObject {Color = new ColorModel(204, 0, 255)},
                                    new ColorObject {Color = new ColorModel(255, 0, 153)},
                                }
                            },
                            new ColorBank
                            {
                                BankName = "Swatch 3",
                                Colors = new ObservableCollection<ColorObject>
                                {
                                    new ColorObject {ColorString = "#0000ff"}, new ColorObject {ColorString = "#000000"}
                                }
                            },
                            new ColorBank
                            {
                                BankName = "Swatch 4",
                                Colors = new ObservableCollection<ColorObject>
                                {
                                    new ColorObject {ColorString = "#ff00ff"}, new ColorObject {ColorString = "#000000"}
                                }
                            }
                        }
                    }
                };
            }

            return result;
        }

    }
}
