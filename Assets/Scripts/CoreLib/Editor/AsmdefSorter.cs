using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Corelib.Utils
{
    public static class AsmdefSorter
    {
        [MenuItem("Game/Project/Sort Asmdef References Alphabetically")]
        public static void SortAsmdefReferences()
        {
            string[] asmdefPaths = Directory.GetFiles("Assets", "*.asmdef", SearchOption.AllDirectories);

            foreach (string path in asmdefPaths)
            {
                string json = File.ReadAllText(path);
                var root = JObject.Parse(json);

                bool updated = false;

                string fileName = Path.GetFileName(path);
                bool isEditorOnlyDefinition = !string.IsNullOrEmpty(fileName) && fileName.Contains(".Editor");
                if (isEditorOnlyDefinition)
                {
                    var includePlatforms = new JArray("Editor");
                    if (!JToken.DeepEquals(root["includePlatforms"], includePlatforms))
                    {
                        root["includePlatforms"] = includePlatforms;
                        updated = true;
                    }

                    var excludePlatforms = new JArray();
                    if (!JToken.DeepEquals(root["excludePlatforms"], excludePlatforms))
                    {
                        root["excludePlatforms"] = excludePlatforms;
                        updated = true;
                    }
                }

                var refsToken = root["references"] as JArray;
                if (refsToken != null && refsToken.Count > 1)
                {
                    var references = refsToken.Select(token => token.ToString()).ToList();
                    var sortedReferences = references
                        .OrderBy(reference => GetDisplayName(reference), System.StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (!references.SequenceEqual(sortedReferences))
                    {
                        root["references"] = new JArray(sortedReferences);
                        updated = true;
                    }
                }

                if (updated)
                {
                    File.WriteAllText(path, root.ToString(Newtonsoft.Json.Formatting.Indented));
                    Debug.Log($"Updated asmdef configuration in {path}");
                }
            }

            AssetDatabase.Refresh();
        }

        private static string GetDisplayName(string reference)
        {
            const string guidPrefix = "GUID:";
            if (reference.StartsWith(guidPrefix))
            {
                var guid = reference.Substring(guidPrefix.Length);
                var targetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
                    return reference;

                try
                {
                    var targetJson = File.ReadAllText(targetPath);
                    var targetRoot = JObject.Parse(targetJson);
                    var name = targetRoot["name"]?.ToString();
                    return string.IsNullOrEmpty(name) ? reference : name;
                }
                catch
                {
                    return reference;
                }
            }
            return reference;
        }
    }
}
