# CustomAssetTextResource

CustomAssetTextResource is a mod for **Monster Black Market** —— allow loading modified TextAssets without unpacking resources.assets.

### Features

* Export all available TextAssets from resources.assets into Config\CustomAssetTextResource\Export through mod settings.
* Load JSON TextAssets from Config\CustomAssetTextResource to replace game data.
* Output detailed diff logs to Config\CustomAssetTextResource\Log through mod settings.

### Notes

* Remember to restart the game after modifying the JSON TextAssets file.
* Although the exported JSON files are categorized into folders, you don’t need to keep the folder structure when placing them into Config\CustomAssetTextResource.

### Requirements

| Dependency           | Version    |
| -------------------- | ---------- |
| MBM.ModLoader        | ≥ 0.6.0    |
| Monster Black Market | ≥ 2.1.2.0  |
| SystemExtensionLib   | ≥ 1.0.0    | [GitHub](https://github.com/LanVVVVV/SystemExtensionLib) |