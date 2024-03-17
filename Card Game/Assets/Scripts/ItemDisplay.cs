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
    [SerializeField] public GameManager gameManager;

    void Awake()
    {
        item = items[Random.Range(0, items.Count)];
        itemButton.onClick.AddListener(() => { gameManager.GainItem(gameObject, item); });
        itemIcon.sprite = item.Icon;
        itemNameText.text = item.Name;
        itemDescriptionText.text = item.Description;
        if (item.Price < 0)
        {
            itemPriceText.text = "-$" + Mathf.Abs(item.Price);
        } else
        {
            itemPriceText.text = "$" + item.Price;
        }
        if (item.Price <= GameManager.money)
        {
            itemPriceText.color = Color.green;
        } else
        {
            itemPriceText.color = Color.red;
        }
    }
}
