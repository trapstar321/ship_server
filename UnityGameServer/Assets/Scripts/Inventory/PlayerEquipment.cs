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

    private Player player;

    private void Awake()
    {
        player = transform.gameObject.GetComponent<Player>();
    }

    public void Add(Item item)
    {
        player.AddEquipment(item);
        switch (item.item_type)
        {
            case "helmet":
                if (helmet != null)
                {
                    player.RemoveEquipment(helmet);
                }
                helmet = item;
                return;
            case "boots":
                if (boots != null)
                {
                    player.RemoveEquipment(boots);
                }
                boots = item;
                return;
            case "legs":
                if (legs != null)
                {
                    player.RemoveEquipment(legs);
                }
                legs = item;
                return;
            case "hands":
                if (hands != null)
                {
                    player.RemoveEquipment(hands);
                }
                hands = item;
                return;
            case "top":
                if (top != null)
                {
                    player.RemoveEquipment(top);
                }
                top = item;
                return;            
        }
    }

    public void Remove(Item item)
    {
        player.RemoveEquipment(item);
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
