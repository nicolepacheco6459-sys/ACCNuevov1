using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Manages the display and positioning of actors on the screen.
    /// </summary>
    public class ActorsView : MonoBehaviour
    {
        [Header("Actor Views")]
        [SerializeField] private Image _leftActor;
        [SerializeField] private Image _centralActor;
        [SerializeField] private Image _rightActor;

        private readonly Dictionary<EActorAlignmentType, Image> _actorsViewMap = new();

        private void Awake()
        {
            // Initialize the map linking alignment types to their corresponding Image components
            _actorsViewMap.Add(EActorAlignmentType.Left, _leftActor);
            _actorsViewMap.Add(EActorAlignmentType.Center, _centralActor);
            _actorsViewMap.Add(EActorAlignmentType.Right, _rightActor);
        }

        /// <summary>
        /// Updates all actor views based on the provided actor holder model.
        /// </summary>
        /// <param name="actorsModel">The model containing actors and their alignments.</param>
        public async UniTask ShowActors(ActorsHolderModel actorsModel)
        {
            foreach (var viewAlignmentPair in _actorsViewMap)
            {
                EActorAlignmentType actorAlignment = viewAlignmentPair.Key;
                Image actorView = viewAlignmentPair.Value;

                // Show or hide each specific actor view
                await ShowActor(actorsModel, actorAlignment, actorView);
            }
        }

        /// <summary>
        /// Handles the display logic for a single actor view.
        /// </summary>
        private async UniTask ShowActor(ActorsHolderModel actorsModel, EActorAlignmentType alignment, Image view)
        {
            if (view == null) return;

            bool isActorPresent = actorsModel.ActorAlignmentMap.TryGetValue(alignment, out DialogueActorModel actorModel);

            // Move active actor to the front (optional but good for visual hierarchy)
            view.transform.SetAsFirstSibling();
            view.gameObject.SetActive(isActorPresent);

            if (isActorPresent)
            {
                view.sprite = actorModel.Sprite;
                // Fade in or other animation can be added here
            }

            // A yield is necessary if any visual change (like an animation) is intended
            await UniTask.Yield();
        }
    }
}