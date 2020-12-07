﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;    

    private void Awake()
    {
        for (int i = 0; i < space; i++)
            items.Add(new InventorySlot() { slotID=i+1});
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 80;

    public List<InventorySlot> items = new List<InventorySlot>();

    public InventorySlot Add(Item item, int quantity=1)
    {
        InventorySlot s = null;
        if (!item.isDefaultItem)
        {
            foreach (InventorySlot slot in items) {
                if (item.item_type.Equals("resource")){
                    if (slot.item != null && item.item_id == slot.item.item_id)
                    {
                        slot.quantity += quantity;
                        s = slot;
                        break;
                    }
                    else if(slot.item==null) {
                        slot.item = item;
                        slot.quantity = quantity;
                        s = slot;
                        break;
                    }
                }
                else {
                    if (slot.item == null) {
                        slot.item = item;
                        slot.quantity = quantity;
                        s = slot;
                        break;
                    }
                }
            }
            
            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return s;
    }

    public InventorySlot Add(InventorySlot slot)
    {   
        foreach (InventorySlot s in items)
        {
            if (s.slotID == slot.slotID)
            {
                s.item = slot.item;
                s.quantity = slot.quantity;
                slot = s;
                break;
            }
        }

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
        return slot;
    }

    public void Remove(int slotID)
    {
        for(int i=0; i < items.Count; i++)
        {
            if (slotID == items[i].slotID) {
                items[i].ClearSlot();
            }
        }
        
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }

    public void DragAndDrop(InventorySlot slot1, InventorySlot slot2) {
        Item item1 = slot1.item;
        Item item2 = slot2.item;

        if (slot1.item?.item_id == slot2.item?.item_id)
        {            
            slot2.quantity += slot1.quantity;
            slot1.ClearSlot();
        }
        else
        {
            int tmp_quantity = slot1.quantity;

            slot1.ClearSlot();
            if (item2 != null)
                slot1.AddItem(item2);
            slot1.quantity = slot2.quantity;

            slot2.ClearSlot();
            if (item1 != null)
                slot2.AddItem(item1);
            slot2.quantity = tmp_quantity;
        }
    }

    public InventorySlot FindSlot(int slotID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].slotID == slotID)
                return items[i];
        }
        return null;
    }
}
