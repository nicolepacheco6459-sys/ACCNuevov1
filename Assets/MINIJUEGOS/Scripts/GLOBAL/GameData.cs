using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    [Header("Player Data")]
    public bool isFemale;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGender(bool female)
    {
        isFemale = female;

        Debug.Log("👤 Género seleccionado: " + (female ? "Femenino" : "Masculino"));
    }
}
