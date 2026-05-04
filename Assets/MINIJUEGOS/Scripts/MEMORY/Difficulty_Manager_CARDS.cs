using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public int currentLevel = 0; // 0 = fácil, 1 = medio, 2 = difícil

    void Awake()
    {
        // Evitar duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseDifficulty()
    {
        if (currentLevel < 2)
            currentLevel++;
    }

    public int GetPairs()
    {
        switch (currentLevel)
        {
            case 0: return 6;
            case 1: return 10;
            case 2: return 15;
        }
        return 6;
    }

    public float GetRevealTime()
    {
        switch (currentLevel)
        {
            case 0: return 2f;
            case 1: return 1.5f;
            case 2: return 1f;
        }
        return 1.5f;
    }

    public int GetLevelTime()
    {
        switch (currentLevel)
        {
            case 0: return 40; // Nivel 1
            case 1: return 60; // Nivel 2
            case 2: return 90; // Nivel 3
        }
        return 40;
    }
}
