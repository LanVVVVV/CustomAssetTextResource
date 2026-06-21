using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemExtensionLib.Systems;

namespace CustomAssetTextResource.Utils;

public static class JsonDiff
{
    public static void InitDiffFile(string modName, string diffFileName)
    {
        string? diffFilePath = ConfigSystem.GetConfigPath(modName, diffFileName);
        if (diffFilePath == null) return;

        using (var writer = new StreamWriter(diffFilePath, append: false))
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"===== Diff Log Initialized at {timestamp} =====");
            writer.WriteLine();
        }
    }

    public static bool DiffJsonListAuto(string internalJson, string externalJson, string fileName, string modName, string diffFileName)
    {
        string? diffFilePath = ConfigSystem.GetConfigPath(modName, diffFileName);
        if (diffFilePath == null) return false;

        var internalList = ParseObjectList(internalJson);
        var externalList = ParseObjectList(externalJson);

        var diffs = new List<string> { $"===== Diff for {fileName} =====" };

        bool changed;

        if (fileName.Equals("SpineData.json", StringComparison.OrdinalIgnoreCase))
        {
            diffFileName = "SpineData_" + diffFileName;
            InitDiffFile(modName, diffFileName);

            string? spineDiffPath = ConfigSystem.GetConfigPath(modName, diffFileName);
            if (spineDiffPath == null) return false;

            changed = DiffSpineData(internalList, externalList, diffs);
            WriteDiffToFile(spineDiffPath, diffs);
            return changed;
        }

        if (internalList.Count > 1 || externalList.Count > 1)
        {
            bool allHaveId = 
                internalList.All(o => o.Contains("\"m_DataId\"")) &&
                externalList.All(o => o.Contains("\"m_DataId\""));

            if (allHaveId)
                changed = DiffListWithDataId(internalList, externalList, diffs);
            else
            {
                diffs.Add("[Error] Multiple objects but no DataId.");
                changed = true;
            }
        }
        else
        {
            changed = DiffSingleObject(internalList, externalList, diffs);
        }

        WriteDiffToFile(diffFilePath, diffs);
        return changed;
    }
    public static string UnwrapList(string json)
    {
        try
        {
            var jObj = JObject.Parse(json);
            if (jObj.TryGetValue("list", out var token) && token is JArray arr)
            {
                return arr.ToString(Formatting.None);
            }
        }
        catch { }
        return json;
    }

    #region SpineData
    private static bool DiffSpineData(List<string> internalList, List<string> externalList, List<string> diffs)
    {
        diffs.Add($"===== Group: figure0Name|figure1Name =====");
        diffs.Add($"===== key: figure0Name|figure0StateNameList|figure1Name|figure1StateNameList|roomTypeNameList|poseTypeList =====");

        var internalDict = internalList.ToDictionary(s => GetSpineKey(s));
        var externalDict = externalList.ToDictionary(s => GetSpineKey(s));

        bool changed = false;

        // figure0Name|figure1Name
        var allGroups = internalDict.Keys.Concat(externalDict.Keys)
            .Select(k => $"{k.Split('|')[0]}|{k.Split('|')[2]}")
            .Distinct()
            .OrderBy(g => g);

        foreach (var group in allGroups)
        {
            var groupDiffs = new List<string>();

            var internalKeys = internalDict.Keys
                .Where(k => $"{k.Split('|')[0]}|{k.Split('|')[2]}" == group);
            var externalKeys = externalDict.Keys
                .Where(k => $"{k.Split('|')[0]}|{k.Split('|')[2]}" == group);

            foreach (var key in internalKeys)
            {
                if (!externalDict.ContainsKey(key))
                {
                    groupDiffs.Add($"  [Removed] {key}");
                    changed = true;
                }
                else if (internalDict[key] != externalDict[key])
                {
                    groupDiffs.Add($"  [Modified] {key}");
                    DiffObjectFields(internalDict[key], externalDict[key], groupDiffs, 2);
                    changed = true;
                }
            }

            foreach (var key in externalKeys.Except(internalKeys))
            {
                groupDiffs.Add($"  [Added] {key}");
                DiffObjectFields(null, externalDict[key], groupDiffs, 2);
                changed = true;
            }

            if (groupDiffs.Count > 0)
            {
                diffs.Add($"[Group] {group}");
                diffs.AddRange(groupDiffs);
            }
        }

        if (!changed) diffs.Add("[No Change]");
        return changed;
    }

    private static string GetSpineKey(string json)
    {
        var obj = JObject.Parse(json);

        string figure0 = obj["figure0Name"]?.ToString() ?? "";
        string figure1 = obj["figure1Name"]?.ToString() ?? "";

        string state0 = GetArrayKey(obj["figure0StateNameList"]);
        string state1 = GetArrayKey(obj["figure1StateNameList"]);
        string room = GetArrayKey(obj["roomTypeNameList"]);
        string pose = GetArrayKey(obj["poseTypeList"]);

        return $"{figure0}|{state0}|{figure1}|{state1}|{room}|{pose}";
    }

    private static string GetArrayKey(JToken? token)
    {
        if (token == null || !token.Any()) return "null";
        return string.Join(",", token.Select(s => s.ToString()).OrderBy(s => s));
    }
    #endregion

    private static bool DiffListWithDataId(List<string> internalList, List<string> externalList, List<string> diffs)
    {
        var internalDict = new Dictionary<string, string>();
        foreach (var obj in internalList)
        {
            var dict = ParseObjectToDict(obj);
            if (dict.TryGetValue("m_DataId", out var id))
                internalDict[id] = obj;
            else
                diffs.Add("[Error] Internal object missing DataId.");
        }

        var externalDict = new Dictionary<string, string>();
        foreach (var obj in externalList)
        {
            var dict = ParseObjectToDict(obj);
            if (dict.TryGetValue("m_DataId", out var id))
                externalDict[id] = obj;
            else
                diffs.Add("[Error] External object missing DataId.");
        }

        bool changed = false;

        foreach (var kv in internalDict)
        {
            if (!externalDict.ContainsKey(kv.Key))
            {
                diffs.Add($"[Removed] m_DataId={kv.Key}");
                changed = true;
            }
            else if (kv.Value != externalDict[kv.Key])
            {
                diffs.Add($"[Modified] m_DataId={kv.Key}");
                DiffObjectFields(kv.Value, externalDict[kv.Key], diffs, 1);
                changed = true;
            }
        }

        foreach (var kv in externalDict)
        {
            if (!internalDict.ContainsKey(kv.Key))
            {
                diffs.Add($"[Added] m_DataId={kv.Key}");
                DiffObjectFields(null, kv.Value, diffs, 1);
                changed = true;
            }
        }

        if (!changed)
            diffs.Add("[No Change]");
        return changed;
    }

    private static bool DiffSingleObject(List<string> internalList, List<string> externalList, List<string> diffs)
    {
        string internalJson = internalList.FirstOrDefault() ?? "{}";
        string externalJson = externalList.FirstOrDefault() ?? "{}";

        if (internalJson == externalJson)
        {
            diffs.Add("[No Change]");
            return false;
        }

        DiffObjectFields(internalJson, externalJson, diffs);
        return true;
    }

    private static void DiffObjectFields(string? internalJsonObject, string? externalJsonObject, List<string> diffs, int indentLevel = 0)
    {
        var indent = new string(' ', indentLevel * 2);

        var internalObj = internalJsonObject is null
            ? new JObject()
            : JObject.Parse(internalJsonObject);

        var externalObj = externalJsonObject is null
            ? new JObject()
            : JObject.Parse(externalJsonObject);


        foreach (var prop in internalObj.Properties())
        {
            var externalProp = externalObj.Property(prop.Name);
            if (externalProp == null)
            {
                diffs.Add($"{indent}[Removed] \"{prop.Name}\": {prop.Value.ToString(Formatting.None)}");
            }
            else if (!JToken.DeepEquals(prop.Value, externalProp.Value))
            {
                diffs.Add($"{indent}[Modified] \"{prop.Name}\": {prop.Value.ToString(Formatting.None)} => {externalProp.Value.ToString(Formatting.None)}");
            }
        }

        foreach (var prop in externalObj.Properties())
        {
            if (internalObj.Property(prop.Name) == null)
            {
                diffs.Add($"{indent}[Added] \"{prop.Name}\": {prop.Value.ToString(Formatting.None)}");
            }
        }
    }

    private static List<string> ParseObjectList(string json)
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(json))
            return list;

        json = json.Trim();
        if (json.StartsWith("["))
        {
            try
            {
                var jArray = JArray.Parse(json);
                foreach (var item in jArray)
                    list.Add(item.ToString(Formatting.None));
                return list;
            }
            catch { return list; }
        }

        if (json.StartsWith("{"))
        {
            try
            {
                var jObj = JObject.Parse(json);
                list.Add(jObj.ToString(Formatting.None));
            }
            catch { }
        }
        return list;
    }

    private static Dictionary<string, string> ParseObjectToDict(string json)
    {
        var dict = new Dictionary<string, string>();
        var jObj = JObject.Parse(json);

        foreach (var prop in jObj.Properties())
        {
            dict[prop.Name] = prop.Value.Type == JTokenType.Object || prop.Value.Type == JTokenType.Array
                ? prop.Value.ToString(Newtonsoft.Json.Formatting.None)
                : prop.Value.ToString();
        }

        return dict;
    }

    private static void WriteDiffToFile(string diffFilePath, List<string> diffs)
    {
        using (var writer = new StreamWriter(diffFilePath, append: true))
        {
            foreach (var line in diffs)
                writer.WriteLine(line);
            writer.WriteLine();
        }
    }
}
