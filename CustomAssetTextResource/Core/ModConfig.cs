using CustomAssetTextResource.Features;
using CustomAssetTextResource.Properties;
using MBM.ModLoader.Settings;

namespace CustomAssetTextResource.Core;

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
                DatabaseReplace.ExportAllResourcesTextAssetFile();
            ModEntry.Log($"{NameExportAllResourcesTextAsset} = {EnableExportAllResourcesTextAsset}");
        });
    }

    public static void OnLanguageChanged()
    {
        ModSettings.SetDescription(ModEntry.ModName, NameExportAllResourcesTextAsset, Strings.Config_ExportAllResourcesTextAsset);
        ModSettings.SetDescription(ModEntry.ModName, NameOutputDiffLog, Strings.Config_OutputDiffLog);
    }
}

