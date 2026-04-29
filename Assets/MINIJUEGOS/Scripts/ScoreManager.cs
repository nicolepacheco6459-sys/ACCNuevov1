using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TextMeshPro scoreText;

    int comboScore;

    void Awake()
    {
        Instance = this;
        comboScore = 0;
    }

    public static void Hit()
    {
        Instance.comboScore += 1;
        Instance.hitSFX.Play();
    }

    public static void Miss()
    {
        Instance.comboScore = 0; // normalmente los rhythm games reinician combo
        Instance.missSFX.Play();
    }

    void Update()
    {
        scoreText.text = Instance.comboScore.ToString();
    }
}
