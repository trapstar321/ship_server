using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemsTable : MonoBehaviour
{
    private Transform container;
    private Transform template;
    public GameObject playerItemsDialog;
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

    public void Reload(int playerID = 0)
    {
        mysql = FindObjectOfType<Mysql>();
        players = mysql.GetPlayers();

        if (playerID == 0)
            LoadPlayerDropdown(players);

        if (playerID == 0)
        {
            playerID = players[0].id;
        }

        player_id = playerID;

        PlayerItemsDialog playerItemsDialogScript = playerItemsDialog.GetComponent<PlayerItemsDialog>();
        Button addButton = transform.Find("addButton").GetComponent<Button>();
        addButton.onClick.RemoveAllListeners();
        addButton.onClick.AddListener(() => playerItemsDialogScript.AddPlayerItem(player_id));

        List<Item> items = mysql.ReadPlayerItems(playerID);

        for (int i = 0; i < container.transform.childCount; i++)
        {
            Transform child = container.transform.GetChild(i);
            if (child.transform.name != "itemsEntryTemplate")
            {
                Destroy(child.gameObject);
            }
        }

        float templateHeight = 20f;
        for (int i = 0; i < items.Count; i++)
        {
            Transform entryTransform = Instantiate(template, container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i - 10);
            entryTransform.gameObject.SetActive(true);            

            entryTransform.Find("idText").GetComponent<Text>().text = items[i].id.ToString();
            entryTransform.Find("itemText").GetComponent<Text>().text = items[i].name;
            entryTransform.Find("itemTypeText").GetComponent<Text>().text = items[i].item_type;            
        }
    }

    public void Action()
    {

    }

    private List<Player> players;

    private void LoadPlayerDropdown(List<Player> players)
    {
        Dropdown playerDropdown = transform.Find("playerDropdown").gameObject.GetComponent<Dropdown>();
        List<string> options = new List<string>();
        foreach (Player player in players)
        {
            options.Add(player.data.username);
        }
        playerDropdown.ClearOptions();
        playerDropdown.AddOptions(options);
        this.players = players;
    }

    public void PlayerChanged(int index)
    {
        Player player = players.ElementAt(index);
        Reload(player.id);
    }
}
