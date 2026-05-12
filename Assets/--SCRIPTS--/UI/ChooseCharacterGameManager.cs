using System.Collections.Generic;
using UnityEngine;

public class ChooseCharacterGameManager : MonoBehaviour
{
    public static ChooseCharacterGameManager Instance;

    public List<Personaje> personajes;

    private void Awake()
    {
        if (ChooseCharacterGameManager.Instance == null)
        {
            ChooseCharacterGameManager.Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
