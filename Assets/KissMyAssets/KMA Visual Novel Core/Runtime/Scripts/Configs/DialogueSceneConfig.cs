using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Configuration asset for a single dialogue scene. 
    /// It links to the scene's JSON file containing the actual graph structure and dialogue content.
    /// Provides methods for editor-time asset management (create/load/save JSON).
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueScene", menuName = VisualNovelConstants.AssetPath + "Scene Config", order = 0)]
    public partial class DialogueSceneConfig : ScriptableObject
    {
        // Cached runtime data when editing the graph
        public DialogueData DialogData { get; private set; }

        /// <summary>
        /// The display name of the dialogue scene.
        /// </summary>
        [field: SerializeField]
        public string DialogueSceneName { get; private set; } = "NewDialogueScene";

        /// <summary>
        /// Reference to the TextAsset containing the dialogue graph data in JSON format.
        /// </summary>
        [field: SerializeField]
        public TextAsset JsonFile { get; private set; }

        /// <summary>
        /// Loads and deserializes the dialogue graph data from the linked <see cref="JsonFile"/>.
        /// </summary>
        /// <returns>A deserialized <see cref="DialogueData"/> object, or a new empty one if the file is missing or invalid.</returns>
        public DialogueData LoadData()
        {
            if (JsonFile == null)
            {
                Debug.LogWarning($"[{nameof(DialogueSceneConfig)}] No JSON file linked to config: {name}. Returning new empty data.");
                return new DialogueData();
            }

            try
            {
                DialogData = JsonUtility.FromJson<DialogueData>(JsonFile.text);

                if (DialogData == null)
                {
                    Debug.LogError($"[{nameof(DialogueSceneConfig)}] Failed to deserialize JSON data from: {JsonFile.name}. Returning new empty data.");
                    return new DialogueData();
                }

                return DialogData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[{nameof(DialogueSceneConfig)}] Exception during JSON deserialization from: {JsonFile.name}. Error: {e.Message}");
                return new DialogueData();
            }
        }

        public void SetNewFile(TextAsset textAsset)
        {
            JsonFile = textAsset;
        }
    }
}