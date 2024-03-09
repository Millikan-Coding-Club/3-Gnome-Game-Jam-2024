using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public List<Card> deck = new List<Card>();
    public Transform[] cardSpawns;

    private GameObject card;
    // Start is called before the first frame update
    void Start()
    {
        card = Instantiate(cardPrefab, cardSpawns[0].transform.position, cardSpawns[0].transform.rotation);
        card.GetComponent<CardDisplay>().card = deck[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
