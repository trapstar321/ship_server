using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;    

    private void Awake()
    {
        Debug.Log("Inventory init");
        if (instance != null)
        {
            Debug.LogWarning("More then one instance of Inventory found!");
            return;
        }
        instance = this;

        for (int i = 0; i < space; i++)
            items.Add(new InventorySlot() { slotID=i+1});
    }
    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int space = 20;

    public List<InventorySlot> items = new List<InventorySlot>();

    public bool Add (Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= space)
            {
                Debug.Log("Not enough room");
                return false;
            }

            foreach (InventorySlot slot in items) {
                if (slot.item == null)
                    slot.item = item;
            }
            
            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        return true;
    }

    public void Remove(int slotID)
    {
        for(int i=0; i < items.Count; i++)
        {
            if (slotID == items[i].slotID) {
                items.RemoveAt(i);
            }
        }
        
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}
