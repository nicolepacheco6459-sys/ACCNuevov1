using System.Collections.Generic;
using UnityEngine;

public class ChooseCharacterGameManager : MonoBehaviour
{
    public static ChooseCharacterGameManager Instance;

    public List<CharacterEntry> characters;

    private void Awake()
    {
        if(ChooseCharacterGameManager.Instance == null)
        {
            ChooseCharacterGameManager.Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
