using UnityEngine;

public class GenderDebug : MonoBehaviour
{
    private void Start()
    {
        int index = PlayerPrefs.GetInt("JugadorIndex");

        Debug.Log(" JugadorIndex recibido: " + index);

        if (PlayerCharacterData.IsFemale())
        {
            Debug.Log(" JUGADOR ACTUAL: FEMENINO");
        }
        else
        {
            Debug.Log(" JUGADOR ACTUAL: MASCULINO");
        }
    }
}
