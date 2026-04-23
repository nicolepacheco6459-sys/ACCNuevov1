using System.IO;
using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Utility class containing editor-only methods for file path manipulation, 
    /// primarily used for locating assets and generating unique file names within the Unity Editor.
    /// </summary>
    public static class EditorPathUtil
    {
        /// <summary>
        /// Retrieves the directory path and base file name of a ScriptableObject asset.
        /// </summary>
        /// <param name="obj">The target ScriptableObject.</param>
        /// <returns>A tuple containing the directory path (e.g., "Assets/Data") and the base file name.</returns>
        public static (string dir, string baseName) GetSelfDirAndBaseName(ScriptableObject obj)
        {
            string soPath = AssetDatabase.GetAssetPath(obj);
            // Get directory, replacing backslashes with forward slashes for Unity's consistency
            string dir = string.IsNullOrEmpty(soPath) ? "Assets" : Path.GetDirectoryName(soPath)?.Replace('\\', '/') ?? "Assets";
            string baseName = Path.GetFileNameWithoutExtension(soPath);
            // Use object name if baseName is empty (e.g., if the asset is not yet saved)
            return (dir, string.IsNullOrEmpty(baseName) ? obj.name : baseName);
        }


        /// <summary>
        /// Generates a unique, sanitized asset path for a new JSON file within a given directory.
        /// </summary>
        /// <param name="directory">The target folder path (e.g., "Assets/Data").</param>
        /// <param name="baseName">The desired base name for the file.</param>
        /// <returns>A unique, full path to the new .json file.</returns>
        public static string GenerateUniqueJsonPath(string directory, string baseName)
        {
            // Combine path, sanitize the name, and use forward slashes
            string candidate = Path.Combine(directory, Sanitize(baseName) + ".json").Replace('\\', '/');
            // Use Unity's function to ensure the path is unique (appends numbers if necessary)
            candidate = AssetDatabase.GenerateUniqueAssetPath(candidate);
            return candidate;
        }


        /// <summary>
        /// Writes text content to a file, ensuring the target directory exists before writing.
        /// </summary>
        /// <param name="path">The full path to the file.</param>
        /// <param name="contents">The string content to write to the file.</param>
        public static void WriteTextFile(string path, string contents)
        {
            // Ensure the folder exists
            string folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(path, contents);
        }


        /// <summary>
        /// Sanitizes a string by replacing invalid file name characters with underscores.
        /// </summary>
        /// <param name="name">The input name string.</param>
        /// <returns>The sanitized file name, defaulting to "DialogueScene" if the result is empty or whitespace.</returns>
        public static string Sanitize(string name)
        {
            // Replace all invalid path characters
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            // Return a default name if the sanitized result is empty
            return string.IsNullOrWhiteSpace(name) ? "DialogueScene" : name;
        }
    }
}