using UnityEngine;
using UnityEngine.UI;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class SmartBackgroundView : MonoBehaviour
    {
        [SerializeField] private bool _fitInParent;

        private Image _background;
        private AspectRatioFitter _aspect;

        private Sprite _pendingBackgroundSprite = null;

        private void Awake()
        {
            _background = GetComponent<Image>();
            _aspect = GetComponent<AspectRatioFitter>();

            _aspect.aspectMode = _fitInParent ? AspectRatioFitter.AspectMode.FitInParent : AspectRatioFitter.AspectMode.EnvelopeParent;

            SyncAspectWithBackground();
        }

        private void Update()
        {
            if (IsBackgroundSpriteChanged())
                SyncAspectWithBackground();
        }

        private bool IsBackgroundSpriteChanged()
        {
            return _pendingBackgroundSprite != _background.sprite;
        }

        private void SyncAspectWithBackground()
        {
            _pendingBackgroundSprite = _background.sprite;

            if (_background.sprite != null)
            {
                Rect rect = _background.sprite.rect;

                _aspect.aspectRatio = rect.width / rect.height;

                return;
            }

            _aspect.aspectRatio = 1.0f;
        }
    }
}
