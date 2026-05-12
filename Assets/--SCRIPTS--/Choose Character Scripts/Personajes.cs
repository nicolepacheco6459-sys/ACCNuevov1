using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPErsonaje", menuName = "Personaje")]
public class Personaje : ScriptableObject
{
    public GameObject personajeJugable;
    public Sprite imagen;
    public string nombre;
}
