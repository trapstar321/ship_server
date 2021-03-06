﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputs : MonoBehaviour
{
    public GameObject itemsTable;
    public GameObject inventoryTable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) {            
            itemsTable.SetActive(!itemsTable.activeSelf);
            ItemsTable script = itemsTable.GetComponent<ItemsTable>();
            script.Reload();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            inventoryTable.SetActive(!inventoryTable.activeSelf);
            InventoryTable script = inventoryTable.GetComponent<InventoryTable>();
            script.Reload();
        }
    }
}
