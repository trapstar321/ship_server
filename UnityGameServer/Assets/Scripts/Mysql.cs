using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mysql : MonoBehaviour
{
    public string server;
    public string userid;
    public string password;
    public string database;

    private MySqlConnection con;

    // Start is called before the first frame update
    private void Awake()
    {
        string cs = $"server={server};userid={userid};password={password};database={database}";

        con = new MySqlConnection(cs);
        con.Open();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<InventorySlot> ReadInventory(int playerID) {
        string sql = @"SELECT b.SLOT_ID, b.QUANTITY, c.* FROM inventory a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        inner join item as c
                        on b.item_id=c.id
                        where player_id="+playerID.ToString();
        
        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<InventorySlot> slots = new List<InventorySlot>();
        while (rdr.Read())
        {
            int slot_id = rdr.GetInt32("SLOT_ID");
            int quantity = rdr.GetInt32("QUANTITY");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");

            Item item = new Item();
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;

            InventorySlot slot = new InventorySlot() { slotID = slot_id, quantity = quantity, item = item };
            slots.Add(slot);
        }
        rdr.Close();
        return slots;
    }
}
