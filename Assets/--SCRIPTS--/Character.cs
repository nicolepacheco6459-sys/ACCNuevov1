using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character")]

public class Character : ScriptableObject
{
    public GameObject playableCharacter;
    public Sprite image;
    public string name;
}
