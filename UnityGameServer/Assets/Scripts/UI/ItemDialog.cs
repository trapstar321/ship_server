using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDialog : MonoBehaviour
{
    private Item item;
    public GameObject itemsTable;

    // Start is called before the first frame update 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem()
    {
        transform.gameObject.SetActive(true);
    }

    public void EditItem(int id)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        item = mysql.ReadItem(id);

        transform.Find("nameInput").GetComponent<InputField>().text = item.name;
        transform.Find("iconNameInput").GetComponent<InputField>().text = item.iconName;
        transform.Find("itemTypeInput").GetComponent<InputField>().text = item.item_type;

        transform.gameObject.SetActive(true);
    }

    public void Cancel() {
        item = null;
        transform.gameObject.SetActive(false);
    }

    public void SaveItem()
    {
        string name = transform.Find("nameInput").GetComponent<InputField>().text;
        string iconName = transform.Find("iconNameInput").GetComponent<InputField>().text;
        string itemType = transform.Find("itemTypeInput").GetComponent<InputField>().text;

        Mysql mysql = FindObjectOfType<Mysql>();

        Item it = new Item();
        it.name = name;
        it.iconName = iconName;
        it.item_type = itemType;

        if (item != null)
            it.item_id = item.item_id;

        if (item == null)
        {
            mysql.AddItem(it);
        }
        else {
            mysql.EditItem(it);
        }

        item = null;
        transform.gameObject.SetActive(false);

        ItemsTable script = itemsTable.GetComponent<ItemsTable>();
        script.Reload();
    }
}
