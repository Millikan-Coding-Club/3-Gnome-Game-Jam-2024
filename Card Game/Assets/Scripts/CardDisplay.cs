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

    public bool letPlayerFlip = false;
    private bool guessIsCorrect = false;
    static private int aceCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        cardImage.sprite = card.faceSprite;
    }

    private void OnMouseDown()
    {
        if (letPlayerFlip) {flip();}
    }

    public void flip()
    {
        StartCoroutine(flipCard());
    }

    public IEnumerator flipCard()
    {
        float yRotation = cardImage.transform.rotation.eulerAngles.y;
        if (yRotation == 0)
        {
            // Card is face up
            while (yRotation < 90)
            {
                yRotation += 1f;
                cardImage.transform.Rotate(0, 1f, 0);
                yield return new WaitForSeconds(0.001f);
            }
            cardImage.sprite = card.backSprite;
            while (yRotation < 180)
            {
                yRotation += 1f;
                cardImage.transform.Rotate(0, 1f, 0);
                yield return new WaitForSeconds(0.001f);
            }
            letPlayerFlip = true;
        } else
        {
            // Card is face down
            while (yRotation > 90)
            {
                yRotation -= 1f;
                cardImage.transform.Rotate(0, -1f, 0);
                yield return new WaitForSeconds(0.001f);
            }
            cardImage.sprite = card.faceSprite;
            while (yRotation > 0)
            {
                yRotation -= 1f;
                cardImage.transform.Rotate(0, -1f, 0);
                yield return new WaitForSeconds(0.001f);
                // TODO: fix card flip
            }
            GameManager.playerGuess += card.value;
            if (card.value == 1) { aceCount++; }
        }
        // Check player's guess after they flip a card
        if (GameManager.playerGuess == GameManager.target)
        {
            guessIsCorrect = true;
        }

        if (!guessIsCorrect) // Accounts for aces
        {
            for (int i = 1; i <= aceCount; i++)
            {
                if (GameManager.playerGuess + 10 * i == GameManager.target)
                {
                    guessIsCorrect = true;
                }
            }
        }

        if (guessIsCorrect)
        {
            Debug.Log("You winned :)");
        }
    }
}
