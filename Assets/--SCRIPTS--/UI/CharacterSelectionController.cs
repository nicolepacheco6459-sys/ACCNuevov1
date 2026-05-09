using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectionController : MonoBehaviour
{
    public static CharacterSelectionController Instance;

    public List<Character> characters;

    private void Awake()
    {
        if (CharacterSelectionController.Instance == null)
        {
            CharacterSelectionController.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
