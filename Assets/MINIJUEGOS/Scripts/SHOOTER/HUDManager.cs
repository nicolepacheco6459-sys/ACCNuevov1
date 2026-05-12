using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    public TextMeshProUGUI timerText;

    void Update()
    {
        if (GameManager_SHOOTER.instance == null) return;

        levelText.text =
            "LEVEL " + GameManager_SHOOTER.instance.currentLevel;

        float time = GameManager_SHOOTER.instance.GetRemainingTime();

        timerText.text =
            Mathf.CeilToInt(time).ToString();
    }
}
