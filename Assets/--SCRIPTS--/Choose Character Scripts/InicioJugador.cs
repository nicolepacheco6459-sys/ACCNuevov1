using UnityEngine;

public class InicioJugador : MonoBehaviour
{
    private void Start()
    {
        int indexJugador = PlayerPrefs.GetInt("IndexJugador");
        Instantiate(ChooseCharacterGameManager.Instance.personajes[indexJugador].personajeJugable, transform.position, Quaternion.identity);
    }


}
