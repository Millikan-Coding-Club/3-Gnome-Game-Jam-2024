using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();
    private List<GameObject> handObjects = new List<GameObject>();
    public Transform[] cardSpawns;
    [SerializeField] private TMP_Text targetText;

    private GameObject card;
    private int target = 0;
    private int targetCardCount;
    public int playerGuess = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cardSpawns.Length; i++)
        {
            int rand = Random.Range(0, deck.Count);
            card = Instantiate(cardPrefab, cardSpawns[i].transform.position, cardSpawns[0].transform.rotation);
            card.GetComponent<CardDisplay>().card = deck[rand];
            deck.RemoveAt(rand);
            hand.Add(card.GetComponent<CardDisplay>().card);
            handObjects.Add(card);
        }
        SetTarget();
        Invoke("flipAllCards", 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetTarget()
    {
        target = 0;
        targetCardCount = Random.Range(1, hand.Count);
        List<Card> handCopy = new List<Card>(hand);
        foreach (Card card in hand)
        {
            if (targetCardCount >= 1)
            {
                int rand = Random.Range(1, handCopy.Count);
                // 1/2 chance to add 11 instead of 1 for aces
                if (handCopy[rand].value == 1 && Random.Range(0, 1) == 1)
                {
                    target += 11;
                } else
                {
                    target += handCopy[rand].value;
                }
                Debug.Log(handCopy[rand].value);
                handCopy.RemoveAt(rand);
                targetCardCount--;
            }
        }
        targetText.text = target.ToString();
    }

    private void flipAllCards()
    {
        foreach (GameObject obj in handObjects)
        {
            obj.GetComponent<CardDisplay>().flip();
            obj.GetComponent<CardDisplay>().letPlayerFlip = true;
        }
    }
}
