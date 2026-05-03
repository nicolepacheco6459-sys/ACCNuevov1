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
            gameObject.SetActive(true);

            if (_replicaView != null)
                _replicaView.gameObject.SetActive(true);

            if (_choiceView != null)
                _choiceView.gameObject.SetActive(false);

            await PlayDialogueList(_dialogueScenes);

            // CERRAR DIÁLOGO AL FINAL
            CloseDialogue();
        }
        void CloseDialogue()
        {
            Debug.Log("🔴 Cerrando diálogo");

            if (_replicaView != null)
                _replicaView.gameObject.SetActive(false);

            if (_choiceView != null)
                _choiceView.gameObject.SetActive(false);

            if (_skipButton != null)
                _skipButton.gameObject.SetActive(false);

            gameObject.SetActive(false);
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

        // =========================
        // FIX: WAIT FOR SKIP (SIN BLOQUEAR CLICK)
        // =========================
        public async UniTask WaitForSkip()
        {
            if (_skipButton == null)
                return;

            _skipButton.gameObject.SetActive(true);

            var taskSource = new UniTaskCompletionSource();

            void OnSkip()
            {
                taskSource.TrySetResult();
            }

            _skipButton.onClick.AddListener(OnSkip);

            // También permitir teclado sin romper UI
            while (!taskSource.Task.Status.IsCompleted())
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
                {
                    taskSource.TrySetResult();
                    break;
                }

                await UniTask.Yield();
            }

            _skipButton.onClick.RemoveListener(OnSkip);
            _skipButton.gameObject.SetActive(false);
        }
    }
}