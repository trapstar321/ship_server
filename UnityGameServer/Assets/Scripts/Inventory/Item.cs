using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Name", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isDefaultItem = false;
    public string iconName;

}
