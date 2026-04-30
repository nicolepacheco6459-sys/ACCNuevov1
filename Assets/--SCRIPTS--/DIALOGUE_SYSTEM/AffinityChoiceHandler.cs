using UnityEngine;

public class AffinityChoiceHandler : MonoBehaviour
{
    public static AffinityChoiceHandler Instance;

    [Header("NPC actual")]
    public string currentCharacterID;

    private void Awake()
    {
        Instance = this;
    }

    // 🔥 NUEVO MÉTODO (EL QUE FALTABA)
    public void ApplyChoiceValue(int value)
    {
        if (AffinitySystem.Instance == null)
        {
            Debug.LogError("AffinitySystem no encontrado");
            return;
        }

        if (string.IsNullOrEmpty(currentCharacterID))
        {
            Debug.LogError("CharacterID vacío en AffinityChoiceHandler");
            return;
        }

        AffinitySystem.Instance.AddAffinity(currentCharacterID, value);

        Debug.Log($"[{currentCharacterID}] Afinidad {(value >= 0 ? "+" : "")}{value}");
    }
}
