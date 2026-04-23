using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    /// <summary>
    /// Custom editor for the <c>ActorsInfoConfig</c> ScriptableObject. 
    /// It displays GUIDs and allows editing of individual actor data structures.
    /// </summary>
    [CustomEditor(typeof(ActorsInfoConfig))]
    public class ActorsInfoConfigEditor : BaseGuidInfoConfigEditor<ActorsInfoConfig, ActorGuidInfo, ActorInfo>
    {
        /// <summary>
        /// Draws the UI for each individual actor entry in the config.
        /// </summary>
        protected override void DrawInfos()
        {
            var entries = Config.Entries;
            var entriesProp = serializedObject.FindProperty("_entries");

            // Iterate backwards to allow safe removal of elements
            for (int i = entriesProp.arraySize - 1; i >= 0; i--)
            {
                var entryProp = entriesProp.GetArrayElementAtIndex(i);
                var dataProp = entryProp.FindPropertyRelative("Data");

                // Assuming SerializableGuid is used and accessible via Unity.VisualScripting or a custom utility
                var guidProperty = entryProp.FindPropertyRelative("Guid");

                var guid = entries[i].Guid; // Direct access is safer if GetUnderlyingValue is complex

                EditorGUILayout.BeginVertical("box");

                // Display GUID for reference
                EditorGUILayout.LabelField($"Guid: {guid}", EditorStyles.miniLabel);

                // Draw the nested data structure (ActorInfo)
                EditorGUILayout.PropertyField(dataProp, includeChildren: true);

                EditorGUILayout.Space(6);

                if (GUILayout.Button("Remove Entry"))
                {
                    // Record undo operation before removing
                    Undo.RecordObject(Config, "Remove Actor Entry");
                    // Remove the element and break the loop to prevent index issues
                    entries.RemoveAt(i);
                    EditorUtility.SetDirty(Config);
                    break;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(4);
            }
        }
    }
}