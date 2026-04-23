using KissMyAssets.VisualNovelCore.Runtime;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Custom editor for the <see cref="DialogueSceneConfig"/> ScriptableObject asset.
    /// It provides buttons to manage the associated dialogue JSON data and open the graph editor window.
    /// </summary>
    [CustomEditor(typeof(DialogueSceneConfig))]
    internal class DialogueSceneConfigEditor : UnityEditor.Editor
    {
        // Controller for opening the main graph window
        private DialogueSceneGraphWindowController _graphWindowController = new DialogueSceneGraphWindowController();

        private DialogueSceneConfig _config => (DialogueSceneConfig)target;

        /// <summary>
        /// Draws the custom inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Draw default fields for the DialogueSceneConfig
            DrawDefaultInspector();

            EditorGUILayout.Space();

            // Draw the custom buttons for graph interaction
            DrawGraphInteractionButtons();
        }

        /// <summary>
        /// Draws the horizontal button layout for creating, editing, and saving the dialogue graph data.
        /// </summary>
        private void DrawGraphInteractionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawCreateJsonButton();
                DrawEditGraphButton();
                DrawSaveButton(_config.DialogData);
            }
        }

        /// <summary>
        /// Draws the button to create a new, empty dialogue JSON file associated with the config.
        /// </summary>
        private void DrawCreateJsonButton()
        {
            if (GUILayout.Button("Create New JSON", GUILayout.Height(22)))
            {
                CreateNewJsonFile();
            }
        }

        /// <summary>
        /// Draws the button to load the dialogue data, open the graph editor window, and start editing.
        /// </summary>
        private void DrawEditGraphButton()
        {
            if (GUILayout.Button("Edit Graph", GUILayout.Height(22)))
            {
                // Load the JSON data into a runtime object
                var data = _config.LoadData();
                // Open the graph editor window
                _graphWindowController.ShowWindow(data, _config.DialogueSceneName);
            }
        }

        /// <summary>
        /// Draws the button to save the currently cached dialogue data back to the JSON file.
        /// </summary>
        private void DrawSaveButton(DialogueData data)
        {
            // Note: This button assumes _dialogData is populated from a previous "Edit Graph" call
            using (new EditorGUI.DisabledScope(data == null))
            {
                if (GUILayout.Button("Save Data", GUILayout.Height(22)))
                {
                    // Mark the ScriptableObject as dirty for potential changes
                    EditorUtility.SetDirty(_config);
                    // Save the cached runtime data to the JSON file
                    SaveData(data);
                }
            }
        }

        /// <summary>
        /// Creates a new empty JSON file (containing an empty <see cref="DialogueData"/> structure) 
        /// next to this ScriptableObject and assigns it to the <see cref="JsonFile"/> property.
        /// If a file with the derived name already exists, a unique name is generated.
        /// </summary>
        public void CreateNewJsonFile()
        {
            // Assuming EditorPathUtil is an existing utility class available in the editor context
            var (dir, baseName) = EditorPathUtil.GetSelfDirAndBaseName(_config);
            var nameForFile = string.IsNullOrWhiteSpace(_config.DialogueSceneName) ? name : _config.DialogueSceneName;


            string uniqueJsonPath = EditorPathUtil.GenerateUniqueJsonPath(dir, nameForFile);

            // Create a default empty DialogData structure and serialize it to JSON
            var emptyData = new DialogueData();
            var emptyJson = JsonUtility.ToJson(emptyData, true);

            EditorPathUtil.WriteTextFile(uniqueJsonPath, emptyJson);

            AssetDatabase.ImportAsset(uniqueJsonPath);

            // Load the newly created asset
            _config.SetNewFile(AssetDatabase.LoadAssetAtPath<TextAsset>(uniqueJsonPath));

            // Mark the ScriptableObject as dirty and save changes
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Serializes and saves the provided <see cref="DialogueData"/> object back to the linked <see cref="JsonFile"/>.
        /// </summary>
        /// <param name="data">The <see cref="DialogueData"/> object containing the graph structure to save.</param>
        public void SaveData(DialogueData data)
        {
            if (_config.JsonFile == null)
            {
                Debug.LogError($"[{nameof(DialogueSceneConfig)}] Cannot save data: No JSON file is linked to config: {name}.");
                return;
            }

            var json = JsonUtility.ToJson(data, true);

            // Get the physical path to the TextAsset and write the new JSON content
            string path = AssetDatabase.GetAssetPath(_config.JsonFile);
            File.WriteAllText(path, json);

            // Mark the TextAsset as dirty and save
            EditorUtility.SetDirty(_config.JsonFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(DialogueSceneConfig)}] Saved data to JSON file: {_config.JsonFile.name}");
        }
    }
}