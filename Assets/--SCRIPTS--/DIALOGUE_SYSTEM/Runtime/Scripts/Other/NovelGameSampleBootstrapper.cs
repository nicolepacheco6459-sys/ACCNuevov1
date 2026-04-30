using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Runtime
{
    public class NovelGameSampleBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            NovelSampleMainMenuWindow.Instance.ShowWindow();
            NovelSampleMainMenuWindow.Instance.InitMainMenu();
        }
    }
}
