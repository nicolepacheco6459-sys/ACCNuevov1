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

        private void Awake()
        {
            // 🔧 Auto-reparar referencias si faltan
            if (_button == null)
            {
                _button = GetComponent<Button>();
                Debug.LogWarning("🔧 Button auto-asignado en " + gameObject.name);
            }

            if (_text == null)
            {
                _text = GetComponentInChildren<TextMeshProUGUI>();
                Debug.LogWarning("🔧 Text auto-asignado en " + gameObject.name);
            }
        }

        private void Start()
        {
            // 🔍 DEBUG DE UI
            RectTransform rt = GetComponent<RectTransform>();

            Debug.Log($"[OptionView INIT] {gameObject.name}");
            Debug.Log($"➡ Tamaño: {rt.rect.width} x {rt.rect.height}");
            Debug.Log($"➡ Botón asignado: {_button != null}");
            Debug.Log($"➡ Texto asignado: {_text != null}");
        }

        public async UniTask<Guid> WaitForChoice(ChoiceOptionModel optionModel)
        {
            var completionSource = new UniTaskCompletionSource<Guid>();

            if (_text != null)
                _text.text = optionModel.DialogueText.Text;
            else
                Debug.LogError("❌ TextMeshPro no asignado en OptionView");

            if (_button == null)
            {
                Debug.LogError("❌ Button no asignado en OptionView");
                return optionModel.RelatedNodeId;
            }

            // 🔥 LIMPIAR listeners
            _button.onClick.RemoveAllListeners();

            // 🔥 DEBUG VISUAL
            Debug.Log($"🟡 Opción creada: {_text.text}");

            // 🔥 EVENTO CLICK
            _button.onClick.AddListener(() =>
            {
                Debug.Log("🟢 CLICK DETECTADO EN BOTÓN");
                Debug.Log("➡ Opción elegida: " + _text.text);

                int affinity = GetAffinityByText(_text.text);

                Debug.Log("➡ Afinidad aplicada: " + affinity);

                if (AffinityChoiceHandler.Instance != null)
                {
                    AffinityChoiceHandler.Instance.ApplyChoiceValue(affinity);
                }
                else
                {
                    Debug.LogWarning("⚠️ AffinityChoiceHandler no encontrado");
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
