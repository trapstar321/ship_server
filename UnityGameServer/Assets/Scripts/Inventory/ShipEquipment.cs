using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEquipment : MonoBehaviour
{
    public Item bow_sprite;
    public Item jib_sail;
    public Item fore_sail;
    public Item rudder;
    public Item keel;
    public Item hull;
    public Item main_mast;
    public Item rigging;
    public Item captains_cabin;
    public Item main_sail;
    public Item crows_nest;

    public void Add(Item item)
    {
        switch (item.item_type)
        {
            case "bow_sprite":
                bow_sprite = item;
                return;
            case "jib_sail":
                jib_sail = item;
                return;
            case "fore_sail":
                fore_sail = item;
                return;
            case "rudder":
                rudder = item;
                return;
            case "keel":
                keel = item;
                return;
            case "hull":
                hull = item;
                return;
            case "main_mast":
                main_mast = item;
                return;
            case "rigging":
                rigging = item;
                return;
            case "captains_cabin":
                captains_cabin = item;
                return;
            case "main_sail":
                main_sail = item;
                return;
            case "crows_nest":
                crows_nest = item;
                return;
        }
    }

    public void Remove(Item item)
    {
        switch (item.item_type)
        {
            case "bow_sprite":
                bow_sprite = null;
                return;
            case "jib_sail":
                jib_sail = null;
                return;
            case "fore_sail":
                fore_sail = null;
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
            case "captains_cabin":
                captains_cabin = null;
                return;
            case "main_sail":
                main_sail = null;
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
            case "bow_sprite":
                return bow_sprite;                
            case "jib_sail":
                return jib_sail;                
            case "fore_sail":
                return fore_sail;                
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
            case "captains_cabin":
                return captains_cabin;                
            case "main_sail":
                return main_sail;                
            case "crows_nest":
                return crows_nest;                
        }
        return null;
    }
}
