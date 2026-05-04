using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    int cardId;
    public SpriteRenderer cardFront;
    public Animator anim;

    public void SetCard(int _id, Sprite _sprite)
    {
        cardId = _id;
        cardFront.sprite = _sprite;
    }

    public void FlipOpen(bool flipped)
    {
         anim.SetBool("FlippedOpen", flipped);
    }

    public int GetCardId()
    {
        return cardId;
    }
}
