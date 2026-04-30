using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
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

        // =========================
        // MAIN ENTRY
        // =========================
        public async UniTask PlayDialogues()
        {
            // 🔥 ASEGURAR QUE TODO EL UI ESTÉ ACTIVO
            gameObject.SetActive(true);

            if (_replicaView != null)
                _replicaView.gameObject.SetActive(true);

            if (_choiceView != null)
                _choiceView.gameObject.SetActive(false);

            await PlayDialogueList(_dialogueScenes);
        }

        // =========================
        // PLAY ALL SCENES
        // =========================
        private async UniTask PlayDialogueList(IReadOnlyList<DialogueSceneConfig> dialogueScenes)
        {
            foreach (var dialogueScene in dialogueScenes)
            {
                await PlayDialogueScene(dialogueScene);
                Debug.Log($"Dialogue Scene {dialogueScene.name} finished.");
            }
        }

        // =========================
        // PLAY ONE SCENE
        // =========================
        private async UniTask PlayDialogueScene(DialogueSceneConfig dialogueScene)
        {
            DialogueData data = dialogueScene.LoadData();
            var dialogueModel = new DialogueSceneModel(data);

            BaseDialogueNodeModel currentNode = dialogueModel.GetNextNode();

            while (!IsDialogueFinish(currentNode))
            {
                Debug.Log($"Node Type: {currentNode.GetType().Name}, Node ID: {currentNode.NodeId}");

                await currentNode.AcceptView(this);

                currentNode = dialogueModel.GetNextNode();
            }
        }

        private bool IsDialogueFinish(BaseDialogueNodeModel model)
        {
            return model is EndNodeModel;
        }

        // =========================
        // VIEW IMPLEMENTATION
        // =========================

        public async UniTask<Guid> ShowChoice(IChoiceShowInfoProvider showInfo, bool showReplicaView = false)
        {
            if (_choiceView != null)
                _choiceView.gameObject.SetActive(true);

            if (_replicaView != null)
                _replicaView.gameObject.SetActive(showReplicaView);

            if (_skipButton != null)
                _skipButton.gameObject.SetActive(false);

            var result = await _choiceView.ShowChoice(showInfo);

            // 🔥 NO apagar opciones automáticamente
            return result;
        }

        public async UniTask ShowReplica(IReplicaShowInfoProvider showInfo)
        {
            if (_choiceView != null)
                _choiceView.gameObject.SetActive(false);

            if (_replicaView != null)
                _replicaView.gameObject.SetActive(true);

            await _replicaView.ShowReplica(showInfo);
            await WaitForSkip();

            // 🔥 NO apagar el panel
        }

        public async UniTask TryToChageActors(ActorsHolderModel actorsModel)
        {
            if (_actorsView != null)
                await _actorsView.ShowActors(actorsModel);
        }

        public async UniTask TryToChangeBackground(BackgroundHolderModel backgroundModel)
        {
            if (_background != null)
                _background.sprite = backgroundModel.BackgroundSprite;

            await UniTask.Yield();
        }

        public async UniTask WaitForSkip()
        {
            var taskSource = new UniTaskCompletionSource();

            if (_skipButton != null)
                _skipButton.gameObject.SetActive(true);

            Action onSkipAction = () => taskSource.TrySetResult();

            if (_skipButton != null)
                _skipButton.onClick.AddListener(onSkipAction.Invoke);

            await taskSource.Task;

            if (_skipButton != null)
            {
                _skipButton.onClick.RemoveListener(onSkipAction.Invoke);
                _skipButton.gameObject.SetActive(false);
            }
        }
    }
}