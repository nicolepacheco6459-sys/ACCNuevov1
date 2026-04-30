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

        public async UniTask<Guid> WaitForChoice(ChoiceOptionModel optionModel, int optionIndex)
        {
            var completionSource = new UniTaskCompletionSource<Guid>();

            // Texto visible de la opción
            _text.text = optionModel.DialogueText.Text;

            Action onClickAction = () =>
            {
                Debug.Log("Opción elegida: " + _text.text);

                // 🔥 AFINIDAD SEGÚN POSICIÓN
                int affinity = GetAffinityByIndex(optionIndex);

                // 🔥 APLICAR AFINIDAD
                if (AffinityChoiceHandler.Instance != null)
                {
                    AffinityChoiceHandler.Instance.ApplyChoiceValue(affinity);
                }

                // 🔥 CONTINUAR DIÁLOGO
                completionSource.TrySetResult(optionModel.RelatedNodeId);
            };

            _button.onClick.AddListener(onClickAction.Invoke);

            var result = await completionSource.Task;

            _button.onClick.RemoveListener(onClickAction.Invoke);

            return result;
        }

        // =========================
        // AFINIDAD POR ÍNDICE
        // =========================
        private int GetAffinityByIndex(int index)
        {
            switch (index)
            {
                case 0: return 10;   // buena
                case 1: return 5;    // neutral
                case 2: return -5;   // mala
                default: return 0;
            }
        }
    }
}