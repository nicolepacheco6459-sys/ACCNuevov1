using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public int pairAmount;
    public Sprite[] spriteList;

    float offSet = 1.2f; // OFFSET BETWEEN CARDS 
    public GameObject cardPrefab;

    public List<GameObject> cardDeck = new List<GameObject>();
    [HideInInspector] public int width;
    [HideInInspector] public int height;


    void Start()
    {
        GameManager.instance.SetPairs(pairAmount);
        CreatePlayField();
    }

    void CreatePlayField()
    {
        for (int i = 0; i < pairAmount; i++)
        {
            for (int j = 0; j < 2; j++) 
            {
                Vector3 pos = Vector3.zero; //new Vector3(i * offSet, 0, 0);
                GameObject newCard = Instantiate(cardPrefab, pos, Quaternion.identity);
                newCard.GetComponent<Card>().SetCard(i, spriteList[i]);
                cardDeck.Add(newCard);
            }
            
        }
        // SHUFFLE CARDS
        for (int i = 0; i < cardDeck.Count; i++)
        {
            int index = Random.Range(0, cardDeck.Count);
            var temp = cardDeck[i];
            cardDeck[i] = cardDeck[index];
            cardDeck[index] = temp;

        }
        // PAST OUT CARDS ON FIELD 

        int num = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * offSet, 0, z * offSet);
                cardDeck[num].transform.position = pos;
                num++;
            }
        }
    }

    void OnDrawGizmos()
    {
         for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * offSet, 0, z * offSet);
                Gizmos.DrawWireCube(pos, new Vector3(1, 0.1f, 1));
            }
        }
    }

}
