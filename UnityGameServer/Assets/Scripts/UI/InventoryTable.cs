using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTable : MonoBehaviour
{
    private Transform container;
    private Transform template;
    public GameObject inventoryDialog;    
    Mysql mysql;
    int player_id;

    private void Awake()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform transform in children)
            if (transform.name.Equals("itemsEntryContainer"))
                container = transform;

        template = container.Find("itemsEntryTemplate");
        template.gameObject.SetActive(false);        
    }

    public void Reload(int playerID=0)
    {
        mysql = FindObjectOfType<Mysql>();
        players = mysql.GetPlayers();

        if(playerID==0)
            LoadPlayerDropdown(players);

        if (playerID == 0)
        {            
            playerID = players[0].id;            
        }

        player_id = playerID;

        InventoryDialog inventoryDialogScript = inventoryDialog.GetComponent<InventoryDialog>();
        Button addButton = transform.Find("addButton").GetComponent<Button>();
        addButton.onClick.RemoveAllListeners();
        addButton.onClick.AddListener(() => inventoryDialogScript.AddInventory(player_id));

        List<InventorySlot> slots = mysql.ReadInventory(playerID);

        for (int i = 0; i < container.transform.childCount; i++)
        {
            Transform child = container.transform.GetChild(i);
            if (child.transform.name != "itemsEntryTemplate")
            {
                Destroy(child.gameObject);
            }
        }

        float templateHeight = 20f;
        for (int i = 0; i < slots.Count; i++)
        {
            Transform entryTransform = Instantiate(template, container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i - 10);
            entryTransform.gameObject.SetActive(true);

            int slot_id = slots[i].id;            

            entryTransform.Find("slotText").GetComponent<Text>().text = slots[i].slotID.ToString();
            entryTransform.Find("itemText").GetComponent<Text>().text = slots[i].item.name;
            entryTransform.Find("itemTypeText").GetComponent<Text>().text = slots[i].item.item_type;
            entryTransform.Find("quantityText").GetComponent<Text>().text = slots[i].quantity.ToString();            

            Button editButton = entryTransform.Find("editButton").GetComponent<Button>();
            editButton.onClick.AddListener(() => inventoryDialogScript.EditInventory(slot_id, player_id));
        }
    }

    public void Action()
    {

    }

    private List<Player> players;

    private void LoadPlayerDropdown(List<Player> players) {
        Dropdown playerDropdown = transform.Find("playerDropdown").gameObject.GetComponent<Dropdown>();
        List<string> options = new List<string>();
        foreach (Player player in players) {
            options.Add(player.username);
        }
        playerDropdown.ClearOptions();
        playerDropdown.AddOptions(options);
        this.players = players;
    }

    public void PlayerChanged(int index) {
        Player player = players.ElementAt(index);
        Reload(player.id);
    }
}
