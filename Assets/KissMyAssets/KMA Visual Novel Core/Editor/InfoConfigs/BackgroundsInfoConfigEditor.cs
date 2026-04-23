using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Custom editor for the <c>BackgroundsInfoConfig</c> ScriptableObject. 
    /// It provides a custom UI to edit background data entries (Name, Sprite) instead of using generic PropertyField.
    /// </summary>
    [CustomEditor(typeof(BackgroundsInfoConfig))]
    public class BackgroundsInfoConfigEditor : BaseGuidInfoConfigEditor<BackgroundsInfoConfig, BackgroundGuidInfo, BackgroundInfo>
    {
        /// <summary>
        /// Draws the UI for each individual background entry in the config.
        /// </summary>
        protected override void DrawInfos()
        {
            var entries = Config.Entries;

            // Iterate backwards to allow safe removal of elements
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var entry = entries[i];

                EditorGUILayout.BeginVertical("box");

                // Display GUID for reference
                EditorGUILayout.LabelField($"Guid: {entry.Guid}", EditorStyles.miniLabel);

                // Use a custom method to draw the specific BackgroundInfo fields
                entry.Data = DrawInfo(entry.Data);

                EditorGUILayout.Space(6);

                if (GUILayout.Button("Remove Entry"))
                {
                    // Record undo operation before removing
                    Undo.RecordObject(Config, "Remove Background Entry");
                    // Remove the element and break the loop to prevent index issues
                    entries.RemoveAt(i);
                    EditorUtility.SetDirty(Config);
                    break;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(4);
            }
        }

        /// <summary>
        /// Draws the custom UI fields for a single <c>BackgroundInfo</c> data object.
        /// </summary>
        /// <param name="info">The data object to draw/edit.</param>
        /// <returns>The updated data object.</returns>
        private BackgroundInfo DrawInfo(BackgroundInfo info)
        {
            // Ensure data object is initialized
            if (info == null)
                info = new BackgroundInfo();

            // Draw custom controls for fields
            info.Name = EditorGUILayout.TextField("Name", info.Name);
            info.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", info.Sprite, typeof(Sprite), allowSceneObjects: false);

            return info;
        }
    }
}