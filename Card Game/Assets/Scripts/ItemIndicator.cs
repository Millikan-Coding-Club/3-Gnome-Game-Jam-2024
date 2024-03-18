using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ItemIndicator : MonoBehaviour
{
    public Item item;
    public GameObject amountObject;
    private int amount = 1;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject nameText;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Awake()
    {
        StartCoroutine(testItem());
    }

    private IEnumerator testItem()
    {
        while (item == null)
        {
            yield return null;
        }
        icon.sprite = item.Icon;
        nameText.GetComponent<TMP_Text>().text = item.Name;
        if (gameObject.tag == "Shield")
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void UseItem()
    {
        switch (item.Name)
        {
            case "Reroll":
                gameManager.SetTarget();
                break;
            case "Reveal Hand":
                foreach (GameObject obj in gameManager.handObjects)
                {
                    if (obj.GetComponent<CardDisplay>().isFlipped)
                    {
                        obj.GetComponent<CardDisplay>().flip(false);
                    }
                }
                gameManager.Invoke("flipAllCards", 3);
                break;
            case "Shield":
                gameManager.shieldCount++;
                break;
        }
        amount--;
        if (amount <= 1)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateAmount(int count)
    {
        amount += count;
        if (amount > 1)
        {
            gameManager.playerItems.Add(item);
            amountObject.SetActive(true);
        } else if (count == 0)
        {
            gameManager.playerItems.Remove(item);
            amountObject.SetActive(false);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        nameText.SetActive(true);
    }

    private void OnMouseExit()
    {
        nameText.SetActive(false);
    }
}
