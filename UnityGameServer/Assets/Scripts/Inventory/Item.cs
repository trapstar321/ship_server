﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Name", menuName = "Inventory/Item")]
public class Item : ScriptableObject
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
    public int rotation;
    public int speed;
    public int visibility;
    public int cannon_reload_speed;
    public int crit_chance;
}
