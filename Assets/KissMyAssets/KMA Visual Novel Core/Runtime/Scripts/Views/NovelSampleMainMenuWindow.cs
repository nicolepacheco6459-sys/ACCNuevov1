using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// The main menu window for the sample, allowing the user to start the dialogue sequence.
    /// </summary>
    public class NovelSampleMainMenuWindow : SampleUIWindow<NovelSampleMainMenuWindow>
    {
        [SerializeField] private Button _playButton;

        /// <summary>
        /// Sets up the main menu window by attaching a listener to the play button.
        /// </summary>
        public void InitMainMenu()
        {
            _playButton.onClick.AddListener(() => PlayButtonClick().Forget());
        }

        /// <summary>
        /// Handles the logic when the play button is clicked: hides the menu, plays the dialogue, and then shows the menu again.
        /// </summary>
        private async UniTask PlayButtonClick()
        {
            HideWindow();

            // Get the dialogue window instance and run the dialogue flow
            NovelSampleDialogueWindow dialogueWindow = NovelSampleDialogueWindow.Instance;
            dialogueWindow.ShowWindow();

            await dialogueWindow.PlayDialogues();

            dialogueWindow.HideWindow();

            ShowWindow();
        }
    }
}