using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject itemsTable;
    public GameObject inventoryTable;
    public GameObject playerItemsTable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ItemsButtonClick() {        
        itemsTable.SetActive(!itemsTable.activeSelf);
        ItemsTable script = itemsTable.GetComponent<ItemsTable>();
        script.Reload();        
    }

    public void InventoryButtonClick() {        
        inventoryTable.SetActive(!inventoryTable.activeSelf);
        InventoryTable script = inventoryTable.GetComponent<InventoryTable>();
        script.Reload();        
    }

    public void PlayerItemsButtonClick()
    {
        playerItemsTable.SetActive(!playerItemsTable.activeSelf);
        PlayerItemsTable script = playerItemsTable.GetComponent<PlayerItemsTable>();
        script.Reload();
    }
}
