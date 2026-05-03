using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ScoreManager_CARDS: MonoBehaviour
{
    public int timeForLevelToComplete = 60;
    public Image timerImage;
    public TMP_Text timeText;


    void Start()
    {
        StartCoroutine("Timer");

    }

    IEnumerator Timer()
    {
        int tempTime = timeForLevelToComplete;
        timeText.text = tempTime.ToString();
        while (tempTime > 0)
        {
            tempTime--;
            yield return new WaitForSeconds(1);

            timerImage.fillAmount = tempTime / (float)timeForLevelToComplete;
            timeText.text = tempTime.ToString();
        }
        // GAME OVER
        GameManager.instance.GameOver();

    }


}


