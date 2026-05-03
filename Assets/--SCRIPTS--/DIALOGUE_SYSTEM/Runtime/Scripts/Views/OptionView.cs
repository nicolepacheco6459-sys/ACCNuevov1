using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        public async UniTask<Guid> WaitForChoice(ChoiceOptionModel optionModel)
        {
            var completionSource = new UniTaskCompletionSource<Guid>();

            _text.text = optionModel.DialogueText.Text;

            _button.onClick.RemoveAllListeners();

            _button.onClick.AddListener(() =>
            {
                Debug.Log("Opción elegida: " + _text.text);

                int affinity = GetAffinityByText(_text.text);

                if (AffinityChoiceHandler.Instance != null)
                {
                    AffinityChoiceHandler.Instance.ApplyChoiceValue(affinity);
                }

                completionSource.TrySetResult(optionModel.RelatedNodeId);
            });

            return await completionSource.Task;
        }

        private int GetAffinityByText(string text)
        {
            text = text.ToLower();

            if (text.Contains("sí") || text.Contains("claro"))
                return 10;

            if (text.Contains("creo") || text.Contains("tal vez"))
                return 5;

            if (text.Contains("no") || text.Contains("exager"))
                return -5;

            return 0;
        }
    }
}