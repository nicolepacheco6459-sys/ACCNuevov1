using UnityEngine;
using UnityEngine.UI;

public class AffinityUI : MonoBehaviour
{
    public static AffinityUI Instance;

    [Header("Personaje actual")]
    public string characterID;

    [Header("Corazones")]
    public Image[] hearts;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCharacter(string id)
    {
        characterID = id;
        UpdateHearts();
    }

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (AffinitySystem.Instance == null || hearts == null || hearts.Length == 0)
            return;

        if (string.IsNullOrEmpty(characterID))
            return;

        int affinity = AffinitySystem.Instance.GetAffinity(characterID);

        int level = affinity / 20;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < level;
        }
    }
}

