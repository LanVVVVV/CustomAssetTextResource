using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomAssetTestResource;

public static class ConfigSystem
{
    private static readonly string RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");

    /// <summary>
    /// Initializes the config system by ensuring the Config directory exists.
    /// </summary>
    public static void Initialize()
    {
        try
        {
            if (!Directory.Exists(RootDir))
                Directory.CreateDirectory(RootDir);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to initialize config root dir: {ex.Message}");
        }
    }

    /// <summary>
    /// Exports a single embedded resource file to the external Config directory.
    /// The modName supports both single-level and multi-level directories.
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// All necessary subdirectories will be created automatically.
    /// </param>
    /// <param name="fileName">
    /// The name of the embedded resource file to export.
    /// </param>
    /// <param name="overwritePredicate">
    /// Delegate to decide overwrite behavior.  
    /// Receives the target file path, returns true if the file should be overwritten.  
    /// If null, defaults to skipping existing files.
    /// </param>
    public static void ExportEmbeddedFile(string modName, string fileName, Func<string, bool>? overwritePredicate = null)
    {
        try
        {
            string? modDir = EnsureModDir(modName);
            if (modDir == null) return;

            string filePath = Path.Combine(modDir, fileName);

            var asm = Assembly.GetCallingAssembly();
            var resourceName = asm.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.Ordinal));

            if (resourceName == null)
            {
                Debug.LogWarning($"[ConfigSystem] Embedded resource not found: {fileName}");
                return;
            }

            bool shouldOverwrite = overwritePredicate?.Invoke(fileName) ?? false;

            if (shouldOverwrite || !File.Exists(filePath))
            {
                using var stream = asm.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream);
                File.WriteAllText(filePath, reader.ReadToEnd());

                string relativeDir = Path.Combine("Config", modName);
                Debug.Log($"[ConfigSystem] Exported embedded {fileName} to {relativeDir}" +
                    (shouldOverwrite ? " (overwritten)" : ""));
            }
            else
            {
                string relativePath = Path.Combine("Config", modName, fileName);
                Debug.Log($"[ConfigSystem] External file already exists: {relativePath}, skipped export.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to export embedded {fileName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Exports all embedded resource files for the given mod to the external Config directory.
    /// The modName supports both single-level and multi-level directories.
    /// Optionally filters by file extension.
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// All necessary subdirectories will be created automatically.
    /// </param>
    /// <param name="extensionFilter">
    /// Optional file extension filter (e.g., ".json"). Only resources ending with this extension will be exported.
    /// </param>
    /// <param name="overwritePredicate">
    /// Delegate to decide overwrite behavior.  
    /// Receives the target file path, returns true if the file should be overwritten.  
    /// If null, defaults to skipping existing files.
    /// </param>
    public static void ExportAllEmbeddedFile(string modName, string? extensionFilter = null, Func<string, bool>? overwritePredicate = null)
    {
        try
        {
            string? modDir = EnsureModDir(modName);
            if (modDir == null) return;


            var asm = Assembly.GetCallingAssembly();
            var resources = asm.GetManifestResourceNames();

            if (!string.IsNullOrEmpty(extensionFilter))
                resources = resources.Where(n => n.EndsWith(extensionFilter, StringComparison.Ordinal)).ToArray();

            foreach (var res in resources)
            {
                string[] parts = res.Split('.');
                string fileName = string.Join(".", parts.Skip(parts.Length - 2));

                string filePath = Path.Combine(modDir, fileName);

                bool shouldOverwrite = overwritePredicate?.Invoke(fileName) ?? false;

                if (shouldOverwrite || !File.Exists(filePath))
                {
                    using var stream = asm.GetManifestResourceStream(res);
                    using var reader = new StreamReader(stream);
                    File.WriteAllText(filePath, reader.ReadToEnd());

                    string relativeDir = Path.Combine("Config", modName);
                    Debug.Log($"[ConfigSystem] Exported embedded {fileName} to {relativeDir}" +
                        (shouldOverwrite ? " (overwritten)" : ""));
                }
                else
                {
                    string relativePath = Path.Combine("Config", modName, fileName);
                    Debug.Log($"[ConfigSystem] External file already exists: {relativePath}, skipped export.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to export all embedded files: {ex.Message}");
        }
    }

    /// <summary>
    /// Exports a Unity Resources TextAsset file to the external Config directory.
    /// The modName supports both single-level and multi-level directories.
    /// If the file already exists, compares content hashes and overwrites only if changed.
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// All necessary subdirectories will be created automatically.
    /// </param>
    /// <param name="fileNameWithoutExt">
    /// The resource file name without extension. The system assumes ".json".
    /// </param>
    public static void ExportResourcesTextAssetFile(string modName, string fileNameWithoutExt)
    {
        try
        {
            TextAsset asset = Resources.Load<TextAsset>(fileNameWithoutExt);
            if (asset == null)
            {
                Debug.LogWarning($"[ConfigSystem] Resource not found: {fileNameWithoutExt}");
                return;
            }

            string? modDir = EnsureModDir(modName);
            if (modDir == null) return;


            string filePath = Path.Combine(modDir, fileNameWithoutExt + ".json");

            string newContent = asset.text;

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, newContent);
                Debug.Log($"[ConfigSystem] Exported new resource {fileNameWithoutExt}.json to Config/{modName}");
                return;
            }

            string oldContent;
            string oldHash = string.Empty;

            try
            {
                oldContent = File.ReadAllText(filePath);
                oldHash = ComputeHash(oldContent);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[ConfigSystem] Failed to read existing {fileNameWithoutExt}.json, will overwrite. Error: {ex.Message}");
                File.WriteAllText(filePath, newContent);
                Debug.Log($"[ConfigSystem] Resource {fileNameWithoutExt}.json overwritten in Config/{modName}");
                return;
            }

            string newHash = ComputeHash(newContent);

            if (oldHash == newHash)
            {
                Debug.Log($"[ConfigSystem] Resource {fileNameWithoutExt}.json unchanged, skip overwrite.");
                return;
            }

            File.WriteAllText(filePath, newContent);
            Debug.Log($"[ConfigSystem] Resource {fileNameWithoutExt}.json changed, overwritten in Config/{modName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to export resource {fileNameWithoutExt}: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads an embedded resource file as a TextAsset.
    /// </summary>
    /// <param name="fileName">
    /// The name of the embedded resource file.
    /// </param>
    /// <returns>
    /// A TextAsset containing the file content, or null if not found.
    /// </returns>
    public static TextAsset? LoadEmbeddedFile(string fileName)
    {
        var asm = Assembly.GetCallingAssembly();
        var resourceName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.Ordinal));

        if (resourceName == null)
        {
            Debug.LogWarning($"[ConfigSystem] Embedded resource not found: {fileName}");
            return null;
        }

        using var stream = asm.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return new TextAsset(reader.ReadToEnd());
    }

    /// <summary>
    /// Gets the full file path in the Config directory for the given modName and fileName.
    /// The modName supports both single-level and multi-level directories.
    /// Ensures the mod directory exists, but does not create the file.
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// </param>
    /// <param name="fileName">
    /// The name of the external file to load.
    /// </param>
    public static string? GetFilePath(string modName, string fileName)
    {
        try
        {
            string? modDir = EnsureModDir(modName);
            if (modDir == null) return null;

            return Path.Combine(modDir, fileName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to get file path for {modName}/{fileName}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Creates an empty file in the specified mod directory and returns its full path.
    /// The modName supports both single-level and multi-level directories.
    /// If the file already exists, it will be cleared (overwritten with empty content).
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// </param>
    /// <param name="fileName">
    /// The name of the external file to load.
    /// </param>
    public static string? CreateEmptyFile(string modName, string fileName)
    {
        try
        {
            string? filePath = GetFilePath(modName, fileName);
            if (filePath == null) return null;

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)){}

            Debug.Log($"[ConfigSystem] Created empty file: {Path.Combine("Config", modName, fileName)}");
            return filePath;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to create empty file {fileName}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Loads an external config file from the Config directory.
    /// The modName supports both single-level and multi-level directories.
    /// This method produces debug log output on success or failure.
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// </param>
    /// <param name="fileName">
    /// The name of the external file to load.
    /// </param>
    /// <returns>
    /// A TextAsset containing the file content, or null if not found.
    /// </returns>
    public static TextAsset? LoadExternalFile(string modName, string fileName)
    {
        if (TryLoadExternalFile(modName, fileName, out var asset))
        {
            Debug.Log($"[ConfigSystem] Loaded external file: {fileName} from Config/{modName}");
            return asset;
        }

        Debug.LogWarning($"[ConfigSystem] External file not found: {fileName}");
        return null;
    }

    /// <summary>
    /// Attempts to load an external config file quietly without debug log output.
    /// The modName supports both single-level and multi-level directories.
    /// </summary>
    /// <param name="modName">
    /// The mod name. It can be a single-level directory ("MyMod")
    /// or a multi-level directory ("MyMod/SubConfig").
    /// Path separators inside the string are handled automatically by Path.Combine.
    /// <param name="fileName">
    /// The name of the external file to load.
    /// </param>
    /// <param name="asset">
    /// Output parameter containing the loaded TextAsset if successful.
    /// </param>
    /// <returns>
    /// True if the file was successfully loaded, otherwise false.
    /// </returns>
    public static bool TryLoadExternalFile(string modName, string fileName, out TextAsset? asset)
    {
        try
        {
            string? modDir = EnsureModDir(modName);
            if (modDir == null)
            {
                asset = null;
                return false;
            }

            string filePath = Path.Combine(modDir, fileName);

            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                asset = new TextAsset(content);
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to load external file {fileName}: {ex.Message}");
        }

        asset = null;
        return false;
    }

    private static string? EnsureModDir(string modName)
    {
        try
        {
            Initialize();

            string modDir = Path.Combine(RootDir, modName);
            if (!Directory.Exists(modDir))
                Directory.CreateDirectory(modDir);

            return modDir;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ConfigSystem] Failed to ensure mod directory for {modName}: {ex.Message}");
            return null;
        }
    }

    private static string ComputeHash(string text)
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hash = sha1.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "");
    }
}
