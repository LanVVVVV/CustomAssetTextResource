# CustomAssetTestResource

CustomAssetTestResource is a mod for **Monster Black Market** —— allow loading modified TextAssets without unpacking resources.assets.

### Features

* Export all available TextAssets from resources.assets into Config\CustomAssetTestResource\Export through mod settings.
* Load JSON TextAssets from Config\CustomAssetTestResource to replace game data.
* Output detailed diff logs to Config\CustomAssetTestResource\Log through mod settings.

### Notes

* Remember to restart the game after modifying the JSON TextAssets file.
* Although the exported JSON files are categorized into folders, you don’t need to keep the folder structure when placing them into Config\CustomAssetTestResource.

### Requirements

| Dependency           | Version    |
| -------------------- | ---------- |
| MBM.ModLoader        | ≥ 0.6.0    |
| Monster Black Market | ≥ 2.1.2.0  |