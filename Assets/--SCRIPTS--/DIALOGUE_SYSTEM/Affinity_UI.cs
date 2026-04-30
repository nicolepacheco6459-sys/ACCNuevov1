using UnityEngine;
using UnityEngine.UI;

public class AffinityUI : MonoBehaviour
{
    public string characterID;
    public Image[] hearts;

    void Start()
    {
        // 🔥 TEST: fuerza afinidad para verificar que se vean los corazones
        if (AffinitySystem.Instance != null)
        {
            AffinitySystem.Instance.AddAffinity(characterID, 60);
            Debug.Log("Afinidad inicial forzada para test");
        }
        else
        {
            Debug.LogError("AffinitySystem no encontrado en escena");
        }
    }

    void Update()
    {
        if (AffinitySystem.Instance == null)
            return;

        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogError("Hearts no asignados en AffinityUI");
            return;
        }

        int affinity = AffinitySystem.Instance.GetAffinity(characterID);
        int level = affinity / 20;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].enabled = i < level;
        }
    }
}

