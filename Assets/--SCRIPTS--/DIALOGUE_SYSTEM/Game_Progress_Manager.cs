using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    private Dictionary<string, bool> unlocked = new();
    private Dictionary<string, bool> completed = new();

    private void Awake()
    {
        Instance = this;
    }

    public void UnlockMinigame(string id)
    {
        unlocked[id] = true;
    }

    public void CompleteMinigame(string id)
    {
        completed[id] = true;
    }

    public bool IsUnlocked(string id)
    {
        return unlocked.ContainsKey(id) && unlocked[id];
    }

    public bool IsCompleted(string id)
    {
        return completed.ContainsKey(id) && completed[id];
    }
}
