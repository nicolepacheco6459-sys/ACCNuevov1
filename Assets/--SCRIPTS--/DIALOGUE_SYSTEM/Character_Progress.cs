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

    //  OBTENER PROGRESO
    public int GetProgress(string characterID)
    {
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogWarning("GetProgress: characterID vacío");
            return 0;
        }

        if (!progress.ContainsKey(characterID))
            progress[characterID] = 0;

        return progress[characterID];
    }

    // SUBIR PROGRESO 
    public void IncreaseProgress(string characterID)
    {
        if (string.IsNullOrEmpty(characterID))
        {
            Debug.LogError("IncreaseProgress: characterID vacío");
            return;
        }

        if (!progress.ContainsKey(characterID))
            progress[characterID] = 0;

        progress[characterID]++;

        Debug.Log($"Progreso de {characterID} → {progress[characterID]}");
    }

    //  OPCIONAL: SET DIRECTO
    public void SetProgress(string characterID, int value)
    {
        progress[characterID] = value;
    }
}