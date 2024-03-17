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
    public AudioSource flipAudio;
    private GameObject leaveButton;

    private bool letPlayerFlip = false;
    static public bool letPlayerFlipGlobal = true;
    public bool isFlipped = false;
    [SerializeField] float flipSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        leaveButton = GameObject.Find("LeaveTableButton");
        cardImage.sprite = card.faceSprite;
    }

    private void OnMouseDown()
    {
        cardImage.color = Color.white;
        if (letPlayerFlip && letPlayerFlipGlobal)
        {
            leaveButton.GetComponent<Button>().interactable = false;
            flip(true);
            GameManager.playedCards.Add(gameObject);
        }
    }

    public void flip(bool addCard)
    {
        StartCoroutine(flipCard(addCard));
    }

    public IEnumerator flipCard(bool addCard)
    {
        flipAudio.Play();
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
                yield return new WaitForSeconds(1 / flipSpeed);
            }

            letPlayerFlip = letPlayerFlipGlobal;
            isFlipped = true;

        } else
        {
            // Card is face down
            if (addCard) { GameManager.playerGuess += card.value; }
            if (card.value == 1 && addCard) { GameManager.aceCount++; }
            letPlayerFlip = false;
            for (int i = 180; i > 0; i--)
            {
                if (i == 90)
                {
                    cardImage.sprite = card.faceSprite;
                }
                cardImage.transform.Rotate(0, -1, 0);
                yRotation = cardImage.transform.rotation.eulerAngles.y;
                yield return new WaitForSeconds(1 / flipSpeed);
            }
            isFlipped = false;
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
