// TMP Font Replacer (Editor)
// Unity 2022.3 LTS compatible. Replaces all TextMeshPro fonts across scenes & prefabs.
// Place this script under an Editor folder: Assets/Editor/TMPFontReplacer.cs

using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Corelib.Utils
{
    public class TMPFontReplacerWindow : EditorWindow
    {
        [MenuItem("Tools/TMP/Font Replacer")]
        private static void Open() => GetWindow<TMPFontReplacerWindow>(true, "TMP Font Replacer");

        [SerializeField] private TMP_FontAsset targetFont;

        private bool scanScenes = true;
        private bool scanPrefabs = true;
        private bool limitToSelection = false; // Only scan selected folders/assets

        private string[] _sceneGuids;
        private string[] _prefabGuids;

        private void OnGUI()
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Replace TextMeshPro Font Across Project", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select a TMP_FontAsset and run. This will edit all scenes and prefabs to use the specified font.", MessageType.Info);

            targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Target TMP Font", targetFont, typeof(TMP_FontAsset), false);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
            scanScenes = EditorGUILayout.ToggleLeft("Scenes (t:Scene)", scanScenes);
            scanPrefabs = EditorGUILayout.ToggleLeft("Prefabs (t:Prefab)", scanPrefabs);
            limitToSelection = EditorGUILayout.ToggleLeft("Only within current Project selection", limitToSelection);

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(targetFont == null || (!scanScenes && !scanPrefabs)))
            {
                if (GUILayout.Button("Run Font Replacement", GUILayout.Height(32)))
                {
                    Run();
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Dry Run: Count Affected Objects"))
            {
                var (_, totalTexts) = CollectTargets(dryRun: true);
                EditorUtility.DisplayDialog("TMP Font Replacer", $"Text components that would be processed: {totalTexts}", "OK");
            }
        }

        private void Run()
        {
            try
            {
                var (paths, _) = CollectTargets(dryRun: false);
                if (paths.Count == 0)
                {
                    EditorUtility.DisplayDialog("TMP Font Replacer", "No assets found to process with current filters.", "OK");
                    return;
                }

                int changedTotal = 0;
                int processedAssets = 0;

                AssetDatabase.StartAssetEditing();
                try
                {
                    for (int i = 0; i < paths.Count; i++)
                    {
                        string path = paths[i];
                        bool isScene = path.EndsWith(".unity");

                        if (EditorUtility.DisplayCancelableProgressBar(
                                "TMP Font Replacer",
                                $"Processing {(isScene ? "Scene" : "Prefab")}: {Path.GetFileName(path)}",
                                (float)i / paths.Count))
                        {
                            break;
                        }

                        if (isScene)
                        {
                            changedTotal += ProcessScene(path);
                        }
                        else
                        {
                            changedTotal += ProcessPrefab(path);
                        }

                        processedAssets++;
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    EditorUtility.ClearProgressBar();
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog(
                    "TMP Font Replacer",
                    $"Done.\nAssets processed: {processedAssets}\nText components changed: {changedTotal}",
                    "OK");
            }
            finally
            {
                // ensure scenes return to previous state if needed
            }
        }

        private (List<string> paths, int totalTexts) CollectTargets(bool dryRun)
        {
            var paths = new List<string>();
            int totalTexts = 0;

            var searchInFolders = (limitToSelection ? GetSelectedFoldersOrAssetDirs() : null);
            string[] folders = (searchInFolders != null && searchInFolders.Length > 0) ? searchInFolders : null; // null => whole project

            if (scanScenes)
            {
                _sceneGuids = AssetDatabase.FindAssets("t:Scene", folders);
                foreach (var g in _sceneGuids)
                {
                    string p = AssetDatabase.GUIDToAssetPath(g);
                    if (!paths.Contains(p)) paths.Add(p);
                }
            }
            if (scanPrefabs)
            {
                _prefabGuids = AssetDatabase.FindAssets("t:Prefab", folders);
                foreach (var g in _prefabGuids)
                {
                    string p = AssetDatabase.GUIDToAssetPath(g);
                    if (!paths.Contains(p)) paths.Add(p);
                }
            }

            if (dryRun)
            {
                // Rough count by loading and scanning quickly
                foreach (var p in paths)
                {
                    if (p.EndsWith(".unity"))
                        totalTexts += CountTextsInScene(p);
                    else
                        totalTexts += CountTextsInPrefab(p);
                }
            }

            return (paths, totalTexts);
        }

        private int ProcessScene(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int changed = 0;

            foreach (var root in scene.GetRootGameObjects())
            {
                changed += ApplyToHierarchy(root);
            }

            if (changed > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            return changed;
        }

        private int ProcessPrefab(string prefabPath)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            int changed = ApplyToHierarchy(root);

            if (changed > 0)
            {
                EditorUtility.SetDirty(root);
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }

            PrefabUtility.UnloadPrefabContents(root);
            return changed;
        }

        private int ApplyToHierarchy(GameObject root)
        {
            int changed = 0;

            // UI (UGUI)
            var ugui = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in ugui)
            {
                if (t == null) continue;
                if (t.font != targetFont)
                {
                    Undo.RegisterCompleteObjectUndo(t, "Replace TMP Font");
                    t.font = targetFont;
                    EditorUtility.SetDirty(t);
                    changed++;
                }
            }

            // 3D TextMeshPro
            var world = root.GetComponentsInChildren<TextMeshPro>(true);
            foreach (var t in world)
            {
                if (t == null) continue;
                if (t.font != targetFont)
                {
                    Undo.RegisterCompleteObjectUndo(t, "Replace TMP Font");
                    t.font = targetFont;
                    EditorUtility.SetDirty(t);
                    changed++;
                }
            }

            return changed;
        }

        private int CountTextsInScene(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int count = 0;
            foreach (var root in scene.GetRootGameObjects())
            {
                count += root.GetComponentsInChildren<TextMeshProUGUI>(true).Length;
                count += root.GetComponentsInChildren<TextMeshPro>(true).Length;
            }
            return count;
        }

        private int CountTextsInPrefab(string prefabPath)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            int count = 0;
            count += root.GetComponentsInChildren<TextMeshProUGUI>(true).Length;
            count += root.GetComponentsInChildren<TextMeshPro>(true).Length;
            PrefabUtility.UnloadPrefabContents(root);
            return count;
        }

        private static string[] GetSelectedFoldersOrAssetDirs()
        {
            var paths = new HashSet<string>();
            foreach (var obj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                if (Directory.Exists(path))
                {
                    paths.Add(path);
                }
                else
                {
                    var dir = Path.GetDirectoryName(path)?.Replace('\\', '/');
                    if (!string.IsNullOrEmpty(dir)) paths.Add(dir);
                }
            }
            return paths.ToArray();
        }
    }

}