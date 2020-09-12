﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot:MonoBehaviour
{    
    public Image icon;
    public Item item;
    public int slotID;
    public int quantity;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public override string ToString()
    {
        return $"name={item.name}, quantity={quantity}";
    }
}
