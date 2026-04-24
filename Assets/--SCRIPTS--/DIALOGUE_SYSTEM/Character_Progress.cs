using System.Collections.Generic;
using UnityEngine;

public class CharacterProgress : MonoBehaviour
{
    public static CharacterProgress Instance;

    private Dictionary<string, int> progress = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetProgress(string characterID)
    {
        if (!progress.ContainsKey(characterID))
            return 0;

        return progress[characterID];
    }

    public void AdvanceProgress(string characterID)
    {
        if (!progress.ContainsKey(characterID))
            progress[characterID] = 0;

        progress[characterID]++;
    }
}