using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Canvas cardCanvas;
    public Image cardImage;

    // Start is called before the first frame update
    void Start()
    {
        cardImage.sprite = card.faceSprite;
        StartCoroutine(flipCard());
    }

    private void Update()
    {
        
    }
    public IEnumerator flipCard()
    {
        while (cardCanvas.transform.rotation.eulerAngles.y < 90)
        {
            cardCanvas.transform.Rotate(0, 1f, 0);
            yield return new WaitForSeconds(0.001f);
        }
        if (cardImage.sprite == card.faceSprite)
        {
            cardImage.sprite = card.backSprite;
        } else
        {
            cardImage.sprite = card.faceSprite;
        }
        while (cardCanvas.transform.rotation.eulerAngles.y < 180)
        {
            cardCanvas.transform.Rotate(0, 1f, 0);
            yield return new WaitForSeconds(0.001f);
        }
    }
}
