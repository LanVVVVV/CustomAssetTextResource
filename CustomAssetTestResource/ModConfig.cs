using CustomAssetTestResource.Properties;
using MBM.ModLoader.Settings;

namespace CustomAssetTestResource;

public static class ModConfig
{
    private const string NameExportAllResourcesTextAsset = "Export All Resources TextAsset";
    private const string NameOutputDiffLog = "Export Diff Log";

    public static bool EnableExportAllResourcesTextAsset { get; set; }

    public static bool EnableOutputDiffLog { get; set; }

    public static void ModSettingRegister()
    {
        ExportAllResourcesTextAssetRegister();
        ExportOutputDiffLog();
    }

    private static void ExportOutputDiffLog()
    {
        ModSettings.RegisterBool(ModEntry.ModName, NameOutputDiffLog, false, Strings.Config_OutputDiffLog);
        EnableOutputDiffLog = ModSettings.GetBool(ModEntry.ModName, NameOutputDiffLog);
        ModSettings.OnChanged(ModEntry.ModName, NameOutputDiffLog, v =>
        {
            if (EnableOutputDiffLog == (bool)v) return;
            EnableOutputDiffLog = (bool)v;
            if (EnableOutputDiffLog)
            ModEntry.Log($"{NameOutputDiffLog} = {EnableOutputDiffLog}");
        });
    }

    private static void ExportAllResourcesTextAssetRegister()
    {
        ModSettings.RegisterBool(ModEntry.ModName, NameExportAllResourcesTextAsset, false, Strings.Config_ExportAllResourcesTextAsset);
        EnableExportAllResourcesTextAsset = ModSettings.GetBool(ModEntry.ModName, NameExportAllResourcesTextAsset);
        ModSettings.OnChanged(ModEntry.ModName, NameExportAllResourcesTextAsset, v =>
        {
            if (EnableExportAllResourcesTextAsset == (bool)v) return;
            EnableExportAllResourcesTextAsset = (bool)v;
            if (EnableExportAllResourcesTextAsset)
                ExportAllResourcesTextAssetFile();
            ModEntry.Log($"{NameExportAllResourcesTextAsset} = {EnableExportAllResourcesTextAsset}");
        });
    }

    public static void ExportAllResourcesTextAssetFile()
    {
        string export = ModEntry.ModName + "/Export";
        string exportSpeciesData = export + "/Species";
        string exportSlaveNPCData = export + "/SlaveNPC";
        string exportNPCData = export + "/NPC";
        string exportMaleData = export + "/Male";

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

    public static void OnLanguageChanged()
    {
        ModSettings.SetDescription(ModEntry.ModName, NameExportAllResourcesTextAsset, Strings.Config_ExportAllResourcesTextAsset);
        ModSettings.SetDescription(ModEntry.ModName, NameOutputDiffLog, Strings.Config_OutputDiffLog);

    }
}

