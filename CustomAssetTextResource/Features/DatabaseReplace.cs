using CustomAssetTextResource.Core;
using CustomAssetTextResource.Utils;
using HarmonyLib;
using MBMScripts;
using System;
using System.Collections.Generic;
using System.IO;
using SystemExtensionLib.Systems;
using UnityEngine;

namespace CustomAssetTextResource.Features;

public static class DatabaseReplace
{
    const string ModName = ModEntry.ModName;
    const string DiffFileName = "diff.log";
    static readonly string Log = Path.Combine(ModEntry.ModName, "Log");

    private static bool EnableDiff => ModConfig.EnableOutputDiffLog;

    public static void ExportAllResourcesTextAssetFile()
    {
        string export = Path.Combine(ModEntry.ModName, "Export");
        string exportSpeciesData = Path.Combine(export, "Species");
        string exportSlaveNPCData = Path.Combine(export, "SlaveNPC");
        string exportNPCData = Path.Combine(export, "NPC");
        string exportMaleData = Path.Combine(export, "Male");

        ConfigSystem.ExportResourcesTextAssetFile(export, "ConfigData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "TraitData");

        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "HumanData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "ElfData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "DwarfData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "NekoData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "InuData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "UsagiData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "HitsujiData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "DragonianData");

        ConfigSystem.ExportResourcesTextAssetFile(exportSpeciesData, "SuccubusData");

        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "SylviaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "ClaireData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "AureData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "KarenData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "ViviData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "BellaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "AnnaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportSlaveNPCData, "NeroData");

        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "AmiliaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "FloraData");
        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "NielData");
        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "SenaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "LenaData");
        ConfigSystem.ExportResourcesTextAssetFile(exportNPCData, "BarbaraData");

        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "PlayerCharacterData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "ClientData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "HorseData");

        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "GoblinData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "OrcData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "WerewolfData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "MinotaurData");
        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "SalamanderData");

        ConfigSystem.ExportResourcesTextAssetFile(exportMaleData, "TentacleData");

        // Additional branches: Items, Events, Rooms, Achievements, etc.
        ConfigSystem.ExportResourcesTextAssetFile(export, "ItemData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "EventData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "RoomData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "UpgradeData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "AchievementData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "LikeabilityData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "MakingData");
        ConfigSystem.ExportResourcesTextAssetFile(export, "Niel1Data");
        ConfigSystem.ExportResourcesTextAssetFile(export, "SpineData");
    }

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

        if (!ConfigSystem.TryLoadExternalConfig(modName, fileName, out var asset))
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

        string internalContent = JsonDiff.UnwrapList(internalAsset.text);
        string externalContent = JsonDiff.UnwrapList(externalasset.text);

        if (EnableDiff)
            return JsonDiff.DiffJsonListAuto(internalContent, externalContent, fileName, Log, DiffFileName);
        else
            return internalContent == externalContent;
    }
}