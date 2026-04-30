using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    private Dictionary<string, bool> unlockedMinigames = new();

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockMinigame(string id)
    {
        unlockedMinigames[id] = true;
        Debug.Log("Minijuego desbloqueado: " + id);
    }

    public bool IsMinigameUnlocked(string id)
    {
        return unlockedMinigames.ContainsKey(id) && unlockedMinigames[id];
    }
}
