using System.Collections.Generic;
using UnityEngine;

public class AffinitySystem : MonoBehaviour
{
    public static AffinitySystem Instance;

    private Dictionary<string, int> affinity = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddAffinity(string characterID, int amount)
    {
        if (!affinity.ContainsKey(characterID))
            affinity[characterID] = 0;

        affinity[characterID] += amount;
    }

    public int GetAffinity(string characterID)
    {
        if (!affinity.ContainsKey(characterID))
            return 0;

        return affinity[characterID];
    }
}
