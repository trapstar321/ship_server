using System;
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
        transform.Find("nameInput").GetComponent<InputField>().text = "";
        transform.Find("iconNameInput").GetComponent<InputField>().text = "";
        transform.Find("itemTypeInput").GetComponent<InputField>().text = "";
        transform.Find("attackInput").GetComponent<InputField>().text = "";
        transform.Find("healthInput").GetComponent<InputField>().text = "";
        transform.Find("defenceInput").GetComponent<InputField>().text = "";
        transform.Find("rotationInput").GetComponent<InputField>().text = "";
        transform.Find("speedInput").GetComponent<InputField>().text = "";
        transform.Find("visibilityInput").GetComponent<InputField>().text = "";
        transform.Find("cannonReloadSpeedInput").GetComponent<InputField>().text = "";
        transform.Find("critChanceInput").GetComponent<InputField>().text = "";
        transform.gameObject.SetActive(true);
    }

    public void EditItem(int id)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        item = mysql.ReadItem(id);

        transform.Find("nameInput").GetComponent<InputField>().text = item.name;
        transform.Find("iconNameInput").GetComponent<InputField>().text = item.iconName;
        transform.Find("itemTypeInput").GetComponent<InputField>().text = item.item_type;
        transform.Find("attackInput").GetComponent<InputField>().text = item.attack.ToString();
        transform.Find("healthInput").GetComponent<InputField>().text = item.health.ToString();
        transform.Find("defenceInput").GetComponent<InputField>().text = item.defence.ToString();
        transform.Find("rotationInput").GetComponent<InputField>().text = item.rotation.ToString();
        transform.Find("speedInput").GetComponent<InputField>().text = item.speed.ToString();
        transform.Find("visibilityInput").GetComponent<InputField>().text = item.visibility.ToString();
        transform.Find("cannonReloadSpeedInput").GetComponent<InputField>().text = item.cannon_reload_speed.ToString();
        transform.Find("critChanceInput").GetComponent<InputField>().text = item.crit_chance.ToString();

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
        int attack, health, defence, rotation, speed, visibility, cannonReloadSpeed, critChance;
        Int32.TryParse(transform.Find("attackInput").GetComponent<InputField>().text, out attack);
        Int32.TryParse(transform.Find("healthInput").GetComponent<InputField>().text, out health);
        Int32.TryParse(transform.Find("defenceInput").GetComponent<InputField>().text, out defence);
        Int32.TryParse(transform.Find("rotationInput").GetComponent<InputField>().text, out rotation);
        Int32.TryParse(transform.Find("speedInput").GetComponent<InputField>().text, out speed);
        Int32.TryParse(transform.Find("visibilityInput").GetComponent<InputField>().text, out visibility);
        Int32.TryParse(transform.Find("cannonReloadSpeedInput").GetComponent<InputField>().text, out cannonReloadSpeed);
        Int32.TryParse(transform.Find("critChanceInput").GetComponent<InputField>().text, out critChance);

        Mysql mysql = FindObjectOfType<Mysql>();

        Item it = new Item();
        it.name = name;
        it.iconName = iconName;
        it.item_type = itemType;
        it.attack = attack;
        it.health = health;
        it.defence = defence;
        it.rotation = rotation;
        it.speed = speed;
        it.visibility = visibility;
        it.cannon_reload_speed = cannonReloadSpeed;
        it.crit_chance = critChance;

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
