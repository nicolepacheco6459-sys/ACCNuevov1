using UnityEngine;
using UnityEngine.UI;

public class AffinityUI : MonoBehaviour
{
    public string characterID;
    public Image[] hearts;

    void Update()
    {
        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        int level = affinity / 20;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < level;
        }
    }
}
