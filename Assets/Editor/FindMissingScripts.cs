using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindInScene()
    {
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        int count = 0;

        foreach (var root in roots)
        {
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                var components = t.gameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.Log($"Missing script in GameObject: {GetFullPath(t)}", t.gameObject);
                        count++;
                    }
                }
            }
        }

        Debug.Log($"Found {count} missing scripts in scene.");
    }

    [MenuItem("Tools/Remove Missing Scripts in Scene")]
    public static void RemoveInScene()
    {
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        int removed = 0;

        foreach (var root in roots)
        {
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                GameObject go = t.gameObject;
                SerializedObject so = new SerializedObject(go);
                SerializedProperty prop = so.FindProperty("m_Component");
                if (prop == null) continue;

                for (int i = prop.arraySize - 1; i >= 0; i--)
                {
                    var element = prop.GetArrayElementAtIndex(i);
                    var compProp = element.FindPropertyRelative("component");
                    if (compProp != null && compProp.objectReferenceValue == null)
                    {
                        prop.DeleteArrayElementAtIndex(i);
                        removed++;
                    }
                }

                so.ApplyModifiedProperties();
            }
        }

        Debug.Log($"Removed {removed} missing script components in scene.");
    }

    static string GetFullPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
