using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    /// <summary>
    /// Base class for UI Windows/Panels, implementing the Singleton pattern and basic visibility control.
    /// </summary>
    /// <typeparam name="TWindow">The type of the inheriting UI window class.</typeparam>
    public abstract class SampleUIWindow<TWindow> : MonoBehaviour where TWindow : SampleUIWindow<TWindow>
    {
        private static TWindow _instance;

        /// <summary>
        /// Gets the singleton instance of the window.
        /// </summary>
        public static TWindow Instance => _instance;

        private void Awake()
        {
            _instance = (TWindow)this;
            // Hide the window immediately after creation
            _instance.HideWindow();
        }

        /// <summary>
        /// Makes the window visible.
        /// </summary>
        public virtual void ShowWindow()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the window.
        /// </summary>
        public void HideWindow()
        {
            gameObject.SetActive(false);
        }
    }
}