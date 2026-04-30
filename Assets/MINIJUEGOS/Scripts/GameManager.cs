using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    bool picked; //SET THIS TRUE IF WE HAVE 2 CARDS PICKED 
    int pairs;
    int pairCounter;
    public bool hideMatches; // IF TRUE, MATCHES WILL BE HIDDEN, IF FALSE, MATCHES WILL BE DISABLED

    List<Card> pickedCards = new List<Card>();

    void Awake()
    {
        instance = this;
    }

    public void AddCardToPickedList(Card card)
    {
        pickedCards.Add(card);
        if(pickedCards.Count == 2)
        {
            picked = true;
            //CHECK IF THE CARDS MATCH
            StartCoroutine(CheckMatch());
        }
    }
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1.5f);
        if (pickedCards[0].GetCardId() == pickedCards[1].GetCardId())
        {
            //WE HAVE A MATCH
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
            yield return new WaitForSeconds(1.5f);
        }
       
        // CLEAN UP
        picked = false;
        pickedCards.Clear();
    }

    void CheckForWin()
    {
        if (pairs == pairCounter)
        {
            // WE WON
            Debug.Log("GANASTE");
        }
    }
    
    public void GameOver()
    {
        // GAME OVER
        Debug.Log("PERDISTE");
    }
    
    public bool HasPicked()
    {
        return picked;
    }

    public void SetPairs(int pairAmount)
    {
        pairs = pairAmount;
    }
}
