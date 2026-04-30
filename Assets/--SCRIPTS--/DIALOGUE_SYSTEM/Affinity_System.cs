using System.Collections.Generic;
using UnityEngine;

public class AffinitySystem : MonoBehaviour
{
    public static AffinitySystem Instance;

    private Dictionary<string, int> affinity = new Dictionary<string, int>();

    private void Awake()
    {
        // 🔒 Evita duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("AffinitySystem inicializado correctamente");
    }

    // ❤️ Añadir afinidad
    public void AddAffinity(string characterID, int amount)
    {
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogWarning("Intento de sumar afinidad sin characterID");
            return;
        }

        if (!affinity.ContainsKey(characterID))
            affinity[characterID] = 0;

        affinity[characterID] += amount;
    }

    // 📊 Obtener afinidad
    public int GetAffinity(string characterID)
    {
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogError("characterID está vacío en GetAffinity");
            return 0;
        }

        if (!affinity.ContainsKey(characterID))
        {
            return 0;
        }

        return affinity[characterID];
    }
}
