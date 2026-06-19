using HarmonyLib;
using MBMScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAssetTestResource.Features;

public static class DatabaseReplace
{
    const string ModName = ModEntry.ModName;
    const string Log = ModEntry.ModName + "/Log";
    const string DiffFileName = "diff.log";

    private static bool EnableDiff => ModConfig.EnableOutputDiffLog;

    public static void AllGameManagerDatabaseReplace()
    {
        if(EnableDiff)
            JsonDiff.InitDiffFile(Log, DiffFileName);

        ReplaceDatabase<ConfigData>(ModName);
        ReplaceDatabase<TraitData>(ModName);

        ReplaceDatabase<HumanData>(ModName);
        ReplaceDatabase<ElfData>(ModName);
        ReplaceDatabase<DwarfData>(ModName);
        ReplaceDatabase<NekoData>(ModName);
        ReplaceDatabase<InuData>(ModName);
        ReplaceDatabase<UsagiData>(ModName);
        ReplaceDatabase<HitsujiData>(ModName);
        ReplaceDatabase<DragonianData>(ModName);

        ReplaceDatabase<SuccubusData>(ModName);

        ReplaceDatabase<SylviaData>(ModName);
        ReplaceDatabase<ClaireData>(ModName);
        ReplaceDatabase<AureData>(ModName);
        ReplaceDatabase<KarenData>(ModName);
        ReplaceDatabase<ViviData>(ModName);
        ReplaceDatabase<BellaData>(ModName);
        ReplaceDatabase<AnnaData>(ModName);
        ReplaceDatabase<NeroData>(ModName);

        ReplaceDatabase<AmiliaData>(ModName);
        ReplaceDatabase<FloraData>(ModName);
        ReplaceDatabase<NielData>(ModName);
        ReplaceDatabase<SenaData>(ModName);
        ReplaceDatabase<LenaData>(ModName);
        ReplaceDatabase<BarbaraData>(ModName);

        ReplaceDatabase<PlayerCharacterData>(ModName);
        ReplaceDatabase<ClientData>(ModName);
        ReplaceDatabase<HorseData>(ModName);

        ReplaceDatabase<GoblinData>(ModName);
        ReplaceDatabase<OrcData>(ModName);
        ReplaceDatabase<WerewolfData>(ModName);
        ReplaceDatabase<MinotaurData>(ModName);
        ReplaceDatabase<SalamanderData>(ModName);

        ReplaceDatabase<TentacleData>(ModName);

        // Additional branches: Items, Events, Rooms, Achievements, etc.
        ReplaceDatabase<ItemData>(ModName);
        ReplaceDatabase<EventData>(ModName);
        ReplaceDatabase<RoomData>(ModName);
        ReplaceDatabase<UpgradeData>(ModName);
        ReplaceDatabase<AchievementData>(ModName);
        ReplaceDatabase<LikeabilityData>(ModName);
        ReplaceDatabase<MakingData>(ModName);
        ReplaceDatabase<Niel1Data>(ModName);
        ReplaceDatabase<SpineData>(ModName);
    }

    private static void ReplaceDatabase<T>(string modName) where T : Data
    {
        if (TryLoadData<T>(modName, out var wrapper))
        {
            var list = wrapper?.list ?? new List<T>();
            var nameDict = new Dictionary<string, T>(list.Count);
            var idDict = new Dictionary<int, T>(list.Count);

            foreach (var entry in list)
            {
                if (!string.IsNullOrEmpty(entry.DataName))
                    nameDict[entry.DataName] = entry;
                idDict[entry.DataId] = entry;
            }

            var trv = Traverse.Create(typeof(Database<T>));
            trv.Field("List").SetValue(list);
            trv.Field("DataNameDictionary").SetValue(nameDict);
            trv.Field("DataIdDictionary").SetValue(idDict);

            ModEntry.Log($"Database<{typeof(T).Name}> has been replaced with custom JSON data");
            return;
        }
        //else
        //{
        //    ModEntry.Log($"External {typeof(T).Name}.json not found, keeping original data");
        //}
    }

    private static bool TryLoadData<T>(string modName, out SeqJsonListWrapper<T>? wrapper) where T : Data
    {
        wrapper = null;
        string fileName = typeof(T).Name + ".json";

        if (!ConfigSystem.TryLoadExternalFile(modName, fileName, out var asset))
            return false;

        try
        {
            bool changed = CompareWithResources<T>(asset!, fileName);

            if (!changed)
                return false;

            wrapper = JsonUtility.FromJson<SeqJsonListWrapper<T>>(asset!.text);
            return wrapper != null;
        }
        catch (Exception ex)
        {
            ModEntry.LogError($"[ConfigSystem] Failed to parse {fileName}: {ex.Message}");
            return false;
        }
    }

    private static bool CompareWithResources<T>(TextAsset externalasset, string fileName) where T : Data
    {
        TextAsset? internalAsset = Resources.Load<TextAsset>(typeof(T).Name);
        if (internalAsset == null)
        {
            ModEntry.LogWarning($"[Missing] {fileName} not found in Resources.");
            return false;
        }

        string internalContent = UnwrapList(internalAsset.text);
        string externalContent = UnwrapList(externalasset.text);

        if (EnableDiff)
            return JsonDiff.DiffJsonListAuto(internalContent, externalContent, fileName, Log, DiffFileName);
        else
            return internalContent == externalContent;
    }

    private static string UnwrapList(string json)
    {
        try
        {
            var jObj = JObject.Parse(json);
            if (jObj.TryGetValue("list", out var token) && token is JArray arr)
            {
                return arr.ToString(Formatting.None);
            }
        }
        catch{}
        return json;
    }
}