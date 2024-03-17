using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIndicator : MonoBehaviour
{
    public Item item;
    [SerializeField] public GameObject amount;
    [SerializeField] private GameManager gameManager;

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
        Destroy(gameObject);
    }
}
