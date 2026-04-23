using KissMyAssets.VisualNovelCore.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Editor window for creating and managing <see cref="DialogueSceneConfig"/> assets.
    /// It provides file management (path selection, creation, listing) and basic diagnostics.
    /// </summary>
    public class DialogueCreatorWindow : EditorWindow
    {
        private const string PrefKeyFolderPath = "DialogueCreator.TargetFolderPath";
        private const string DefaultFolderPath = "Assets/Resources/DialogueScenes";

        [SerializeField] private DefaultAsset _folderAsset; // Folder in Project
        [SerializeField] private string _folderPath;        // Relative path (Assets/...)

        private Vector2 _scroll;
        private string _search = string.Empty;
        private string _newAssetName = string.Empty;
        private SearchField _searchField;

        // Unified list entry for scene configurations
        private sealed class Entry
        {
            public DialogueSceneConfig Config;
            public string Path;
        }

        private readonly List<string> _uniqueFolderPaths = new();

        private readonly List<Entry> _entries = new();

        // private DialogueDiagnosticsController _diag; 

        /// <summary>
        /// Called when the window is enabled. Loads preferences and initializes components.
        /// </summary>
        private void OnEnable()
        {
            titleContent = new GUIContent("Dialogue Creator");
            _searchField = new SearchField();

            LoadFolderFromPreferences();
            RefreshList();
        }

        /// <summary>
        /// Unity's built-in method for drawing and handling GUI events within the editor window.
        /// </summary>
        private void OnGUI()
        {
            DrawFolderSelectionPanel();
            EditorGUILayout.Space(8);

            DrawFolderListPanel();
            EditorGUILayout.Space(8);

            DrawAssetCreationPanel();
            EditorGUILayout.Space(8);

            DrawAssetListPanel();
        }

        /// <summary>
        /// Loads the last selected folder path from Editor Preferences.
        /// </summary>
        private void LoadFolderFromPreferences()
        {
            _folderPath = EditorPrefs.GetString(PrefKeyFolderPath, DefaultFolderPath);
            _folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(_folderPath);
            if (_folderAsset == null)
            {
                // Fallback to default if the saved path is invalid
                _folderPath = DefaultFolderPath;
                _folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(_folderPath);
            }
        }

        /// <summary>
        /// Draws the control panel for selecting the target folder path.
        /// </summary>
        private void DrawFolderSelectionPanel()
        {
            GUILayout.Label("Target Folder", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            _folderAsset = (DefaultAsset)EditorGUILayout.ObjectField(
                "Folder",
                _folderAsset,
                typeof(DefaultAsset),
                false,
                GUILayout.Height(EditorGUIUtility.singleLineHeight)
            );

            if (EditorGUI.EndChangeCheck() && _folderAsset != null)
            {
                UpdateFolderPath(_folderAsset);
            }

            if (string.IsNullOrEmpty(_folderPath) || _folderAsset == null)
                EditorGUILayout.HelpBox("Select a folder to manage dialogue scene assets.", MessageType.Error);
        }

        /// <summary>
        /// Updates the internal folder path and saves it to preferences.
        /// </summary>
        private void UpdateFolderPath(DefaultAsset folder)
        {
            string newPath = AssetDatabase.GetAssetPath(folder);
            if (AssetDatabase.IsValidFolder(newPath))
            {
                _folderAsset = folder;
                _folderPath = newPath;
                EditorPrefs.SetString(PrefKeyFolderPath, _folderPath);
                RefreshList();

                Repaint();
            }
        }

        /// <summary>
        /// Draws the list of folders containing DialogueSceneConfig assets.
        /// </summary>
        private void DrawFolderListPanel()
        {
            GUILayout.Label("Existing Scene Folders", EditorStyles.boldLabel);

            if (_uniqueFolderPaths.Count == 0)
            {
                EditorGUILayout.HelpBox("No existing DialogueSceneConfig assets found in the project.", MessageType.Info);
                return;
            }

            // Create a copy of the list to prevent InvalidOperationException during enumeration
            var pathsToDraw = _uniqueFolderPaths.ToList();

            foreach (string path in pathsToDraw)
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    string folderName = Path.GetFileName(path);

                    GUILayout.Label($"{folderName} ({path})", EditorStyles.label, GUILayout.ExpandWidth(true));

                    // Button to select the folder
                    if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
                    {
                        DefaultAsset folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                        if (folderAsset != null)
                        {
                            UpdateFolderPath(folderAsset);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the panel for creating a new <see cref="DialogueSceneConfig"/> asset.
        /// </summary>
        private void DrawAssetCreationPanel()
        {
            GUILayout.Label("Create New Scene", EditorStyles.boldLabel);

            _newAssetName = EditorGUILayout.TextField(
                new GUIContent("Asset Name", "Name for the new DialogueSceneConfig asset"),
                _newAssetName
            );

            if (string.IsNullOrEmpty(_newAssetName))
            {
                // Suggest a name if the field is empty
                GUILayout.Label($"Suggested name: {SuggestAssetName()}", EditorStyles.miniLabel);
            }

            using (new EditorGUI.DisabledScope(_folderAsset == null))
            {
                if (GUILayout.Button("Create Dialogue Scene Asset", GUILayout.Height(30)))
                {
                    CreateNewAsset();
                }
            }
        }

        /// <summary>
        /// Creates a new DialogueSceneConfig asset based on the current input.
        /// </summary>
        private void CreateNewAsset()
        {
            // Use suggested name if field is empty
            string baseName = string.IsNullOrEmpty(_newAssetName) ? SuggestAssetName() : _newAssetName;
            string sanitizedName = SanitizeFileName(baseName);

            string targetPath = GenerateUniqueAssetPath(_folderPath, sanitizedName);

            var instance = ScriptableObject.CreateInstance<DialogueSceneConfig>();
            AssetDatabase.CreateAsset(instance, targetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SelectAndPing(instance);
            _newAssetName = string.Empty;
            RefreshList();
        }

        /// <summary>
        /// Draws the list of existing <see cref="DialogueSceneConfig"/> assets in the target folder.
        /// </summary>
        private void DrawAssetListPanel()
        {
            GUILayout.Label($"Scene Assets ({_entries.Count})", EditorStyles.boldLabel);

            // Search bar
            _search = _searchField.OnGUI(EditorGUILayout.GetControlRect(), _search);

            // List view
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var filteredEntries = FilterEntries(_search);

            foreach (var entry in filteredEntries)
            {
                DrawEntry(entry);
            }

            EditorGUILayout.EndScrollView();

            // Refresh button
            if (GUILayout.Button("Refresh List"))
                RefreshList();
        }

        /// <summary>
        /// Filters the list of dialogue scene assets based on the search query.
        /// </summary>
        private IEnumerable<Entry> FilterEntries(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _entries;

            return _entries.Where(e => e.Config.DialogueSceneName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Draws a single entry in the asset list with ping/edit buttons.
        /// </summary>
        private void DrawEntry(Entry entry)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                // Main label
                GUILayout.Label(entry.Config.DialogueSceneName, GUILayout.ExpandWidth(true));

                // Ping button
                if (GUILayout.Button("Ping", GUILayout.Width(45)))
                    SelectAndPing(entry.Config);
            }
        }

        /// <summary>
        /// Populates the internal list with all <see cref="DialogueSceneConfig"/> assets found in the current folder path.
        /// </summary>
        private void RefreshList()
        {
            _entries.Clear();
            _uniqueFolderPaths.Clear();

            // Find ALL DialogueSceneConfig assets in the entire project
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(DialogueSceneConfig)}");

            var uniquePathsSet = new HashSet<string>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<DialogueSceneConfig>(path);

                if (config != null)
                {
                    // Filter assets for the current target folder
                    if (path.StartsWith(_folderPath + "/") || path == _folderPath)
                    {
                        _entries.Add(new Entry { Config = config, Path = path });
                    }

                    // Collect unique directory paths
                    string directoryPath = Path.GetDirectoryName(path).Replace('\\', '/');
                    uniquePathsSet.Add(directoryPath);
                }
            }

            _uniqueFolderPaths.AddRange(uniquePathsSet.OrderBy(p => p));
        }

        /// <summary>
        /// Generates a unique asset path for the new file, preventing overwrites.
        /// </summary>
        private static string GenerateUniqueAssetPath(string folderPath, string baseName)
            => AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, baseName + ".asset"));

        /// <summary>
        /// Suggests a sequential asset name if the user has not provided one.
        /// </summary>
        private string SuggestAssetName()
        {
            int counter = 1;
            while (true)
            {
                string name = $"DialogueScene_{counter:D3}";
                string path = Path.Combine(_folderPath, name + ".asset");
                if (!AssetExists(path)) return name;
                counter++;
            }
        }

        /// <summary>
        /// Checks if an asset already exists at the specified path.
        /// </summary>
        private static bool AssetExists(string path)
            => AssetDatabase.LoadAssetAtPath<DialogueSceneConfig>(path) != null;

        /// <summary>
        /// Removes invalid characters from a file name.
        /// </summary>
        private static string SanitizeFileName(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var safe = new string(input.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
            return string.IsNullOrWhiteSpace(safe) ? "DialogueScene" : safe;
        }

        /// <summary>
        /// Selects the object in the Project window and highlights it (pings).
        /// </summary>
        private static void SelectAndPing(UnityEngine.Object obj)
        {
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }
}