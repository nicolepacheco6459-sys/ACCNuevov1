using KissMyAssets.VisualNovelCore.Runtime;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public abstract class BaseGuidInfoConfigEditor<TConfig, TEntry, TData> : UnityEditor.Editor where TConfig : BaseGuidInfoConfig<TEntry, TData> where TEntry : BaseGuidInfo<TData>, new() where TData : class
    {
        protected TConfig Config => (TConfig)target;

        public override void OnInspectorGUI()
        {
            DrawInfos();

            if (GUILayout.Button("Add New"))
            {
                Undo.RecordObject(Config, "Add");
                Config.AddNewEntry();
                EditorUtility.SetDirty(Config);
            }

            if (GUILayout.Button("Save"))
            {
                Undo.RecordObject(Config, "Save");
                EditorUtility.SetDirty(Config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        protected virtual void DrawInfos()
        {
            var entriesProp = serializedObject.FindProperty("_entries");
            for (int i = 0; i < entriesProp.arraySize; i++)
            {
                var entryProp = entriesProp.GetArrayElementAtIndex(i);
                var dataProp = entryProp.FindPropertyRelative("Data");
                EditorGUILayout.PropertyField(dataProp, includeChildren: true);

                EditorGUILayout.Space(6);
            }
        }
    }
}
