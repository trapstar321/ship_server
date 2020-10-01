using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Item helmet;
    public Item boots;
    public Item hands;
    public Item top;
    public Item legs;

    public void Add(Item item)
    {
        switch (item.item_type)
        {
            case "helmet":
                helmet = item;
                return;
            case "boots":
                boots = item;
                return;
            case "legs":
                legs = item;
                return;
            case "hands":
                hands = item;
                return;
            case "top":
                top = item;
                return;            
        }
    }

    public void Remove(Item item)
    {
        switch (item.item_type)
        {
            case "helmet":
                helmet = null;
                return;
            case "boots":
                boots = null;
                return;
            case "legs":
                legs = null;
                return;
            case "hands":
                hands = null;
                return;
            case "top":
                top = null;
                return;            
        }
    }

    public Item GetItem(string type)
    {
        switch (type)
        {
            case "helmet":
                return helmet;
            case "boots":
                return boots;
            case "legs":
                return legs;
            case "hands":
                return hands;
            case "top":
                return top;            
        }
        return null;
    }
}
