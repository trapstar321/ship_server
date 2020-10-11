using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDialog : MonoBehaviour
{
    private InventorySlot slot;
    public GameObject inventoryTable;    
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

    public void AddInventory(int player_id)
    {
        this.player_id = player_id;
        Mysql mysql = FindObjectOfType<Mysql>();
        items = mysql.ReadPlayerItems(player_id);
        transform.Find("slotInput").GetComponent<InputField>().text = "";
        transform.Find("slotInput").GetComponent<InputField>().enabled = true;
        transform.Find("quantityInput").GetComponent<InputField>().text = "";
        LoadItemsDropdown();
        transform.gameObject.SetActive(true);
    }

    public void EditInventory(int id, int player_id)
    {
        this.player_id = player_id;
        Mysql mysql = FindObjectOfType<Mysql>();
        items = mysql.ReadPlayerItems(player_id);
        slot = mysql.ReadInventorySlot(id);
        transform.Find("slotInput").GetComponent<InputField>().text = slot.slotID.ToString();
        transform.Find("slotInput").GetComponent<InputField>().enabled = false;
        transform.Find("quantityInput").GetComponent<InputField>().text = slot.quantity.ToString();
        LoadItemsDropdown();
        Dropdown itemDropdown = transform.Find("itemInput").GetComponent<Dropdown>();
        itemDropdown.value = GetItemIndex(slot.item.name);
        transform.gameObject.SetActive(true);
    }

    public void Cancel()
    {
        slot = null;
        transform.gameObject.SetActive(false);
    }

    public void SaveInventory()
    {
        int slot_id, quantity;
        Int32.TryParse(transform.Find("slotInput").GetComponent<InputField>().text, out slot_id);
        Int32.TryParse(transform.Find("quantityInput").GetComponent<InputField>().text, out quantity);
        Dropdown itemDropdown = transform.Find("itemInput").GetComponent<Dropdown>();
        Item it = items.ElementAt(itemDropdown.value);

        InventorySlot sl = new InventorySlot();
        sl.slotID = slot_id;
        sl.item = it;
        sl.quantity = quantity;

        Mysql mysql = FindObjectOfType<Mysql>();
        mysql.SaveInventorySlot(player_id, sl);

        slot = null;
        transform.gameObject.SetActive(false);

        InventoryTable script = inventoryTable.GetComponent<InventoryTable>();
        script.Reload(player_id);
    }

    private void LoadItemsDropdown() {
        Dropdown itemDropdown = transform.Find("itemInput").GetComponent<Dropdown>();
        List<string> options = new List<string>();
        foreach (Item it in items)
            options.Add(it.name);
        itemDropdown.ClearOptions();
        itemDropdown.AddOptions(options);
    }

    private int GetItemIndex(string name) {
        int i = 0;
        foreach (Item item in items) {
            if (item.name.Equals(name))
                return i;
            i++;
        }
        return i;
    }
}
