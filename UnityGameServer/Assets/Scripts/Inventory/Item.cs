using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Name", menuName = "Inventory/Item")]
public class Item : ScriptableObject,IComparable
{
    public int id;
    public int item_id;
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public string iconName;
    public string item_type;

    public int attack;
    public int defence;
    public int health;
    public int max_health;
    public int rotation;
    public int speed;
    public int visibility;
    public int cannon_reload_speed;
    public int crit_chance;
    public int cannon_force;
    public bool stackable;
    public int energy;
    public int max_energy;
    public bool overtime;
    public float buff_duration;
    public float cooldown;
    public DateTime buff_start;

    public int dropChance;
    public float maxLootQuantity;

    public int CompareTo(object obj)
    {
        Item item = (Item)obj;

        if (item.dropChance < dropChance)
        {
            return -1;
        }
        else if (item.dropChance > dropChance) {
            return 1;
        }
        return 0;
    }
}
