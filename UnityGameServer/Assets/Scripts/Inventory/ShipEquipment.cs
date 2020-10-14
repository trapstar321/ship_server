using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEquipment : MonoBehaviour
{
    public Item fore_sail;
    public Item rudder;
    public Item keel;
    public Item hull;
    public Item main_mast;
    public Item rigging;
    public Item crows_nest;
    public Item cannon;

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
            case "fore_sail":
                if (fore_sail != null) {
                    player.RemoveEquipment(fore_sail);
                }
                fore_sail = item;                
                return;
            case "cannon":
                if (cannon != null)
                {
                    player.RemoveEquipment(cannon);
                }
                cannon = item;
                return;
            case "rudder":
                if (rudder != null)
                {
                    player.RemoveEquipment(rudder);
                }
                rudder = item;
                return;
            case "keel":
                if (keel != null)
                {
                    player.RemoveEquipment(keel);
                }
                keel = item;
                return;
            case "hull":
                if (hull != null)
                {
                    player.RemoveEquipment(hull);
                }
                hull = item;
                return;
            case "main_mast":
                if (main_mast != null)
                {
                    player.RemoveEquipment(main_mast);
                }
                main_mast = item;
                return;
            case "rigging":
                if (rigging != null)
                {
                    player.RemoveEquipment(rigging);
                }
                rigging = item;
                return;
            case "crows_nest":
                if (crows_nest != null)
                {
                    player.RemoveEquipment(crows_nest);
                }
                crows_nest = item;
                return;
        }
    }

    public void Remove(Item item)
    {
        player.RemoveEquipment(item);
        switch (item.item_type)
        {
            case "fore_sail":
                fore_sail = null;
                return;
            case "cannon":
                cannon = null;
                return;
            case "rudder":
                rudder = null;
                return;
            case "keel":
                keel = null;
                return;
            case "hull":
                hull = null;
                return;
            case "main_mast":
                main_mast = null;
                return;
            case "rigging":
                rigging = null;
                return;
            case "crows_nest":
                crows_nest = null;
                return;
        }
    }

    public Item GetItem(string type)
    {
        switch (type)
        {               
            case "fore_sail":
                return fore_sail;
            case "cannon":
                return cannon;
            case "rudder":
                return rudder;                
            case "keel":
                return keel;                
            case "hull":
                return hull;                
            case "main_mast":
                return main_mast;                
            case "rigging":
                return rigging;               
            case "crows_nest":
                return crows_nest;                
        }
        return null;
    }

    public List<Item> Items() {
        List<Item> items = new List<Item>();
        if (fore_sail != null) items.Add(fore_sail);
        if (cannon != null) items.Add(cannon);
        if (rudder != null) items.Add(rudder);
        if (keel != null) items.Add(keel);
        if (hull != null) items.Add(hull);
        if (main_mast != null) items.Add(main_mast);
        if (rigging != null) items.Add(rigging);
        if (crows_nest != null) items.Add(crows_nest);
        return items;
    }
}
