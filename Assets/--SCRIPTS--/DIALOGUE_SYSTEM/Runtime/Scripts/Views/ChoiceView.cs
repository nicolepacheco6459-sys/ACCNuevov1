using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Manages the creation and display of dialogue choice options.
    /// </summary>
    public class ChoiceView : MonoBehaviour
    {
        [SerializeField] private OptionView _optionPrefab;
        [SerializeField] private VerticalLayoutGroup _optionLayoutGroup;

        private readonly List<OptionView> _spawnedOptions = new();

        /// <summary>
        /// Instantiates options based on the model, displays them, and waits for a single selection.
        /// </summary>
        /// <param name="choiceModel">The model containing the choice options.</param>
        /// <returns>The <see cref="Guid"/> of the next node from the selected option.</returns>
        public async UniTask<Guid> ShowChoice(IChoiceShowInfoProvider showInfo)
        {
            List<UniTask<Guid>> tasks = SpawnOptions(showInfo);

            // Wait until any of the option tasks completes (i.e., one button is clicked)
            var result = (await UniTask.WhenAny(tasks)).result;

            ClearOptions();

            return result;
        }

        /// <summary>
        /// Spawns option UI elements and initiates the wait for user input.
        /// </summary>
        /// <param name="showInfo">The model containing the option data.</param>
        /// <returns>A list of <see cref="UniTask{T}"/>, one for each option.</returns>
        private List<UniTask<Guid>> SpawnOptions(IChoiceShowInfoProvider showInfo)
        {
            List<UniTask<Guid>> tasks = new();

            foreach (var option in showInfo.ChoiceOptionsHolder.Options)
            {
                OptionView optionView = Instantiate(_optionPrefab, _optionLayoutGroup.transform);

                tasks.Add(optionView.WaitForChoice(option));
                _spawnedOptions.Add(optionView);
            }

            return tasks;
        }

        /// <summary>
        /// Destroys all currently spawned option UI elements.
        /// </summary>
        private void ClearOptions()
        {
            foreach (var option in _spawnedOptions)
            {
                // Ensure to destroy the GameObject, not just the component
                Destroy(option.gameObject);
            }

            _spawnedOptions.Clear();
        }
    }
}