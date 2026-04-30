using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Represents a single clickable option in a choice node.
    /// </summary>
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        /// <summary>
        /// Initializes the option, sets its text, and waits asynchronously until the user clicks the button.
        /// </summary>
        /// <param name="optionModel">The model containing the option data.</param>
        /// <returns>The <see cref="Guid"/> of the next node associated with this option.</returns>
        public async UniTask<Guid> WaitForChoice(ChoiceOptionModel optionModel)
        {
            // Use UniTaskCompletionSource to convert the event-based click into an awaitable Task
            var completionSource = new UniTaskCompletionSource();

            _text.text = optionModel.DialogueText.Text;

            // Add a temporary listener that completes the task when clicked
            Action onClickAction = () => completionSource.TrySetResult();
            _button.onClick.AddListener(onClickAction.Invoke);

            // Wait until the button is clicked
            await completionSource.Task;

            // Clean up the listener after the task is completed
            _button.onClick.RemoveListener(onClickAction.Invoke);

            return optionModel.RelatedNodeId;
        }
    }
}