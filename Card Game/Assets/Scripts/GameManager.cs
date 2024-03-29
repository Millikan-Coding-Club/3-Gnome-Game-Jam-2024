using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public List<Card> deck = new List<Card>();
    private List<Card> fullDeck = new List<Card>();
    private List<Card> hand = new List<Card>();
    public List<GameObject> handObjects = new List<GameObject>();
    static public List<GameObject> playedCards = new List<GameObject>();
    private List<Transform> cardSpawns = new List<Transform>();
    public List<Item> playerItems = new List<Item>();
    private List<GameObject> playerItemObjects = new List<GameObject>();
    [SerializeField] private Transform cardSpawnPrefab;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject playCardsButton;
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private Button leaveButton;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private TMP_Text handSizeText1;
    [SerializeField] private TMP_Text handSizeText2;
    [SerializeField] private GameObject itemBarPanel;
    [SerializeField] private GameObject itemIndicatorPrefab;
    [SerializeField] private Button emptyButton;
    private List<GameObject> activeItems = new List<GameObject>();
    public Card joker_red;
    public Card joker_black;
    public GameObject deckPrefab;
    public AudioSource drawAudio;
    public AudioSource wrongAudio;
    public AudioSource correctAudio;

    [SerializeField] private int startingCardAmount = 3;
    static public int target = 0;
    static public int money = 0;
    private int targetCardCount;
    static public int playerGuess = 0;
    static public int aceCount = 0;
    static public bool guessIsCorrect = false;
    private int wrongGuess;
    private int threshold = 20;
    private int streak = 0;
    public int shieldCount = 0;

    private void Awake()
    {
        while (deck.Count > 52)
        {
            deck.RemoveAt(deck.Count - 1);
        }
        handObjects.Clear();
        streak = 0;
        money = 0;
        hand.Clear();
        shieldCount = 0;
        aceCount = 0;
        activeItems.Clear();
        cardSpawns.Clear();
        playerItems.Clear();
        playerItemObjects.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        fullDeck = new List<Card>(deck);
        StartCoroutine(setUp(1));
        Invoke("flipAllCards", 3);
    }

    private IEnumerator setUp(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(drawCards(startingCardAmount));
        SetTarget();
    }

    private void AlignCards()
    {
        float leftMostPosition = 20f / cardSpawns.Count - 9f;
        float cardSpacing = 2f / Mathf.Max(1f, cardSpawns.Count - 1f) * -leftMostPosition;
        for (int i = 0; i < cardSpawns.Count; i++)
        {
            if (cardSpawns.Count > 1)
            {
                cardSpawns[i].position = new Vector3(leftMostPosition + cardSpacing * i, -2, 0);
            }
            else
            {
                cardSpawns[i].position = new Vector3(0, -2, 0);
            }
        }
    }

    private IEnumerator drawCards(int amount)
    {
        if (amount < 0)
        {
            for (int i = amount; i > 0; i--)
            {
                Destroy(cardSpawns[cardSpawns.Count - 1].parent);
                cardSpawns.RemoveAt(cardSpawns.Count - 1);
                playedCards.Remove(handObjects[handObjects.Count - 1]);
                hand.RemoveAt(hand.Count - 1);
                Destroy(handObjects[handObjects.Count - 1]);
                handObjects.RemoveAt(handObjects.Count - 1);
            }
            AlignCards();
        } else
        {
            for (int i = 0; i < amount; i++) // Create card spawns
            {
                cardSpawns.Add(Instantiate(cardSpawnPrefab, transform.position, Quaternion.identity));
            }
            AlignCards();
            for (int i = 0; i < amount; i++) // Create and assign cards
            {
                var card = Instantiate(cardPrefab, (Vector2)deckPrefab.transform.position, Quaternion.identity);
                int randomCardIndex = Random.Range(0, deck.Count);
                card.GetComponent<CardMovement>().target = cardSpawns[cardSpawns.Count - 1 - i];
                card.GetComponent<CardDisplay>().card = deck[randomCardIndex];
                hand.Add(card.GetComponent<CardDisplay>().card);
                handSizeText1.text = hand.Count.ToString();
                handSizeText2.text = hand.Count.ToString();
                handObjects.Add(card);
                deck.RemoveAt(randomCardIndex);
                if (deck.Count <= 0)
                {
                    deck = new List<Card>(fullDeck);
                }
                drawAudio.Play();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void SetTarget()
    {
        target = 0;
        targetCardCount = Random.Range(1, hand.Count);
        List<Card> handCopy = new List<Card>(hand);
        foreach (Card card in hand)
        {
            if (targetCardCount >= 1)
            {
                int rand = Random.Range(0, handCopy.Count);
                if (handCopy[rand].value == 1 && Random.Range(0, 1) == 1) // 1/2 chance to add 11 instead of 1 for aces
                {
                    target += 11;
                } else if (handCopy[rand].value == 0) // Card is a joker. Choose another card to add
                {
                    targetCardCount++;
                } else
                {
                    target += handCopy[rand].value;
                }
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
                obj.GetComponent<CardDisplay>().flip(false);
            }
        }
        if (!guessIsCorrect)
        {
            playCardsButton.GetComponent<Button>().interactable = true;
            CardDisplay.letPlayerFlipGlobal = true;
        }
    }

    private void startRound(bool setTarget)
    {
        leaveButton.interactable = true;
        if (setTarget) { SetTarget(); }
        playerGuess = 0;
        wrongGuess = 0;
        aceCount = 0;
        playedCards.Clear();
        guessIsCorrect = false;
        Invoke("flipAllCards", 3);
    }

    private IEnumerator endRound()
    {
        if (money > threshold)
        {
            OpenShop();
        }
        while (money > threshold)
        {
            StartCoroutine(drawCards(1));
            streakText.text = streak.ToString();
            threshold += 20 * Mathf.RoundToInt(Mathf.Pow(hand.Count - startingCardAmount + 1, 2));
        }
        CardDisplay.letPlayerFlipGlobal = false;
        playCardsButton.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(1);

        foreach (GameObject obj in playedCards)
        {
            hand.Remove(obj.GetComponent<CardDisplay>().card);
            handObjects.Remove(obj);
            obj.SetActive(false);
        }
        foreach (GameObject obj in playedCards)
        {
            var card = Instantiate(cardPrefab, (Vector2)deckPrefab.transform.position, Quaternion.identity);
            int randomCardIndex = Random.Range(0, deck.Count);
            card.GetComponent<CardDisplay>().card = deck[randomCardIndex];
            card.GetComponent<CardMovement>().target = obj.GetComponent<CardMovement>().target;
            hand.Add(card.GetComponent<CardDisplay>().card);
            handSizeText1.text = hand.Count.ToString();
            handSizeText2.text = hand.Count.ToString();
            handObjects.Add(card);
            deck.RemoveAt(randomCardIndex);
            if (deck.Count <= 0)
            {
                deck = new List<Card>(fullDeck);
            }
            drawAudio.Play();
            Destroy(obj);
            yield return new WaitForSeconds(0.1f);
        }
        while (shopCanvas.activeSelf)
        {
            yield return null;
        }
        startRound(true);
    }

    private void OpenShop()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity, itemsPanel.transform);
            activeItems.Add(newItem);
        }
        shopCanvas.SetActive(true);
    }

    public void GainItem(GameObject button, Item item)
    {
        correctAudio.Play();
        emptyButton.Select();
        if (item.Price <= money)
        {
            button.GetComponent<Button>().interactable = false;
            button.GetComponent<ItemDisplay>().soldOutOverlay.SetActive(true);
            updateMoney(-button.GetComponent<ItemDisplay>().price);
            foreach (GameObject obj in activeItems)
            {
                obj.GetComponent<ItemDisplay>().UpdatePrice();
            }
            switch (item.Name)
            {
                case "Discard":
                    StartCoroutine(drawCards(-1));
                    break;
                case "Streak Increase":
                    streak += hand.Count - 1;
                    streakText.text = streak.ToString();
                    break;
                case "Draw Card":
                    StartCoroutine(drawCards(1));
                    break;
                case "Joker":
                    if (Random.Range(0, 2) == 0)
                    {
                        deck.Add(joker_red);
                        fullDeck.Add(joker_red);
                    }
                    else
                    {
                        deck.Add(joker_black);
                        fullDeck.Add(joker_black);
                    }
                    break;
                case "Reroll":
                    AddItemToItemBar(item);
                    break;
                case "Reveal Hand":
                    AddItemToItemBar(item);
                    break;
                case "Shield":
                    AddItemToItemBar(item);
                    break;
            }
        }
    }

    private void AddItemToItemBar(Item item)
    {
        if (playerItems.Contains(item))
        {
            playerItemObjects[playerItems.IndexOf(item)].GetComponent<ItemIndicator>().UpdateAmount(1);
        } else
        {
            playerItems.Add(item);
            GameObject newItem = Instantiate(itemIndicatorPrefab, transform.position, Quaternion.identity, itemBarPanel.transform);
            playerItemObjects.Add(newItem);
            newItem.GetComponent<ItemIndicator>().item = item;
            if (item.Name == "Shield")
            {
                newItem.tag = "Shield";
            }
        }
    }

    public void CloseShop()
    {
        shopCanvas.SetActive(false);
        foreach (GameObject obj in activeItems)
        {
            Destroy(obj);
        }
        activeItems.Clear();
        Time.timeScale = 1f;
    }

    public void playCards()
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
        }

        if (guessIsCorrect)
        {
            correctAudio.pitch = 1 + (Mathf.Max(1, streak) - 1) * 0.01f;
            correctAudio.Play();
            updateMoney(hand.Count + hand.Count * streak);
            StartCoroutine(endRound());
            streak++;
            streakText.text = streak.ToString();
        } else if (playerGuess != wrongGuess)
        {
            if (shieldCount == 0)
            {
                wrongAudio.Play();
                wrongGuess = playerGuess;
                updateMoney(-hand.Count - hand.Count * streak);
                streak = 0;
                streakText.text = streak.ToString();
                startRound(false);
            } else
            {
                startRound(false);
                shieldCount--;
                GameObject.FindGameObjectWithTag("Shield").GetComponent<ItemIndicator>().UpdateAmount(-1);
            }
        }
    }

    private void updateMoney(int amount)
    {
        money += amount;
        moneyText.text = "$" + Mathf.Abs(money).ToString();
        if (money < 0)
        {
            moneyText.text = moneyText.text.PadLeft(moneyText.text.Length + 1, '-');
            moneyText.color = Color.red;
        }
        else if (money == 0)
        {
            moneyText.color = Color.white;
        } else
        {
            moneyText.color = Color.green;
        }
    }

    public void endGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
