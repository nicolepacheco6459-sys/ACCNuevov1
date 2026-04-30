using UnityEngine;

public class Dialogue_ChoiceHandler : MonoBehaviour
{
    public void AddAffinity(string characterID, int value)
    {
        AffinitySystem.Instance.AddAffinity(characterID, value);
    }
}
