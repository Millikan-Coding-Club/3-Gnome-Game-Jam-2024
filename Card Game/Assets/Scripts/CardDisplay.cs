using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Canvas cardCanvas;
    public Image cardImage;
    private GameObject manager;

    private bool letPlayerFlip = false;
    static public bool letPlayerFlipGlobal = true;
    public bool isFlipped = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager");
        cardImage.sprite = card.faceSprite;
    }

    private void OnMouseDown()
    {
        cardImage.color = Color.white;
        if (letPlayerFlip && letPlayerFlipGlobal) {flip();}
    }

    public void flip()
    {
        StartCoroutine(flipCard());
    }

    public IEnumerator flipCard()
    {
        float yRotation = cardImage.transform.rotation.eulerAngles.y;
        if (!isFlipped)
        {
            // Card is face up
            for (int i = 0; i < 180; i++)
            {
                if (i == 90)
                {
                    cardImage.sprite = card.backSprite;
                }
                cardImage.transform.Rotate(0, 1, 0);
                yRotation = cardImage.transform.rotation.eulerAngles.y;
                yield return new WaitForSeconds(0.001f);
            }

            letPlayerFlip = letPlayerFlipGlobal;
            isFlipped = true;

        } else
        {
            // Card is face down
            letPlayerFlip = false;
            for (int i = 180; i > 0; i--)
            {
                if (i == 90)
                {
                    cardImage.sprite = card.faceSprite;
                }
                cardImage.transform.Rotate(0, -1, 0);
                yRotation = cardImage.transform.rotation.eulerAngles.y;
                yield return new WaitForSeconds(0.001f);
            }

            isFlipped = false;
            GameManager.playerGuess += card.value;
            if (card.value == 1) { GameManager.aceCount++; }
        }
    }

    private void OnMouseEnter()
    {
        if (letPlayerFlip && letPlayerFlipGlobal) { cardImage.color = new Color(0.8f, 0.8f, 0.8f, 1); }
    }

    private void OnMouseExit()
    {
        cardImage.color = Color.white;
    }
}
