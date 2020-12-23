using System.Collections;
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
        bool found = false;
        if (!item.isDefaultItem)
        {
            if (item.stackable)
            {
                foreach (InventorySlot slot in items)
                {
                    if (slot.item != null && item.item_id == slot.item.item_id)
                    {
                        found = true;
                        slot.quantity += quantity;
                        s = slot;
                        break;
                    }
                }
            }

            foreach (InventorySlot slot in items) {
                if (!found && item.stackable){                    
                    if(slot.item==null) {
                        slot.item = item;
                        slot.quantity = quantity;
                        s = slot;
                        break;
                    }
                }
                else if(!item.stackable){
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

    public void RemoveAmount(int slotID, int amount) {
        for (int i = 0; i < items.Count; i++)
        {
            if (slotID == items[i].slotID)
            {
                if (items[i].quantity-amount == 0)
                {
                    items[i].ClearSlot();
                }
                else {
                    items[i].quantity -= amount;
                }
            }
        }
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
