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

            int index = 0;

            foreach (var option in showInfo.ChoiceOptionsHolder.Options)
            {
                OptionView optionView = Instantiate(_optionPrefab, _optionLayoutGroup.transform);

                // 🔥 PASAMOS EL ÍNDICE DE LA OPCIÓN
                tasks.Add(optionView.WaitForChoice(option, index));

                _spawnedOptions.Add(optionView);

                index++;
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