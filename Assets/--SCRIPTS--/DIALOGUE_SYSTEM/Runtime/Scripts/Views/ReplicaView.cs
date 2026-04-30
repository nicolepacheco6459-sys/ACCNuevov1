using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Manages the display of a dialogue replica (text and speaker name) and waits for user confirmation.
    /// </summary>
    public class ReplicaView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TypingTextView _typingReplica;
        [SerializeField] private Button _skipButton;

        [Header("Name Plates Layouts")]
        [SerializeField] private NamePlateView _leftNamePlateLayout;
        [SerializeField] private NamePlateView _centralNamePlateLayout;
        [SerializeField] private NamePlateView _rightNamePlateLayout;

        private readonly Dictionary<EActorAlignmentType, NamePlateView> _namePlatesMap = new();

        private void Awake()
        {
            // Initialize the map for quick lookup of name plates by alignment
            _namePlatesMap.Add(EActorAlignmentType.Left, _leftNamePlateLayout);
            _namePlatesMap.Add(EActorAlignmentType.Center, _centralNamePlateLayout);
            _namePlatesMap.Add(EActorAlignmentType.Right, _rightNamePlateLayout);

            _skipButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays the replica text and speaker name, then waits for the skip button click.
        /// </summary>
        /// <param name="showInfo">The model containing the replica data.</param>
        public async UniTask ShowReplica(IReplicaShowInfoProvider showInfo)
        {
            CancellationTokenSource cancellationSource = new CancellationTokenSource();

            _typingReplica.ClearText();

            Debug.Log($"Replica Text: {showInfo.ReplicaDialogueText.Text}");

            await DisplayActorName(showInfo.ReplicaActorsHolder);

            // Force layout rebuild if necessary for the replica text area
            LayoutRebuilder.ForceRebuildLayoutImmediate(_typingReplica.GetComponent<RectTransform>());

            _skipButton.gameObject.SetActive(true);

            _skipButton.onClick.AddListener(_typingReplica.Skip);

            await _typingReplica.TypeTextAsync(showInfo.ReplicaDialogueText.Text, cancellationSource.Token).SuppressCancellationThrow();

            _skipButton.onClick.RemoveAllListeners();

            _skipButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the visibility and text of the name plate based on the speaking actor.
        /// </summary>
        private async UniTask DisplayActorName(ActorsHolderModel actorsModel)
        {
            // Get the first (and typically only) actor in the map for replica nodes to determine the speaker
            var speakerAlignmentPair = actorsModel.ActorAlignmentMap.FirstOrDefault();

            foreach (var namePlateAlignmentPair in _namePlatesMap)
            {
                EActorAlignmentType currentAlignment = namePlateAlignmentPair.Key;
                NamePlateView namePlate = namePlateAlignmentPair.Value;

                // Show the name plate only if its alignment matches the speaker's alignment
                bool showName = currentAlignment == speakerAlignmentPair.Key && speakerAlignmentPair.Value != null;

                namePlate.gameObject.SetActive(showName);

                if (showName)
                {
                    await namePlate.SetName(speakerAlignmentPair.Value.Name.Text);
                }
            }
        }
    }
}