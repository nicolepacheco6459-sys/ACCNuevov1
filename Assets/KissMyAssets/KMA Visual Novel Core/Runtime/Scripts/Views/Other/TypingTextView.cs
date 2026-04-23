using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TypingTextView : MonoBehaviour
    {
        [SerializeField] private float _charactersPerSecond = 30f;

        private TextMeshProUGUI _textComponent;

        private CancellationTokenSource _cancellationSource;

        private string _fullText;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        public async UniTask TypeTextAsync(string fullText, CancellationToken token = default)
        {
            CancelTyping();

            _fullText = fullText;

            _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            var localToken = _cancellationSource.Token;

            _textComponent.text = "";
            float delay = 1f / _charactersPerSecond;

            for (int i = 0; i <= fullText.Length; i++)
            {
                if (localToken.IsCancellationRequested) return;

                _textComponent.text = fullText.Substring(0, i);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: localToken);
            }

            _cancellationSource = null;
        }

        public void ClearText()
        {
            _textComponent.text = string.Empty;
        }

        public void Skip()
        {
            _textComponent.text = _fullText;

            if (_cancellationSource != null)
            {
                _cancellationSource.Cancel();
                _cancellationSource = null;
            }
        }

        private void CancelTyping()
        {
            if (_cancellationSource != null)
            {
                _cancellationSource.Cancel();
                _cancellationSource.Dispose();
                _cancellationSource = null;
            }
        }
    }
}