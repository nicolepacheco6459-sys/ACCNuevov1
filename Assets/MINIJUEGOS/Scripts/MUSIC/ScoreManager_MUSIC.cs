using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    int misses = 0;
    public int maxMisses = 10;
    public GameManager_MUSIC gameManager;
    public static ScoreManager Instance;

    public AudioSource hitSFX;
    public AudioSource missSFX;
    public TextMeshPro scoreText;
    public TextMeshProUGUI feedbackText;

   

    int comboScore;

    void Awake()
    {
        Instance = this;
        comboScore = 0;
    }
    public void ShowFeedback(string text)
    {
        StopAllCoroutines();
        StartCoroutine(FeedbackRoutine(text));
    }

    IEnumerator FeedbackRoutine(string text)
    {
        feedbackText.text = text;
        feedbackText.alpha = 1;

        float duration = 0.5f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            feedbackText.alpha = 1 - (t / duration);

            feedbackText.transform.localScale = Vector3.Lerp(
                Vector3.one * 1.2f,
                Vector3.one,
                t / duration
            );

            yield return null;
        }

        feedbackText.text = "";

        if (text == "Perfect")
            feedbackText.color = Color.cyan;
        else if (text == "Good")
            feedbackText.color = Color.yellow;
        else
            feedbackText.color = Color.red;
    }
    public static void Hit(string accuracy)
    {
        Instance.comboScore += 1;
        Instance.hitSFX.Play();

        Instance.ShowFeedback(accuracy);
    }

    public static void Miss()
    {
        Instance.comboScore = 0;
        Instance.missSFX.Play();

        Instance.misses++;

        Instance.ShowFeedback("Miss");

        if (Instance.misses >= Instance.maxMisses)
        {
            Instance.gameManager.GameOver();
        }
    }

    void Update()
    {
        scoreText.text = Instance.comboScore.ToString();
    }
    
}
