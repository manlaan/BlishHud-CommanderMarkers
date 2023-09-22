using Manlaan.CommanderMarkers.Presets.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Manlaan.CommanderMarkers.Presets.Services;


[Serializable]
public class MarkerListing
{
    public event EventHandler? MarkersChanged;
    
    [JsonIgnore]
    public static string FILENAME = "custom_markers.json";

    [JsonProperty("version")]
    public string Version { get; set; } = "2.0.0";

    [JsonProperty("squadMarkerPreset")]
    public List<MarkerSet> presets { get; set; } = new();

    public List<MarkerSet> GetEmpty() => new();

    public void SaveMarker(MarkerSet markerSet)
    {
        if (presets.Contains(markerSet)) return;

        presets.Add(markerSet);
        Save();

    }
    public void EditMarker(int index, MarkerSet markerSet) 
    {
        presets[index] = markerSet;
        Save();
    }

    public void DeleteMarker(MarkerSet markerSet)
    {
        presets.Remove(markerSet);
        Save();
    }

    public List<MarkerSet> GetAllMarkerSets()
    {
        return presets.ToList();
    }
    public List<MarkerSet> GetMarkersForMap(int mapId)
    {
        if (Service.Settings.AutoMarker_FeatureEnabled.Value == false) return new List<MarkerSet>();

        return presets.Where(m => m.mapId == mapId).ToList();
    }

    public void ResetToDefault()
    {
        presets.Clear();
        InitEmptyFile();
        
    }

    public void Save()
    {
        //PluginLog.Verbose($"{DateTime.Now} - {CharacterData.Name} Saved");

        var configFileInfo = GetConfigFileInfo();

        var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

        using var writer = new StreamWriter(configFileInfo.FullName);
        writer.Write(serializedContents);
        writer.Close();

        MarkersChanged?.Invoke(this, null);;
        //PluginLog.Warning("Tried to save a config with invalid LocalContentID, aborting save.");

    }

    private static FileInfo GetConfigFileInfo()
    {
        var pluginConfigDirectory = CommanderMarkers.Service.DirectoriesManager.GetFullDirectoryPath(Module.DIRECTORY_PATH);

        return new FileInfo($@"{pluginConfigDirectory}\{FILENAME}");
    }

    public void ReloadFromFile()
    {
        var temp = Load();
        presets = temp.presets;
    }

    public static MarkerListing Load()
    {
        if (GetConfigFileInfo() is { Exists: true } configFileInfo)
        {
            using var reader = new StreamReader(configFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Close();

            return LoadExistingCharacterConfiguration(fileText);
        }
        else
        {
            return CreateNewCharacterConfiguration();
        }
    }

    private static MarkerListing LoadExistingCharacterConfiguration(string fileText)
    {
        var loadedCharacterConfiguration = JsonConvert.DeserializeObject<MarkerListing>(fileText);

        if (loadedCharacterConfiguration == null)
        {
            loadedCharacterConfiguration = new MarkerListing();
        }
        if(loadedCharacterConfiguration.Version == "1.0.0")
        {
            loadedCharacterConfiguration = MigrateToVersion2(loadedCharacterConfiguration);
        }

        return loadedCharacterConfiguration;
    }

    private static MarkerListing CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new MarkerListing();
        newCharacterConfiguration.Save();
        newCharacterConfiguration.InitEmptyFile();
        return newCharacterConfiguration;
    }

    protected static MarkerListing MigrateToVersion2(MarkerListing loadedFromFile)
    {
        loadedFromFile.ResetToDefault();
        loadedFromFile.Version = "2.0.0";
        loadedFromFile.Save();


       
        return loadedFromFile;
    }


    public void InitEmptyFile()
    {

        var ms = new MarkerSet
        {
            name = "Sabetha",
            description = "Cannon bomb launch platforms",
            mapId = 1062,
            trigger = new WorldCoord { x = -78.40887f, y = 133.5607f, z = 70.977f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){x= -132.6262f, y =  56.29699f, z = 62.96119f, icon =1, name="South"},
                new MarkerCoord(){x= -157.7033f, y =  87.01214f, z = 62.27411f, icon =2, name="West"},
                new MarkerCoord(){x= -126.95f, y = 111.8531f, z = 62.94498f, icon =3, name="North"},
                new MarkerCoord(){x= -101.9202f, y = 81.18908f, z = 63.03901f, icon =4, name="East"},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "Gorseval",
            description = "Spirit Spawns",
            mapId = 1062,
            trigger = new WorldCoord { x=-2.034508f,y=-107.3541f, z=50.7478f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=21.407f, y = -92.894f, z = 48.572f, icon =1, name=""},
                new MarkerCoord(){ x=64.096f, y = -93.094f, z = 48.908f, icon =2, name=""},
                new MarkerCoord(){ x=62.752f, y = -131.938f, z = 48.868f, icon =3, name=""},
                new MarkerCoord(){ x=22.018f, y = -134.791f, z = 48.618f, icon =4, name=""},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "Slothasor",
            description = "Mushroom Locations",
            mapId = 1149,
            trigger = new WorldCoord { x=211.8357f, y=36.68708f, z=8.145153f },
            marks = new List<MarkerCoord>
            {
            new MarkerCoord(){ x=208.928f, y = 25.562f, z = 4.933f, icon =1, name="1"},
            new MarkerCoord(){ x=174.839f, y = -9.068f, z = 2.414f, icon =2, name="2"},
            new MarkerCoord(){ x=192.947f, y = -35.956f, z = 0.846f, icon =3, name="3"},
            new MarkerCoord(){ x=224.945f, y = -13.404f, z = -0.177f, icon =4, name="4"},
            }
        };
        SaveMarker(ms);

        /*ms = new MarkerSet
        {
            name = "Bandit Trio",
            description = "Boss Spawns, Square-Mortar",
            mapId = 1149,
            trigger = new WorldCoord { x = 64.95536f, y = -270.91f, z = 0.9945363f },
            marks = new List<MarkerCoord>
            {
            new MarkerCoord(){ x=-1.953f, y = -235.489f, z = -0.132f, icon =1, name="Berg"},
            new MarkerCoord(){ x=-0.567f, y = -290.812f, z = -0.619f, icon =2, name="Zane"},
            new MarkerCoord(){ x=-32.935f, y = -221.024f, z = 4.176f, icon =3, name="Narella"},
            new MarkerCoord(){ x=64.95536f, y = -270.91f, z = 0.9945363f, icon =4, name="Placement location"},
            }
        };
        SaveMarker(ms);*/

        ms = new MarkerSet
        {
            name = "Mathias Gabriel",
            description = "Fountain Locations",
            mapId = 1149,
            trigger = new WorldCoord { x=-150.7991f, y=116.5239f, z=131.5231f },
            marks = new List<MarkerCoord>
             {
                new MarkerCoord(){ x=-171.0662f, y = 129.6464f, z = 131.7141f, icon =1, name = "SW"},
                new MarkerCoord(){ x=-171.065f, y = 170.2808f, z = 131.7728f, icon =2, name="nw"},
                new MarkerCoord(){ x=-129.9426f, y = 170.2092f, z = 131.8336f, icon =3, name="ne"},
                new MarkerCoord(){ x=-130.633f, y = 129.8348f, z = 131.5248f, icon =4, name=" se"},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "Keep Construct",
            description = "Green Circles",
            mapId = 1156,
            trigger = new WorldCoord() { x = -81.049f, y = 228.662f, z = 148.556f },
            marks = new List<MarkerCoord> {
                new MarkerCoord(){ x=-81.049f, y=228.662f, z=148.556f, icon =1, name="DPS"},
                new MarkerCoord(){ x=-114.034f, y=229.050f, z=148.556f, icon =2, name="SupportDPS"},
                new MarkerCoord(){ x=-97.019f, y=255.827f, z=148.556f, icon =3, name="Healers"},
            }

        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "Xera",
            description = "Tanking Locations",
            mapId = 1156,
            trigger = new WorldCoord { x = -43.524f, y = -17.326f, z = 512.354f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=-55.524f, y = -92.326f, z = 512.354f, icon =1, name="Xera P2-1"},
                new MarkerCoord(){ x=-64.367f, y = -58.505f, z = 512.354f, icon =2, name="Xera P2-2"},
                new MarkerCoord(){ x=-101.757f, y = -80.51f, z = 512.356f, icon =3, name="Xera P2-3"},
                new MarkerCoord(){ x=-98.724f, y = -68.167f, z = 512.354f, icon =4, name="Xera P2-4"},
            }
        };
        SaveMarker(ms);


        ms = new MarkerSet
        {
            name = "Cairn",
            description = "Stack at arrow, Agony to other markers",
            mapId = 1188,
            trigger = new WorldCoord { x = 438.4015f, y = 34.58722f, z = 49.29768f },
            marks = new List<MarkerCoord>
            {
            new MarkerCoord(){ x=368.6021f, y = 52.26963f, z = 33.72034f, icon =1, name=""},
            new MarkerCoord(){ x=366.5356f, y = 44.91729f, z = 33.72039f, icon =2, name=""},
            new MarkerCoord(){ x=358.4903f, y = 49.50121f, z = 33.72039f, icon =3, name=""},
            new MarkerCoord(){ x=363.007f, y = 57.25174f, z = 33.72039f, icon =4, name=""},
            }
        };
        SaveMarker(ms);



        ms = new MarkerSet
        {
            name = "Dhuum",
            description = "Reaper/Green Locations",
            mapId = 1264,
            trigger = new WorldCoord { x = 351.7534f, y = 20.52355f, z = 159.6575f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=429.065f, y = 30.76263f, z = 157.8669f, icon =1, name=""},
                new MarkerCoord(){ x=427.8602f, y = 2.028903f, z = 157.867f, icon =2, name=""},
                new MarkerCoord(){ x=404.944f, y = -14.71885f, z = 157.8641f, icon =3, name=""},
                new MarkerCoord(){ x=377.8015f, y = -6.618744f, z = 157.8605f, icon =4, name=""},
                new MarkerCoord(){ x=365.9529f, y = 19.33479f, z = 157.8795f, icon =5, name=""},
                new MarkerCoord(){ x=378.9905f, y = 44.77333f, z = 157.8602f, icon =6, name=""},
                new MarkerCoord(){ x=407.5791f, y = 49.89919f, z = 157.864f, icon =7, name=""},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "Qadim",
            description = "Stab/Prot pyres, Heart:Lamp3, Square:portal/stack",
            mapId = 1303,
            trigger = new WorldCoord { x = -152.3928f, y = 285.804f, z = 120.1688f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=-146.6841f, y = 330.8714f, z = 120.1684f, icon =1, name="Stability Pyre,"},
                new MarkerCoord(){ x=-231.5254f, y = 331.0974f, z = 120.1693f, icon =2, name="Protection Pyre"},
                new MarkerCoord(){ x=-166.9229f, y = 295.0649f, z = 120.1684f, icon =3, name="Lamp3"},
                new MarkerCoord(){ x=-185.8315f, y = 294.9296f, z = 120.1688f, icon =4, name="Portal/Stack"},
            }
        };
        SaveMarker(ms);

        /*ms = new MarkerSet
        {
            name = "Cardinal Sabir",
            description = "",
            mapId = 1323,
            trigger = new WorldCoord(),
            marks = new List<MarkerCoord>
            {

            }
        };
        SaveMarker(ms);*/

        ms = new MarkerSet
        {
            name = "Cardinal Adina",
            description = "Basic Pillar Spawns",
            mapId = 1323,
            trigger = new WorldCoord { x = 399.4921f, y = -78.82588f, z = 4.237658f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=369.621f, y=-48.45922f, z=7.752968f, icon=1, name="SW" },
                new MarkerCoord(){ x=364.5227f, y=-35.13273f, z=7.752968f, icon=2, name="NW" },
                new MarkerCoord(){ x=378.7284f, y=-23.93113f, z=7.752968f, icon=3, name="N" },
                new MarkerCoord(){ x=392.8951f, y=-35.176f, z=7.752968f, icon=4, name="NE" },
                new MarkerCoord(){ x=387.7964f, y=-48.41273f, z=7.753212f, icon=5, name="SE" },
            }
        };
        SaveMarker(ms);

        /*ms = new MarkerSet
        {
            name = "Qadim The Peerless - complex",
            description = "First fire clockwise, second counter-clockwise",
            mapId = 1323,
            trigger = new WorldCoord { x = 65.85422f, y = 187.6384f, z = 16.6768f },
            marks = new List<MarkerCoord>
            {
            new MarkerCoord(){ x=22.89519f, y = 289.5503f, z = 16.81249f, icon =1, name=""},
            new MarkerCoord(){ x=59.32327f, y = 277.8169f, z = 16.81249f, icon =2, name=""},
            new MarkerCoord(){ x=79.97083f, y = 256.7198f, z = 16.81204f, icon =3, name=""},
            new MarkerCoord(){ x=51.55045f, y = 229.2897f, z = 16.81205f, icon =4, name=""},
            new MarkerCoord(){ x=22.88567f, y = 222.5481f, z = 16.81205f, icon =5, name=""},
            new MarkerCoord(){ x=13.20081f, y = 261.1675f, z = 16.81205f, icon =6, name=""},
            }
        };
        SaveMarker(ms);*/

        ms = new MarkerSet
        {
            name = "Qadim The Peerless (PUG Fires)",
            description = "First fires on marker, second on opposite side",
            mapId = 1323,
            trigger = new WorldCoord { x= 17.149731f, y = 187.959f, z = 16.6768f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=60.046f, y = 289.6017f, z = 16.81205f, icon =1, name=""},
                new MarkerCoord(){ x=3.21177f, y = 256.745f, z = 16.81205f, icon =2, name=""},
                new MarkerCoord(){ x=61.19013f, y = 223.1329f, z = 16.81205f, icon =3, name=""},


            }
        };
        SaveMarker(ms);


        ms = new MarkerSet
        {
            name = "IBS - Fraenir",
            description = "Safe stack spots - start at arrow",
            mapId = 1341,
            trigger = new WorldCoord { x = 55.65976f, y = -18.19911f, z = 179.7118f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=16.215f, y = -2.602f, z = 178.325f, icon =1, name=""},
                new MarkerCoord(){ x=11.345f, y = 1.620f, z = 178.325f, icon =2, name=""},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "IBS - Voice and Claw",
            description = "Stack Spots - start at arrow",
            mapId = 1346,
            trigger = new WorldCoord { x = 52.28861f, y = -21.5971f, z = 179.7112f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=8.01693599187199f, y = -10.2486639573279f, z = 178.338f, icon =1, name="VoiceBackStep"},
                new MarkerCoord(){ x=-8.690f, y = -5.287f, z = 178.538f, icon =2, name=""},
                new MarkerCoord(){ x=16.201f, y = 22.438f, z = 178.112f, icon =3, name=""},
            }
        };
        SaveMarker(ms);


/*        ms = new MarkerSet
        {
            name = "IBS - Cold War",
            description = "Boss spawn location",
            mapId = 1374,
            trigger = new WorldCoord { x = 104.9626f, y = -151.5361f, z = 18.22491f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=34.900f, y = -89.992f, z = 17.272f, icon =1, name="Varina"},
                //new MarkerCoord(){ x=29.084f, y = -51.671f, z = 18.002f, icon =1, name="//Champ spawn - toofar from door"},
	        }
        };
        SaveMarker(ms);*/


        ms = new MarkerSet
        {
            name = "EoD - Aetherblade CM Setup",
            description = "Assign color groups for green circles",
            mapId = 1432,
            trigger = new WorldCoord { x = 33.61228f, y = 33.58646f, z = 46.45338f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=46.50667f, y = 48.16092f, z = 46.28915f, icon =1, name="CM setup Arrow"},
                new MarkerCoord(){ x=49.2936f, y = 39.81492f, z = 45.88486f, icon =2, name="CM setup Circle"},
                new MarkerCoord(){ x=51.49508f, y = 30.82986f, z = 46.10174f, icon=3 , name="CM seteup heart"},
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
        {
            name = "EoD - Aetherblade CM Markers",
            description = "Destroy X, Stack at color groups",
            mapId = 1432,
            trigger = new WorldCoord { x = 58.76889f, y = 56.79849f, z = 45.76037f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=81.10351f, y = 55.76778f, z = 45.73673f, icon =1, name="CM Arrow"},
                new MarkerCoord(){ x=79.9226f, y = 48.54653f, z = 45.7366f, icon =2, name="CM Circle"},
                new MarkerCoord(){ x=86.92951f, y = 47.29297f, z = 45.73663f, icon =3, name="CM Heart"},

                new MarkerCoord(){ x=65.36295f, y = 32.88283f, z = 45.73687f, icon =8, name="X bomb"},
                //new MarkerCoord(){ x=79.75883f, y = 41.17043f, z = 45.73667f, icon =1, name="//right bomb"},
                //new MarkerCoord(){ x=65.31778f, y = 49.443f, z = 45.73673f, icon =1, name="//left bomb"},

	        }
        };
        SaveMarker(ms);


        //XJJ boss spawns
        //new MarkerCoord(){ x=-100.121f, y = 1.696f, z = 90.1249f, icon =1, name=""},
        //new MarkerCoord(){ x=53.688f, y = 32.763f, z = 116.172f, icon =1, name=""},
        //new MarkerCoord(){ x=-53.279f, y = 127.418f, z = 156.726f, icon =1, name=""},
        ms = new MarkerSet
        {
            name = "EoD Xunlai Jade Phase2",
            description = "Phase2CC",
            mapId = 1450,
            trigger = new WorldCoord { x = 28.72438f, y = 5.002136f, z = 118.4035f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=18.985f, y = 40.703f, z = 117.643f, icon =1, name=""},
                new MarkerCoord(){ x=88.307f, y = 37.673f, z = 118.695f, icon =2, name=""},
                new MarkerCoord(){ x=66.610f, y = -4.534f, z = 119.851f, icon =3, name=""},
            }
        };
        SaveMarker(ms);
        ms = new MarkerSet
        {
            name = "EoD Xunlai Jade Phase 3",
            description = "Phase3CC",
            mapId = 1450,
            trigger = new WorldCoord { x = -24.7418f, y = 123.3096f, z = 157.8843f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x=-78.353f, y = 136.421f, z = 157.493f, icon =1, name=""},
                new MarkerCoord(){ x=-44.588f, y = 108.224f, z = 157.411f, icon =2, name=""},
                new MarkerCoord(){ x=-41.937f, y = 151.549f, z = 157.431f, icon =3, name=""},
            }
        };
        SaveMarker(ms);


        /*ms = new MarkerSet
        {
            name = "eod_kaineng",
            description = "",
            mapId = 1451,
            trigger = new WorldCoord { x = -511.4021f, y = -346.3018f, z = 266.0575f },
            marks = new List<MarkerCoord>
            {

                new MarkerCoord(){ x = -597.3679f, y = -410.3128f, z = 274.5604f, icon =1, name=""}, //Li - normal back-center
                new MarkerCoord(){ x = -590.149f, y = -419.8066f, z = 274.5604f, icon =1, name=""}, // Li - backleft
                new MarkerCoord(){ x = -604.752f, y = -400.647f, z = 274.5742f, icon =1, name=""}, // Li - backright
                new MarkerCoord(){ x = -585.4771f, y = -386.282f, z = 274.5677f, icon =1, name=""},//Li - front-right
                new MarkerCoord(){ x = -570.8882f, y = -405.8025f, z = 274.5604f, icon =1, name=""}, //Li - front-left
                new MarkerCoord(){ x = -532.3221f, y = -389.4334f, z = 266.1935f, icon =1, name=""}, //left bait
                new MarkerCoord(){ x = -558.7625f, y = -353.8418f, z = 266.1942f, icon =1, name=""},//right bait
                new MarkerCoord(){ x = -560.0508f, y = -382.3326f, z = 266.1826f, icon =1, name=""}, //sniper-mech by zipline
            }
        };
        SaveMarker(ms);
        */

        ms = new MarkerSet
        {
            name = "EoD - Harvest Temple",
            description = "Boss location and Mordremoth stack",
            mapId = 1437,
            trigger = new WorldCoord { x = 14.88956f, y = -484.7346f, z = 391.7232f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){x=15.50351f, y=-554.128f, z=391.7127f, icon =8, name="Boss Spawn" },
                new MarkerCoord(){x=28.14585f, y=-551.954f, z=391.719f, icon =1, name="side-stack" },
            }
        };
        SaveMarker(ms);

        ms = new MarkerSet
            { 
                name = "Aerodrome Wing",
                description = "Demo of automarker feature",
                mapId = 1155,
                trigger = new WorldCoord
                {
                     x = 116.939705f, y= -366.006042f, z= 34.18379f
                },
                marks = new List<MarkerCoord>
                {
                    new MarkerCoord(){x = 59.378746f, y= -342.325073f, z= 34.1844864f, icon=1, name="Spirit Vale"},//W1
                    new MarkerCoord(){x = 67.14233f, y= -370.322571f, z= 39.56038f, icon=2, name="Salvation Pass"},//W2
                    new MarkerCoord(){x = 86.7961655f, y= -385.274139f, z= 34.18382f, icon=3, name="Stronghold of the Faithful"},//W3
                    new MarkerCoord(){x = 93.7534943f, y= -413.138275f, z= 39.59739f, icon=4, name="Bastion of the Penitent"},//W4
                    new MarkerCoord(){x = 126.134933f, y= -407.566254f,z=34.18382f, icon=5, name="Hall of Chains"},//W5
                    new MarkerCoord(){x = 161.817551f, y= -388.35556f, z= 34.2041321f, icon=6, name="Mythwright Gambit"},//W6
                    new MarkerCoord(){x = 156.105789f, y= -411.953827f, z= 39.6989861f, icon=7, name="The Key of Ahdashim"},//W7
                    new MarkerCoord(){x = 195.512543f, y= -324.111f, z= 39.075735f, icon=8, name="Special Forces Training"},//RT
                }

            };
            SaveMarker(ms);

       /* ms = new MarkerSet
        {
            name = "Arborstone Test",
            description = "Test marker keybinds",
            mapId = 1428,
            trigger = new WorldCoord { x = -783.0035f, y = 485.4295f, z = 1.958099f },
            marks = new List<MarkerCoord>
            {
                new MarkerCoord(){ x = -790.0322f, y = 485.1069f, z = 1.959626f, icon=1, name=""},
                new MarkerCoord(){ x = -786.5455f, y = 488.7949f, z = 1.958098f, icon=2, name=""},
                new MarkerCoord(){ x = -782.1669f, y = 493.2789f, z = 2.037168f, icon=3, name=""},
                new MarkerCoord(){ x = -777.7001f, y = 487.6703f, z = 1.958116f, icon=4, name=""},
                new MarkerCoord(){ x = -773.0765f, y = 482.8819f, z = 1.958126f, icon=5, name=""},
                new MarkerCoord(){ x = -776.7319f, y = 479.2359f, z = 1.956693f, icon=6, name=""},
                new MarkerCoord(){ x = -780.2802f, y = 475.6377f, z = 2.015322f, icon=7, name=""},
                new MarkerCoord(){ x = -784.8472f, y = 480.3472f, z = 1.96817f, icon=8, name=""},
                new MarkerCoord(){ x = -790.0322f, y = 485.1069f, z = 1.959626f, icon=1, name=""},
                new MarkerCoord(){ x = -786.5455f, y = 488.7949f, z = 1.958098f, icon=2, name=""},
                new MarkerCoord(){ x = -782.1669f, y = 493.2789f, z = 2.037168f, icon=3, name=""},
                new MarkerCoord(){ x = -777.7001f, y = 487.6703f, z = 1.958116f, icon=4, name=""},
                new MarkerCoord(){ x = -773.0765f, y = 482.8819f, z = 1.958126f, icon=5, name=""},
                new MarkerCoord(){ x = -776.7319f, y = 479.2359f, z = 1.956693f, icon=6, name=""},
                new MarkerCoord(){ x = -780.2802f, y = 475.6377f, z = 2.015322f, icon=7, name=""},
                new MarkerCoord(){ x = -784.8472f, y = 480.3472f, z = 1.96817f, icon=8, name=""},
                new MarkerCoord(){ x = -790.0322f, y = 485.1069f, z = 1.959626f, icon=1, name=""},
                new MarkerCoord(){ x = -786.5455f, y = 488.7949f, z = 1.958098f, icon=2, name=""},
                new MarkerCoord(){ x = -782.1669f, y = 493.2789f, z = 2.037168f, icon=3, name=""},
                new MarkerCoord(){ x = -777.7001f, y = 487.6703f, z = 1.958116f, icon=4, name=""},
                new MarkerCoord(){ x = -773.0765f, y = 482.8819f, z = 1.958126f, icon=5, name=""},
                new MarkerCoord(){ x = -776.7319f, y = 479.2359f, z = 1.956693f, icon=6, name=""},
                new MarkerCoord(){ x = -780.2802f, y = 475.6377f, z = 2.015322f, icon=7, name=""},
                new MarkerCoord(){ x = -784.8472f, y = 480.3472f, z = 1.96817f, icon=8, name=""},
                new MarkerCoord(){ x = -784.8472f, y = 480.3472f, z = 1.96817f, icon=9, name="clear"},
            }

        };
        SaveMarker(ms);*/
    }
}