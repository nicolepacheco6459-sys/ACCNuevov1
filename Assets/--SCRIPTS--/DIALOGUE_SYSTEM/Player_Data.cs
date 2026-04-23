using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;

    public enum Gender { Male, Female }
    public Gender playerGender;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
