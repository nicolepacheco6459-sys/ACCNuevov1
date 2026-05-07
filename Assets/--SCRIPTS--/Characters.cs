using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterAsset", menuName = "Character Asset")]

public class CharacterAsset : ScriptableObject
{
    public GameObject playableCharacter;
    public Sprite image;
    public string name;
}
