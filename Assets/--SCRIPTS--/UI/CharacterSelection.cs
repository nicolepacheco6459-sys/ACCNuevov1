using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterEntry
{
    public Sprite image;
    public string characterName;
}

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance;
    public List<CharacterEntry> characters = new List<CharacterEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
