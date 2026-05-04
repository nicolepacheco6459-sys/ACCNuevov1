using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager_CARDS : MonoBehaviour
{
    public static GameManager_CARDS instance;

    bool picked;
    int pairs;
    int pairCounter;

    public bool hideMatches;

    List<Card> pickedCards = new List<Card>();

    float revealTime;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Tiempo de revelado según dificultad
        revealTime = DifficultyManager.instance.GetRevealTime();
    }

    public void AddCardToPickedList(Card card)
    {
        pickedCards.Add(card);

        if (pickedCards.Count == 2)
        {
            picked = true;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(revealTime);

        if (pickedCards[0].GetCardId() == pickedCards[1].GetCardId())
        {
            // MATCH
            if (hideMatches)
            {
                pickedCards[0].gameObject.SetActive(false);
                pickedCards[1].gameObject.SetActive(false);
            }
            else
            {
                pickedCards[0].GetComponent<BoxCollider>().enabled = false;
                pickedCards[1].GetComponent<BoxCollider>().enabled = false;
            }

            pairCounter++;
            CheckForWin();
        }
        else
        {
            pickedCards[0].FlipOpen(false);
            pickedCards[1].FlipOpen(false);

            yield return new WaitForSeconds(revealTime);
        }

        picked = false;
        pickedCards.Clear();
    }

    void CheckForWin()
    {
        if (pairs == pairCounter)
        {
            Debug.Log("GANASTE");

            // Subir dificultad automáticamente
            DifficultyManager.instance.IncreaseDifficulty();

            StartCoroutine(ReturnToMainMenu());
        }
    }

    public void GameOver()
    {
        Debug.Log("PERDISTE");

        // Mostrar UI de derrota
        UIManager_CARDS.instance.ShowLosePanel();
    }

    [SerializeField] string mainSceneName = "Samantha";

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);

        // ⚠️ Cambia el nombre por el de tu escena real
        SceneManager.LoadScene("mainSceneName");
    }

    public bool HasPicked()
    {
        return picked;
    }

    public void SetPairs(int pairAmount)
    {
        pairs = pairAmount;
        pairCounter = 0; // 🔥 IMPORTANTE: reiniciar contador
    }
}