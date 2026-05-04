using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class ChoiceView : MonoBehaviour
    {
        [SerializeField] private OptionView _optionPrefab;
        [SerializeField] private VerticalLayoutGroup _optionLayoutGroup;

        private readonly List<OptionView> _spawnedOptions = new();

        public async UniTask<Guid> ShowChoice(IChoiceShowInfoProvider showInfo)
        {
            List<UniTask<Guid>> tasks = SpawnOptions(showInfo);

            var result = (await UniTask.WhenAny(tasks)).result;

            ClearOptions();

            return result;
        }

        private List<UniTask<Guid>> SpawnOptions(IChoiceShowInfoProvider showInfo)
        {
            List<UniTask<Guid>> tasks = new();

            foreach (var option in showInfo.ChoiceOptionsHolder.Options)
            {
                OptionView optionView = Instantiate(_optionPrefab, _optionLayoutGroup.transform);

                // LLAMADA CORRECTA (SIN INDEX)
                tasks.Add(optionView.WaitForChoice(option));

                _spawnedOptions.Add(optionView);
            }

            return tasks;
        }

        private void ClearOptions()
        {
            foreach (var option in _spawnedOptions)
            {
                Destroy(option.gameObject);
            }

            _spawnedOptions.Clear();
        }
    }
}