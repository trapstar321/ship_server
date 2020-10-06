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

    public List<InventorySlot> ReadInventory(int playerID)
    {
        string sql = @"SELECT b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item, d.item_type,
                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED
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
            string item_type = rdr.GetString("ITEM_TYPE");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
            int attack = rdr.GetInt32("ATTACK");
            int health = rdr.GetInt32("HEALTH");
            int defence = rdr.GetInt32("DEFENCE");
            int rotation = rdr.GetInt32("ROTATION");
            int speed = rdr.GetInt32("SPEED");
            int visibility = rdr.GetInt32("VISIBILITY");
            int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");

            Item item = new Item();
            item.id = id;
            item.item_id = item_id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;
            item.item_type = item_type;
            item.attack = attack;
            item.health = health;
            item.defence = defence;
            item.rotation = rotation;
            item.speed = speed;
            item.visibility = visibility;
            item.cannon_reload_speed = cannon_reload_speed;

            InventorySlot slot = new InventorySlot() { slotID = slot_id, quantity = quantity, item = item };
            slots.Add(slot);
        }
        rdr.Close();
        return slots;
    }

    public List<Item> ReadShipEquipment(int playerID)
    {
        string sql = @"select b.id, c.id as item_id, c.name, c.icon_name, c.is_default_item, c.item_type,
                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED
                        from ship_equipment as a
                        inner join player_item as b
                        on a.item_id= b.id
                        inner join item as c
                        on b.item_id= c.id
                        where a.player_id= " + playerID.ToString();

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Item> items = new List<Item>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            int item_id = rdr.GetInt32("ITEM_ID");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            string item_type = rdr.GetString("ITEM_TYPE");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
            int attack = rdr.GetInt32("ATTACK");
            int health = rdr.GetInt32("HEALTH");
            int defence = rdr.GetInt32("DEFENCE");
            int rotation = rdr.GetInt32("ROTATION");
            int speed = rdr.GetInt32("SPEED");
            int visibility = rdr.GetInt32("VISIBILITY");
            int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");

            Item item = new Item();
            item.id = id;
            item.item_id = item_id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;
            item.item_type = item_type;
            item.attack = attack;
            item.health = health;
            item.defence = defence;
            item.rotation = rotation;
            item.speed = speed;
            item.visibility = visibility;
            item.cannon_reload_speed = cannon_reload_speed;

            items.Add(item);
        }
        rdr.Close();
        return items;
    }

    public List<Item> ReadPlayerEquipment(int playerID)
    {
        string sql = @"select b.id, c.id as item_id, c.name, c.icon_name, c.is_default_item, c.item_type,
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED
                        from player_equipment as a
                        inner join player_item as b
                        on a.item_id= b.id
                        inner join item as c
                        on b.item_id= c.id
                        where a.player_id= " + playerID.ToString();

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Item> items = new List<Item>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            int item_id = rdr.GetInt32("ITEM_ID");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            string item_type = rdr.GetString("ITEM_TYPE");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
            int attack = rdr.GetInt32("ATTACK");
            int health = rdr.GetInt32("HEALTH");
            int defence = rdr.GetInt32("DEFENCE");
            int rotation = rdr.GetInt32("ROTATION");
            int speed = rdr.GetInt32("SPEED");
            int visibility = rdr.GetInt32("VISIBILITY");
            int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");

            Item item = new Item();
            item.id = id;
            item.item_id = item_id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;
            item.item_type = item_type;
            item.attack = attack;
            item.health = health;
            item.defence = defence;
            item.rotation = rotation;
            item.speed = speed;
            item.visibility = visibility;
            item.cannon_reload_speed = cannon_reload_speed;
            items.Add(item);
        }
        rdr.Close();
        return items;
    }

    public List<Item> ReadItems()
    {
        string sql = @"select ID, NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE, 
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED
                       from item";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Item> items = new List<Item>();
        while (rdr.Read())
        {            
            int item_id = rdr.GetInt32("ID");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            string item_type = rdr.GetString("ITEM_TYPE");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
            int attack = rdr.GetInt32("ATTACK");
            int health = rdr.GetInt32("HEALTH");
            int defence = rdr.GetInt32("DEFENCE");
            int rotation = rdr.GetInt32("ROTATION");
            int speed = rdr.GetInt32("SPEED");
            int visibility = rdr.GetInt32("VISIBILITY");
            int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");

            Item item = new Item();            
            item.item_id = item_id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;
            item.item_type = item_type;
            item.attack = attack;
            item.health = health;
            item.defence = defence;
            item.rotation = rotation;
            item.speed = speed;
            item.visibility = visibility;
            item.cannon_reload_speed = cannon_reload_speed;
            items.Add(item);
        }
        rdr.Close();
        return items;
    }

    public Item ReadItem(int id)
    {
        string sql = @"select ID, NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE,
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED
                       from item where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", id);
        MySqlDataReader rdr = cmd.ExecuteReader();        

        Item item = new Item();
        while (rdr.Read())
        {
            int item_id = rdr.GetInt32("ID");
            string name = rdr.GetString("NAME");
            string icon_name = rdr.GetString("ICON_NAME");
            string item_type = rdr.GetString("ITEM_TYPE");
            bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
            int attack = rdr.GetInt32("ATTACK");
            int health = rdr.GetInt32("HEALTH");
            int defence = rdr.GetInt32("DEFENCE");
            int rotation = rdr.GetInt32("ROTATION");
            int speed = rdr.GetInt32("SPEED");
            int visibility = rdr.GetInt32("VISIBILITY");
            int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");

            item.item_id = item_id;
            item.name = name;
            item.iconName = icon_name;
            item.isDefaultItem = is_default_item;
            item.item_type = item_type;
            item.attack = attack;
            item.health = health;
            item.defence = defence;
            item.rotation = rotation;
            item.speed = speed;
            item.visibility = visibility;
            item.cannon_reload_speed = cannon_reload_speed;
        }
        rdr.Close();
        return item;
    }

    public void AddItem(Item item)
    {
        string sql = @"insert into item(NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE,
                                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED)
                       select @name, @icon_name, 0, @type, @attack, @health, @defence, @rotation, @speed, @visibility, 
                              @cannon_reload_speed";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@name", item.name);
        cmd.Parameters.AddWithValue("@icon_name", item.iconName);
        cmd.Parameters.AddWithValue("@type", item.item_type);
        cmd.Parameters.AddWithValue("@attack", item.attack);
        cmd.Parameters.AddWithValue("@defence", item.defence);
        cmd.Parameters.AddWithValue("@health", item.health);
        cmd.Parameters.AddWithValue("@rotation", item.rotation);
        cmd.Parameters.AddWithValue("@speed", item.speed);
        cmd.Parameters.AddWithValue("@visibility", item.visibility);
        cmd.Parameters.AddWithValue("@cannon_reload_speed", item.cannon_reload_speed);
        cmd.ExecuteNonQuery();
    }

    public void EditItem(Item item)
    {
        string sql = @"update item set NAME=@name, ICON_NAME=@icon_name, ITEM_TYPE=@type,
                                       ATTACK=@attack,HEALTH=@health,DEFENCE=@defence,ROTATION=@rotation,SPEED=@speed,
                                       VISIBILITY=@visibility,CANNON_RELOAD_SPEED=@cannon_reload_speed
                       where ID=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", item.item_id);
        cmd.Parameters.AddWithValue("@name", item.name);
        cmd.Parameters.AddWithValue("@icon_name", item.iconName);
        cmd.Parameters.AddWithValue("@type", item.item_type);
        cmd.Parameters.AddWithValue("@attack", item.attack);
        cmd.Parameters.AddWithValue("@defence", item.defence);
        cmd.Parameters.AddWithValue("@health", item.health);
        cmd.Parameters.AddWithValue("@rotation", item.rotation);
        cmd.Parameters.AddWithValue("@speed", item.speed);
        cmd.Parameters.AddWithValue("@visibility", item.visibility);
        cmd.Parameters.AddWithValue("@cannon_reload_speed", item.cannon_reload_speed);
        cmd.ExecuteNonQuery();
    }

    public void DropItem(int player, InventorySlot slot)
    {
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

        sql = "update inventory_slot set item_id=null, quantity=0 where id=@slot_id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", slot_id);
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

        int id_1 = 0;

        while (reader.Read())
        {
            id_1 = reader.GetInt32("id");
        }

        reader.Close();
        
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", add.slotID);

        reader = cmd.ExecuteReader();

        int id_2 = 0;

        while (reader.Read())
        {
            id_2 = reader.GetInt32("id");
        }

        reader.Close();

        cmd.CommandText = "update inventory_slot set slot_id=@new_slot_id where id=@slot_id";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@slot_id", id_1);
        cmd.Parameters.AddWithValue("@new_slot_id", add.slotID);
        cmd.ExecuteNonQuery();

        if (id_2 != 0)
        {
            cmd.CommandText = "update inventory_slot set slot_id=@new_slot_id where id=@slot_id";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@slot_id", id_2);
            cmd.Parameters.AddWithValue("@new_slot_id", remove.slotID);
            cmd.ExecuteNonQuery();
        }
    }

    public void DragAndDrop_Change(int player, InventorySlot slot1, InventorySlot slot2)
    {
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

        int id_2 = 0;
        int slot_id_2 = 0;

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

    public void DragAndDrop_Stack(int player, InventorySlot slot1, InventorySlot slot2)
    {
        string sql = @"select a.id, b.id as slot_id from inventory as a
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

        int id_2 = 0;
        int slot_id_2 = 0;

        while (reader.Read())
        {
            id_2 = reader.GetInt32("id");
            slot_id_2 = reader.GetInt32("slot_id");
        }

        reader.Close();        

        sql = "update inventory_slot set item_id=null, quantity=0 where id=@id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@id", slot_id_1);        
        cmd.ExecuteNonQuery();        

        sql = "update inventory_slot set quantity=@quantity where id=@id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@quantity", slot1.quantity+slot2.quantity);
        cmd.Parameters.AddWithValue("@id", slot_id_2);
        cmd.ExecuteNonQuery();
    }

    public void RemoveInventoryItem(int player, int slotid) {
        string sql = @"select b.id from inventory as a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        where a.player_id=@player_id and b.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", slotid);
        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;        

        while (reader.Read())
        {
            id = reader.GetInt32("id");            
        }

        reader.Close();

        sql = "update inventory_slot set item_id=null where id=@id";
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void AddItemToInventory(int player, InventorySlot slot) {
        string sql = @"select b.id from inventory as a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        where a.player_id=@player_id and b.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@slot_id", slot.slotID);
        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();

        if (id != 0)
        {
            sql = "update inventory_slot set item_id=@item_id where id=@id";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        else {
            sql = "insert into inventory_slot(item_id, slot_id, quantity)values(@item_id, @slot_id, 1);select last_insert_id();";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@slot_id", slot.slotID);
            id = Convert.ToInt32(cmd.ExecuteScalar());

            sql = "insert into inventory(player_id, slot_id)values(@player_id, @slot_id)";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@slot_id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public void AddShipEquipment(int player, Item item) {
        string sql = @"select a.id from ship_equipment as a
                       where a.player_id=@player_id and a.item_type=@item_type";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@item_type", item.item_type);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;        

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();

        if (id != 0)
        {
            sql = "UPDATE ship_equipment set ITEM_ID=@item_id WHERE player_id=@player_id and item_type=@item_type";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);
            cmd.Parameters.AddWithValue("@item_id", item.id);
            cmd.ExecuteNonQuery();
        }
        else {
            sql = "INSERT INTO ship_equipment(ITEM_ID, PLAYER_ID, ITEM_TYPE)VALUES(@item_id, @player_id,@item_type)";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);
            cmd.Parameters.AddWithValue("@item_id", item.id);
            cmd.ExecuteNonQuery();
        }
    }

    public void AddPlayerEquipment(int player, Item item)
    {
        string sql = @"select a.id from player_equipment as a
                       where a.player_id=@player_id and a.item_type=@item_type";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@item_type", item.item_type);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();

        if (id != 0)
        {
            sql = "UPDATE player_equipment set ITEM_ID=@item_id WHERE player_id=@player_id and item_type=@item_type";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);
            cmd.Parameters.AddWithValue("@item_id", item.id);
            cmd.ExecuteNonQuery();
        }
        else
        {
            sql = "INSERT INTO player_equipment(ITEM_ID, PLAYER_ID, ITEM_TYPE)VALUES(@item_id, @player_id,@item_type)";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);
            cmd.Parameters.AddWithValue("@item_id", item.id);
            cmd.ExecuteNonQuery();
        }
    }

    public void RemoveShipEquipment(int player, Item item)
    {
        string sql = @"select a.id from ship_equipment as a
                       where a.player_id=@player_id and a.item_type=@item_type";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@item_type", item.item_type);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();

        if (id != 0)
        {
            sql = "UPDATE ship_equipment set ITEM_ID=null WHERE player_id=@player_id and item_type=@item_type";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);            
            cmd.ExecuteNonQuery();
        }
    }

    public void RemovePlayerEquipment(int player, Item item)
    {
        string sql = @"select a.id from player_equipment as a
                       where a.player_id=@player_id and a.item_type=@item_type";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player);
        cmd.Parameters.AddWithValue("@item_type", item.item_type);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();

        if (id != 0)
        {
            sql = "UPDATE player_equipment set ITEM_ID=null WHERE player_id=@player_id and item_type=@item_type";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@item_type", item.item_type);
            cmd.ExecuteNonQuery();
        }
    }
}
