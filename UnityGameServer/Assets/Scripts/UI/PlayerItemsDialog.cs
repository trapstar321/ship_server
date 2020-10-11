using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemsDialog : MonoBehaviour
{
    private InventorySlot slot;
    public GameObject playerItemsTable;
    List<Item> items;
    int player_id;

    // Start is called before the first frame update 
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddPlayerItem(int player_id)
    {
        this.player_id = player_id;
        Mysql mysql = FindObjectOfType<Mysql>();
        items = mysql.ReadItems();        
        LoadItemsDropdown();
        transform.gameObject.SetActive(true);
    }

    public void Cancel()
    {
        slot = null;
        transform.gameObject.SetActive(false);
    }

    public void SavePlayerItem()
    {        
        Dropdown itemDropdown = transform.Find("itemInput").GetComponent<Dropdown>();
        Item it = items.ElementAt(itemDropdown.value);

        Mysql mysql = FindObjectOfType<Mysql>();
        mysql.AddPlayerItem(player_id, it);

        slot = null;
        transform.gameObject.SetActive(false);

        PlayerItemsTable script = playerItemsTable.GetComponent<PlayerItemsTable>();
        script.Reload(player_id);
    }

    private void LoadItemsDropdown()
    {
        Dropdown itemDropdown = transform.Find("itemInput").GetComponent<Dropdown>();
        List<string> options = new List<string>();
        foreach (Item it in items)
            options.Add(it.name);
        itemDropdown.ClearOptions();
        itemDropdown.AddOptions(options);
    }

    private int GetItemIndex(string name)
    {
        int i = 0;
        foreach (Item item in items)
        {
            if (item.name.Equals(name))
                return i;
            i++;
        }
        return i;
    }
}
