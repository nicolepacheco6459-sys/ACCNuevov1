using UnityEngine;

public class DialogueChoiceHandler : MonoBehaviour
{
    public void AddAffinity(string characterID, int value)
    {
        AffinitySystem.Instance.AddAffinity(characterID, value);
    }
}
