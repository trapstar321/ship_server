using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot:MonoBehaviour
{
    public int id;
    public Image icon;
    public Item item;
    public int slotID;
    public int quantity;

    public void AddItem(Item newItem)
    {
        item = newItem;
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public override string ToString()
    {
        return $"name={item.name}, quantity={quantity}";
    }
}
