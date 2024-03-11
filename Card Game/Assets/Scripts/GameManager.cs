using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public List<Card> deck = new List<Card>();
    private List<Card> hand = new List<Card>();
    private List<GameObject> handObjects = new List<GameObject>();
    static public List<GameObject> playedCards = new List<GameObject>();
    public Transform[] cardSpawns;
    [SerializeField] private TMP_Text targetText;
    private GameObject card;
    [SerializeField] private GameObject button;
    public GameObject deckPrefab;

    static public int target = 0;
    private int targetCardCount;
    static public int playerGuess = 0;
    static public int aceCount = 0;
    private bool guessIsCorrect = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setUpCards());
        Invoke("flipAllCards", 3);
    }

    private IEnumerator setUpCards()
    {
        for (int i = 0; i < cardSpawns.Length; i++)
        {
            StartCoroutine(drawCard(cardSpawns[i].position));
            yield return new WaitForSeconds(0.1f);
        }
        SetTarget();
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
                if (handCopy[rand].value == 1 && Random.Range(0, 1) == 1) // 1/2 chance to add 11 instead of 1 for aces
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

    public void flipAllCards()
    {
        foreach (GameObject obj in handObjects)
        {
            if (!obj.GetComponent<CardDisplay>().isFlipped)
            {
                obj.GetComponent<CardDisplay>().flip();
            }
        }
        if (!guessIsCorrect)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }

    public void play()
    {
        StartCoroutine(playCards());
    }

    public IEnumerator playCards()
    {
        // Compare player's guess to target
        if (playerGuess == target)
        {
            guessIsCorrect = true;
        }

        if (!guessIsCorrect) // Accounts for aces
        {
            for (int i = 1; i <= aceCount; i++)
            {
                if (playerGuess + 10 * i == target)
                {
                    guessIsCorrect = true;
                }
            }
            aceCount = 0;
        }

        if (guessIsCorrect)
        {
            Debug.Log("You winned :)");
            CardDisplay.letPlayerFlipGlobal = false;
            button.GetComponent<Button>().interactable = false;
            yield return new WaitForSeconds(1);
            foreach (GameObject obj in playedCards)
            {
                hand.Remove(obj.GetComponent<CardDisplay>().card);
                handObjects.Remove(obj);
                obj.SetActive(false);
            }
            foreach (GameObject obj in playedCards)
            {
                StartCoroutine(drawCard(obj.transform.position));
                Destroy(obj);
                yield return new WaitForSeconds(0.1f);
            }
            Invoke("flipAllCards", 3);
            Invoke("SetTarget", 3);
        } else
        {
            Debug.Log("wronged >:(");
        }
    }

    private IEnumerator drawCard(Vector3 position)
    {
        var card = Instantiate(cardPrefab, deckPrefab.transform.position, Quaternion.identity);
        card.GetComponent<CardDisplay>().card = deck[Random.Range(0, deck.Count)];
        hand.Add(card.GetComponent<CardDisplay>().card);
        handObjects.Add(card);
        while (Vector2.Distance(card.transform.position, position) > 0.01f)
        {
            card.transform.position += (position - card.transform.position) / 100;
            yield return new WaitForSeconds(0.001f);
        }
    }
}
