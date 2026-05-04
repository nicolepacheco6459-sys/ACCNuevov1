using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public int pairAmount;
    public Sprite[] spriteList;

    public GameObject cardPrefab;
    public Transform boardAnchor;
    public float boardWidth = 6f;   // ancho máximo
    public float boardHeight = 4f;  // alto máximo

    float offSet = 1.2f;

    public List<GameObject> cardDeck = new List<GameObject>();

    [HideInInspector] public int width;
    [HideInInspector] public int height;

    void Start()
    {
        if (DifficultyManager.instance == null)
        {
            Debug.LogError("No hay DifficultyManager en la escena");
            return;
        }

        SetupDifficulty();

        if (GameManager_CARDS.instance != null)
        {
            GameManager_CARDS.instance.SetPairs(pairAmount);
        }
    }

    void SetupDifficulty()
    {
        pairAmount = DifficultyManager.instance.GetPairs();

        switch (pairAmount)
        {
            case 6:
                width = 4;
                height = 3;
                break;

            case 10:
                width = 5;
                height = 4;
                break;

            case 15:
                width = 6;
                height = 5;
                break;

            default:
                width = 4;
                height = 3;
                break;
        }
    }

    public void GenerateBoard()
    {
        CreatePlayField();
    }

    void CreatePlayField()
    {
        float dynamicOffsetX = boardWidth / (width - 1);
        float dynamicOffsetZ = boardHeight / (height - 1);

        cardDeck.Clear();

        // CREAR CARTAS
        for (int i = 0; i < pairAmount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
                newCard.GetComponent<Card>().SetCard(i, spriteList[i]);
                cardDeck.Add(newCard);
            }
        }
        Debug.Log("Width: " + width + " Height: " + height);
        Debug.Log("OffsetX: " + dynamicOffsetX + " OffsetZ: " + dynamicOffsetZ);


        // SHUFFLE
        for (int i = 0; i < cardDeck.Count; i++)
        {
            int randomIndex = Random.Range(0, cardDeck.Count);

            GameObject temp = cardDeck[i];
            cardDeck[i] = cardDeck[randomIndex];
            cardDeck[randomIndex] = temp;
        }

        // POSICIONAR
        int num = 0;
        Vector3 origin = Vector3.zero; // Añadido para definir 'origin'

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (num >= cardDeck.Count)
                    break;

                Vector3 pos = origin + new Vector3(x * dynamicOffsetX, 0, z * dynamicOffsetZ);

                Debug.Log("Asignando posición: " + pos);

                cardDeck[num].transform.position = pos;

                num++;
            }
        }


        CenterGrid();

    }

    void CenterGrid()
    {
        float totalWidth = (width - 1) * offSet;
        float totalHeight = (height - 1) * offSet;

        Vector3 offset = new Vector3(totalWidth / 2f, 0, totalHeight / 2f);

        foreach (GameObject card in cardDeck)
        {
            card.transform.position -= offset;
        }


    }
}
