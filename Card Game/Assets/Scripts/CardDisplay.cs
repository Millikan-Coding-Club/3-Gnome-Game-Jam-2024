using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Canvas cardCanvas;
    public Image cardImage;
    [SerializeField] GameObject manager;

    public bool letPlayerFlip = false;

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
        manager.GetComponent<GameManager>().playerGuess += card.value;
        Debug.Log("Player guess: " + manager.GetComponent<GameManager>().playerGuess);
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
            
        } else
        {
            // Card is flipped

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
        }
    }
}
