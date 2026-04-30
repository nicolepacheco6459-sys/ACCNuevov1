using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Component responsible for displaying the speaker's name in the dialogue UI.
    /// </summary>
    public class NamePlateView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;

        /// <summary>
        /// Sets the speaker's name and forces a UI layout rebuild to ensure proper sizing.
        /// </summary>
        /// <param name="name">The name to display.</param>
        public async UniTask SetName(string name)
        {
            _nameText.text = name;
            // Force the layout to rebuild immediately to correctly size the name plate
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.GetComponent<RectTransform>());
            await UniTask.Yield(); // Yield to ensure the frame finishes (optional, but good practice with UI updates)
        }
    }
}