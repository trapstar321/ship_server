using MySql.Data.MySqlClient;
using System;
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
        string sql = @"SELECT b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item 
                        FROM inventory a 
                        inner join inventory_slot as b 
                        on a.slot_id=b.id 
                        inner join player_item as c 
                        on b.item_id=c.id 
                        and a.PLAYER_ID=c.PLAYER_ID
                        inner join item as d
                        on c.item_id=d.id
                        where a.player_id=" + playerID.ToString();
        
        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<InventorySlot> slots = new List<InventorySlot>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            int item_id = rdr.GetInt32("ITEM_ID");
            int slot_id = rdr.GetInt32("SLOT_ID");
            int quantity = rdr.GetInt32("QUANTITY");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");

            Item item = new Item();            
            item.id = id;
            item.item_id = id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;

            InventorySlot slot = new InventorySlot() { slotID = slot_id, quantity = quantity, item = item };
            slots.Add(slot);
        }
        rdr.Close();
        return slots;
    }

    public void DropItem(int player, InventorySlot slot) {
        string sql = @"select b.id from inventory as a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        where a.player_id=@player_id and b.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", slot.slotID);

        MySqlDataReader reader = cmd.ExecuteReader();

        int slot_id = 0;

        while (reader.Read())
        {
            slot_id = reader.GetInt32("id");
        }

        reader.Close();

        sql = "select item_id from inventory_slot where id=@slot_id";        
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id);
        reader = cmd.ExecuteReader();

        int item_id = 0;

        while (reader.Read())
        {
            item_id = reader.GetInt32("item_id");
        }

        Debug.Log("Slot_ID=" + slot_id);
        Debug.Log("Item_ID=" + item_id);

        reader.Close();

        sql = @"delete from inventory
                where slot_id = @slot_id and player_id = @player_id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id);
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.ExecuteNonQuery();

        sql = "delete from inventory_slot where id=@slot_id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id);
        cmd.ExecuteNonQuery();

        sql = "delete from player_item where id=@item_id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@item_id", item_id);
        cmd.ExecuteNonQuery();
    }

    public void DragAndDrop_Move(int player, InventorySlot slot1, InventorySlot slot2)
    {
        InventorySlot add = null;
        InventorySlot remove = null;

        add = slot1.item == null ? slot1 : slot2;
        remove = slot1.item != null ? slot1 : slot2;

        string sql = @"select b.id from inventory as a
                            inner join inventory_slot as b
                            on a.slot_id=b.id
                            where a.player_id=@player_id and b.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", remove.slotID);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");            
        }

        reader.Close();

        cmd.CommandText = "delete from inventory where player_id=@player_id and slot_id=@slot_id";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", id);
        cmd.ExecuteNonQuery();

        cmd.CommandText = "delete from inventory_slot where id=@id";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();

        cmd.CommandText = "insert into inventory_slot(item_id, slot_id, quantity)values(@item_id, @slot_id, @quantity);select last_insert_id();";
        cmd.Parameters.AddWithValue("@item_id", remove.item.id);
        cmd.Parameters.AddWithValue("@slot_id", add.slotID);
        cmd.Parameters.AddWithValue("@quantity", remove.quantity);
        int new_id = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText = "insert into inventory(player_id, slot_id)values(@player_id, @slot_id)";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", new_id);
        cmd.ExecuteNonQuery();
    }

    public void DragAndDrop_Change(int player, InventorySlot slot1, InventorySlot slot2) {
        string sql = @"select b.id, b.slot_id from inventory as a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        where a.player_id=@player_id and b.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", slot1.slotID);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id_1 = 0;
        int slot_id_1 = 0;

        while (reader.Read())
        {
            id_1 = reader.GetInt32("id");
            slot_id_1 = reader.GetInt32("slot_id");
        }

        reader.Close();

        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", slot2.slotID);
        reader = cmd.ExecuteReader();

        int id_2=0;
        int slot_id_2=0;

        while (reader.Read())
        {
            id_2 = reader.GetInt32("id");
            slot_id_2 = reader.GetInt32("slot_id");
        }

        reader.Close();

        sql = "update inventory_slot set slot_id=@slot_id where id=@id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id_2);
        cmd.Parameters.AddWithValue("@id", id_1);
        cmd.ExecuteNonQuery();
        
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id_1);
        cmd.Parameters.AddWithValue("@id", id_2);
        cmd.ExecuteNonQuery();        
    }
}
