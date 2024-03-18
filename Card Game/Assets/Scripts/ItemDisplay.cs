using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField] private List<Item> items = new List<Item>();
    private Item item;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private Button itemButton;
    [SerializeField] public GameObject soldOutOverlay;
    private GameManager gameManager;

    public int price;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        item = items[Random.Range(0, items.Count)];
        itemButton.onClick.AddListener(() => { gameManager.GainItem(gameObject, item); });
        itemIcon.sprite = item.Icon;
        itemNameText.text = item.Name;
        itemDescriptionText.text = item.Description;
        UpdatePrice();
    }

    public void UpdatePrice()
    {
        price = item.Price + item.Price * Mathf.CeilToInt((gameManager.handObjects.Count - 3) / 10);
        if (price < 0)
        {
            itemPriceText.text = "-$" + Mathf.Abs(price);
        }
        else
        {
            itemPriceText.text = "$" + price;
        }
        if (price <= GameManager.money)
        {
            itemPriceText.color = Color.green;
        }
        else
        {
            itemPriceText.color = Color.red;
        }
    }
}
