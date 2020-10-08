using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsTable : MonoBehaviour
{
    private Transform container;
    private Transform template;
    public GameObject itemDialog;
        
    private void Awake()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform transform in children)
            if (transform.name.Equals("itemsEntryContainer"))
                container = transform;

        template = container.Find("itemsEntryTemplate");
        template.gameObject.SetActive(false);        
    }

    public void Reload() {
        Mysql mysql = FindObjectOfType<Mysql>();
        List<Item> items = mysql.ReadItems();

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

            int item_id = items[i].item_id;
            ItemDialog itemDialogScript = itemDialog.GetComponent<ItemDialog>();

            entryTransform.Find("idText").GetComponent<Text>().text = items[i].item_id.ToString();
            entryTransform.Find("nameText").GetComponent<Text>().text = items[i].name;
            entryTransform.Find("iconNameText").GetComponent<Text>().text = items[i].iconName;
            entryTransform.Find("itemTypeText").GetComponent<Text>().text = items[i].item_type;

            entryTransform.Find("attackText").GetComponent<Text>().text = items[i].attack.ToString();
            entryTransform.Find("healthText").GetComponent<Text>().text = items[i].health.ToString();
            entryTransform.Find("defenceText").GetComponent<Text>().text = items[i].defence.ToString();
            entryTransform.Find("rotationText").GetComponent<Text>().text = items[i].rotation.ToString();
            entryTransform.Find("speedText").GetComponent<Text>().text = items[i].speed.ToString();
            entryTransform.Find("visibilityText").GetComponent<Text>().text = items[i].visibility.ToString();
            entryTransform.Find("cannonReloadSpeedText").GetComponent<Text>().text = items[i].cannon_reload_speed.ToString();
            entryTransform.Find("critChanceText").GetComponent<Text>().text = items[i].crit_chance.ToString();

            Button editButton = entryTransform.Find("editButton").GetComponent<Button>();
            editButton.onClick.AddListener(() => itemDialogScript.EditItem(item_id));
        }
    }

    public void Action() { 
    
    }
}
