using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.HasPicked())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject);

                Card currentCard = hit.transform.GetComponent<Card>();
                currentCard.FlipOpen(true);
                GameManager.instance.AddCardToPickedList(currentCard);
            }
        }
    }
}
