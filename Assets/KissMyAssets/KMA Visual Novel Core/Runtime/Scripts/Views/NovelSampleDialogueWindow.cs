using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// A sample UI window that implements the <see cref="IDialogueView"/> interface 
    /// to run and display a list of dialogue scenes.
    /// </summary>
    public class NovelSampleDialogueWindow : SampleUIWindow<NovelSampleDialogueWindow>, IDialogueView
    {
        [Header("Dialogue Scenes List")]
        [SerializeField] private List<DialogueSceneConfig> _dialogueScenes;

        [Header("Node View Elements")]
        [SerializeField] private Image _background;
        [SerializeField] private Button _skipButton;
        [SerializeField] private ActorsView _actorsView;
        [SerializeField] private ReplicaView _replicaView;
        [SerializeField] private ChoiceView _choiceView;

        /// <summary>
        /// Initiates the playback of all configured dialogue scenes.
        /// </summary>
        public async UniTask PlayDialogues()
        {
            SetNodePanelsActivity(false);

            await PlayDialogueList(_dialogueScenes);

            SetNodePanelsActivity(true);
        }

        /// <summary>
        /// Sets the active state of the node-specific UI panels.
        /// </summary>
        private void SetNodePanelsActivity(bool active)
        {
            _replicaView.gameObject.SetActive(active);
            _choiceView.gameObject.SetActive(active);
        }

        /// <summary>
        /// Iterates and plays a sequence of dialogue scenes.
        /// </summary>
        private async UniTask PlayDialogueList(IReadOnlyList<DialogueSceneConfig> dialogueScenes)
        {
            foreach (var dialogueScene in dialogueScenes)
            {
                await PlayDialogueScene(dialogueScene);
                Debug.Log($"Dialogue Scene {dialogueScene.name} finished.");
            }
        }

        /// <summary>
        /// Initializes a dialogue model and runs the dialogue graph for a single scene config.
        /// </summary>
        private async UniTask PlayDialogueScene(DialogueSceneConfig dialogueScene)
        {
            // Load the JSON data and create the runtime model
            DialogueData data = dialogueScene.LoadData();
            var dialogueModel = new DialogueSceneModel(data);

            // Get the starting node
            BaseDialogueNodeModel currentNode = dialogueModel.GetNextNode();

            // Loop through the graph until an EndNode is reached
            while (!IsDialogueFinish(currentNode))
            {
                Debug.Log($"Node Type: {currentNode.GetType().Name}, Node ID: {currentNode.NodeId}");

                // The core logic: the node model accepts the view interface, causing the view method to be called
                await currentNode.AcceptView(this);

                // Advance to the next node based on the previous interaction/link
                currentNode = dialogueModel.GetNextNode();
            }
        }

        /// <summary>
        /// Checks if the current node signifies the end of the dialogue.
        /// </summary>
        /// <param name="model">The current dialogue node model.</param>
        /// <returns>True if the node is an <see cref="EndNodeModel"/>, otherwise False.</returns>
        private bool IsDialogueFinish(BaseDialogueNodeModel model)
        {
            return model is EndNodeModel;
        }

        #region IDialogueView Implementation

        /// <inheritdoc/>
        public async UniTask<Guid> ShowChoice(IChoiceShowInfoProvider showInfo, bool showReplicaView = false)
        {
            _choiceView.gameObject.SetActive(true);
            _replicaView.gameObject.SetActive(showReplicaView);

            _skipButton.gameObject.SetActive(false);

            var result = await _choiceView.ShowChoice(showInfo);

            _choiceView.gameObject.SetActive(false);

            return result;
        }

        /// <inheritdoc/>
        public async UniTask ShowReplica(IReplicaShowInfoProvider showInfo)
        {
            _choiceView.gameObject.SetActive(false);
            _replicaView.gameObject.SetActive(true);

            await _replicaView.ShowReplica(showInfo);
            await WaitForSkip();

            _replicaView.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public async UniTask TryToChageActors(ActorsHolderModel actorsModel)
        {
            await _actorsView.ShowActors(actorsModel);

            Debug.Log($"Actors Count: {actorsModel.ActorAlignmentMap.Count}");
        }

        /// <inheritdoc/>
        public async UniTask TryToChangeBackground(BackgroundHolderModel backgroundModel)
        {
            _background.sprite = backgroundModel.BackgroundSprite;
            await UniTask.Yield();
        }

        public async UniTask WaitForSkip()
        {
            var taskSource = new UniTaskCompletionSource();

            _skipButton.gameObject.SetActive(true);

            // Add a temporary listener to complete the task when the user clicks 'skip'
            Action onSkipAction = () => taskSource.TrySetResult();
            _skipButton.onClick.AddListener(onSkipAction.Invoke);

            await taskSource.Task;

            // Clean up
            _skipButton.onClick.RemoveListener(onSkipAction.Invoke);
            _skipButton.gameObject.SetActive(false);
        }

        #endregion
    }
}