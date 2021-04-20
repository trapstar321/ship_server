using MySql.Data.MySqlClient;
using SerializableObjects;
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
        string cs = $"server={server};userid={userid};password={password};database={database};";

        con = new MySqlConnection(cs);
        con.Open();

        NetworkManager.skillLevel = ReadSkillLevel();
        NetworkManager.recipes = ReadRecipes();
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
        string sql = @"SELECT b.id as INV_SLOT_ID, b.SLOT_ID, b.QUANTITY, c.id, d.id as item_id, d.name, d.icon_name, d.is_default_item, d.item_type,d.stackable,
                        DROP_CHANCE, MAX_LOOT_QUANTITY,
                        ATTACK,HEALTH,MAX_HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED, CRIT_CHANCE, CANNON_FORCE,ENERGY, MAX_ENERGY, OVERTIME,
                        BUFF_DURATION, COOLDOWN
                        FROM inventory a 
                        inner join inventory_slot as b 
                        on a.slot_id=b.id 
                        inner join player_item as c 
                        on b.item_id=c.id 
                        and a.PLAYER_ID=c.PLAYER_ID
                        inner join item as d
                        on c.item_id=d.id
                        where a.player_id=" + playerID.ToString() + " order by slot_id asc";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<InventorySlot> slots = new List<InventorySlot>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            int inv_slot_id = rdr.GetInt32("INV_SLOT_ID");            
            int slot_id = rdr.GetInt32("SLOT_ID");
            int quantity = rdr.GetInt32("QUANTITY");            

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);

            InventorySlot slot = new InventorySlot() { id= inv_slot_id, slotID = slot_id, quantity = quantity, item = item };
            slots.Add(slot);
        }
        rdr.Close();
        return slots;
    }

    public InventorySlot ReadInventorySlot(int id)
    {
        string sql = @"select a.id as inv_slot_id, c.id as item_id, slot_id, quantity, c.*
                        from inventory_slot as a
                        inner join player_item as b
                        on a.item_id=b.id
                        inner join item as c
                        on b.item_id=c.id
                        where a.id="+id.ToString();

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        InventorySlot slot=null;
        while (rdr.Read())
        {            
            int inv_slot_id = rdr.GetInt32("INV_SLOT_ID");            
            int slot_id = rdr.GetInt32("SLOT_ID");
            int quantity = rdr.GetInt32("QUANTITY");            

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);

            slot = new InventorySlot() { id = inv_slot_id, slotID = slot_id, quantity = quantity, item = item };            
        }
        rdr.Close();
        return slot;
    }

    public List<Item> ReadShipEquipment(int playerID)
    {
        string sql = @"select b.id, c.id as item_id,name,is_default_item, c.item_type,c.icon_name,
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE, CANNON_FORCE,DROP_CHANCE,
                       MAX_LOOT_QUANTITY, STACKABLE, MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
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

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);            

            items.Add(item);
        }
        rdr.Close();
        return items;
    }

    public List<Item> ReadPlayerEquipment(int playerID)
    {
        string sql = @"select b.id, c.id as item_id,name,is_default_item, c.item_type,c.icon_name,
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE, CANNON_FORCE,DROP_CHANCE,
                       MAX_LOOT_QUANTITY, STACKABLE, MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
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

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);

            items.Add(item);
        }
        rdr.Close();
        return items;
    }

    public List<Item> ReadItems()
    {
        string sql = @"select ID, NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE, 
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE, CANNON_FORCE,DROP_CHANCE,
                       MAX_LOOT_QUANTITY, STACKABLE, MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
                       from item";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Item> items = new List<Item>();
        while (rdr.Read())
        {      
            int drop_chance = 0;
            if (!rdr.IsDBNull(14))
            {
                drop_chance = rdr.GetInt32("DROP_CHANCE");
            }
            float max_loot_quantity = 0;

            if (!rdr.IsDBNull(15))
            {
                max_loot_quantity = rdr.GetFloat("MAX_LOOT_QUANTITY"); 
            }            

            
        }
        rdr.Close();
        return items;
    }

    public List<Item> ReadPlayerItems(int player_id)
    {
        string sql = @"select a.id as id, b.id as item_id,b.* from player_item as a
                        inner join item as b
                        on a.item_id=b.id
                        where a.player_id="+player_id;

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Item> items = new List<Item>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");           

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);
        }
        rdr.Close();
        return items;
    }

    public Item ReadItem(int id)
    {
        string sql = @"select ID, NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE,STACKABLE,
                       ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE,CANNON_FORCE,
                       DROP_CHANCE, MAX_LOOT_QUANTITY,
                       MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
                       from item where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", id);
        MySqlDataReader rdr = cmd.ExecuteReader();        

        Item item = new Item();
        while (rdr.Read())
        {
            int item_id = rdr.GetInt32("ID");            
            item.item_id = item_id;
            ReadItem(item, rdr);
        }
        rdr.Close();
        return item;
    }

    public void AddItem(Item item)
    {
        string sql = @"insert into item(NAME, ICON_NAME, IS_DEFAULT_ITEM, ITEM_TYPE,
                                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE,
                                        CANNON_FORCE,STACKABLE)
                       select @name, @icon_name, 0, @type, @attack, @health, @defence, @rotation, @speed, @visibility, 
                              @cannon_reload_speed,@crit_chance,@cannon_force,@stackable";

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
        cmd.Parameters.AddWithValue("@crit_chance", item.crit_chance);
        cmd.Parameters.AddWithValue("@cannon_force", item.cannon_force);
        cmd.Parameters.AddWithValue("@stackable", item.stackable);
        cmd.ExecuteNonQuery();
    }

    public void EditItem(Item item)
    {
        string sql = @"update item set NAME=@name, ICON_NAME=@icon_name, ITEM_TYPE=@type,
                                       ATTACK=@attack,HEALTH=@health,DEFENCE=@defence,ROTATION=@rotation,SPEED=@speed,
                                       VISIBILITY=@visibility,CANNON_RELOAD_SPEED=@cannon_reload_speed,CRIT_CHANCE=@crit_chance,
                                       CANNON_FORCE=@cannon_force, STACKABLE=@stackable
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
        cmd.Parameters.AddWithValue("@crit_chance", item.crit_chance);
        cmd.Parameters.AddWithValue("@cannon_force", item.cannon_force);
        cmd.Parameters.AddWithValue("@stackable", item.stackable);
        cmd.ExecuteNonQuery();
    }

    public int AddPlayerItem(int player_id, Item item)
    {
        string sql = @"insert into player_item(ITEM_ID, PLAYER_ID)
                       values(@item_id, @player_id);select last_insert_id();";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@item_id", item.item_id);
        cmd.Parameters.AddWithValue("@player_id", player_id);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public int GetPlayerItemId(int player_id, Item item)
    {
        string sql = @"select id from player_item where player_id=@player_id and item_id=@item_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player_id);
        cmd.Parameters.AddWithValue("@item_id", item.item_id);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();
        return id;
    }

    public int GetPlayerItemId(int player_id, int item_id)
    {
        string sql = @"select id from player_item where player_id=@player_id and item_id=@item_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@player_id", player_id);
        cmd.Parameters.AddWithValue("@item_id", item_id);

        MySqlDataReader reader = cmd.ExecuteReader();

        int id = 0;

        while (reader.Read())
        {
            id = reader.GetInt32("id");
        }

        reader.Close();
        return id;
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

        sql = "update inventory_slot set item_id=null, quantity=0 where id=@id";
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
            sql = "update inventory_slot set item_id=@item_id, quantity=@quantity where id=@id";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@quantity", slot.quantity);
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        else {
            sql = "insert into inventory_slot(item_id, slot_id, quantity)values(@item_id, @slot_id, @quantity);select last_insert_id();";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@slot_id", slot.slotID);
            cmd.Parameters.AddWithValue("@quantity", slot.quantity);
            id = Convert.ToInt32(cmd.ExecuteScalar());

            sql = "insert into inventory(player_id, slot_id)values(@player_id, @slot_id)";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@player_id", player);
            cmd.Parameters.AddWithValue("@slot_id", id);
            cmd.ExecuteNonQuery();
        }
    }

    public void UpdateItemQuantity(int dbid, InventorySlot slot) {
        string sql = @"update inventory_slot a
                        inner join inventory as b
                        on a.id=b.slot_id
                        set a.quantity=@quantity
                        where player_id = @player_id and a.slot_id=@slot_id";

        var cmd = new MySqlCommand(sql, con);

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@quantity", slot.quantity);        
        cmd.Parameters.AddWithValue("@player_id", dbid);
        cmd.Parameters.AddWithValue("@slot_id", slot.slotID);
        cmd.ExecuteNonQuery();
    }    

    public void SaveInventorySlot(int player, InventorySlot slot)
    {
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
            sql = "update inventory_slot set item_id=@item_id, quantity=@quantity where id=@id";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@quantity", slot.quantity);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        else
        {
            sql = "insert into inventory_slot(item_id, slot_id, quantity)values(@item_id, @slot_id, @quantity);select last_insert_id();";
            cmd.CommandText = sql;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@item_id", slot.item.id);
            cmd.Parameters.AddWithValue("@slot_id", slot.slotID);
            cmd.Parameters.AddWithValue("@quantity", slot.quantity);
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

    public PlayerData ReadPlayerData(int id) {
        string sql = @"select LEVEL,EXPERIENCE, USERNAME, X_SHIP, Y_SHIP, Z_SHIP, Y_ROT_SHIP,
                        X_PLAYER, Y_PLAYER, Z_PLAYER, Y_ROT_PLAYER, IS_ON_SHIP, GOLD, DEAD, SUNK
                       from player where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", id);
        MySqlDataReader rdr = cmd.ExecuteReader();

        PlayerData data=null;

        while (rdr.Read())
        {
            int level = rdr.GetInt32("LEVEL");
            int experience = rdr.GetInt32("EXPERIENCE");
            float X_ship = 0;
            if (!rdr.IsDBNull(3))
                X_ship = rdr.GetFloat("X_SHIP");

            float Y_ship = 0;
            if (!rdr.IsDBNull(4))
                Y_ship = rdr.GetFloat("Y_SHIP");

            float Z_ship = 0;
            if (!rdr.IsDBNull(5))
                Z_ship = rdr.GetFloat("Z_SHIP");

            float Y_rot_ship = 0;
            if (!rdr.IsDBNull(6))
                Y_rot_ship = rdr.GetFloat("Y_ROT_SHIP");

            float X_player = 0;
            if (!rdr.IsDBNull(7))
                X_player = rdr.GetFloat("X_PLAYER");

            float Y_player = 0;
            if (!rdr.IsDBNull(8))
                Y_player = rdr.GetFloat("Y_PLAYER");

            float Z_player = 0;
            if (!rdr.IsDBNull(9))
                Z_player = rdr.GetFloat("Z_PLAYER");

            float Y_rot_player = 0;
            if (!rdr.IsDBNull(10))
                Y_rot_player = rdr.GetFloat("Y_ROT_PLAYER");

            bool is_on_ship = false;
            if (!rdr.IsDBNull(11))
                is_on_ship = rdr.GetBoolean("IS_ON_SHIP");

            bool dead = rdr.GetBoolean("DEAD");
            bool sunk = rdr.GetBoolean("SUNK");

            string username = rdr.GetString("USERNAME");
            float gold = rdr.GetFloat("GOLD");

            data = new PlayerData();
            data.level = level;
            data.experience = experience;
            data.X_SHIP = X_ship;
            data.Y_SHIP = Y_ship;
            data.Z_SHIP = Z_ship;
            data.Y_ROT_SHIP = Y_rot_ship;

            data.X_PLAYER = X_player;
            data.Y_PLAYER = Y_player;
            data.Z_PLAYER = Z_player;
            data.Y_ROT_PLAYER = Y_rot_player;

            data.is_on_ship = is_on_ship;

            data.username = username;
            data.gold = gold;
            data.dead = dead;
            data.sunk = sunk;
        }
        rdr.Close();
        return data;
    }

    public List<ShipBaseStat> ReadShipBaseStatsTable()
    {
        string sql = @"select LEVEL,ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE,
                              CANNON_FORCE
                       from ship_base_stats";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<ShipBaseStat> stats = new List<ShipBaseStat>();
        while (rdr.Read())
        {
            float level = rdr.GetFloat("LEVEL");
            float attack = rdr.GetFloat("ATTACK");
            float health = rdr.GetFloat("HEALTH");
            float defence = rdr.GetFloat("DEFENCE");
            float rotation = rdr.GetFloat("ROTATION");
            float speed = rdr.GetFloat("SPEED");
            float visibility = rdr.GetFloat("VISIBILITY");
            float cannon_reload_speed = rdr.GetFloat("CANNON_RELOAD_SPEED");
            float crit_chance = rdr.GetFloat("CRIT_CHANCE");
            float cannon_force = rdr.GetFloat("CANNON_FORCE");

            ShipBaseStat stat = new ShipBaseStat();
            stat.level = level;
            stat.attack = attack;
            stat.health = health;
            stat.defence = defence;
            stat.rotation = rotation;
            stat.speed = speed;
            stat.visibility = visibility;
            stat.cannon_reload_speed = cannon_reload_speed;
            stat.crit_chance = crit_chance;
            stat.cannon_force = cannon_force;
            stats.Add(stat);
        }
        rdr.Close();
        return stats;
    }

    public List<PlayerBaseStat> ReadPlayerBaseStatsTable()
    {
        string sql = @"select LEVEL,ATTACK,HEALTH,DEFENCE,SPEED,CRIT_CHANCE,ENERGY
                       from player_base_stats";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<PlayerBaseStat> stats = new List<PlayerBaseStat>();
        while (rdr.Read())
        {
            float level = rdr.GetFloat("LEVEL");
            float attack = rdr.GetFloat("ATTACK");
            float health = rdr.GetFloat("HEALTH");
            float defence = rdr.GetFloat("DEFENCE");            
            float speed = rdr.GetFloat("SPEED");
            float crit_chance = rdr.GetFloat("CRIT_CHANCE");
            float energy = rdr.GetFloat("ENERGY");

            PlayerBaseStat stat = new PlayerBaseStat();
            stat.level = level;
            stat.attack = attack;
            stat.health = health;
            stat.defence = defence;            
            stat.speed = speed;
            stat.crit_chance = crit_chance;
            stat.energy = energy;
            stats.Add(stat);
        }
        rdr.Close();
        return stats;
    }

    public List<ShipBaseStat> ReadNPCBaseStatsTable()
    {
        string sql = @"select LEVEL,ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED,CRIT_CHANCE,
                              CANNON_FORCE
                       from npc_base_stats";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<ShipBaseStat> stats = new List<ShipBaseStat>();
        while (rdr.Read())
        {
            float level = rdr.GetFloat("LEVEL");
            float attack = rdr.GetFloat("ATTACK");
            float health = rdr.GetFloat("HEALTH");
            float defence = rdr.GetFloat("DEFENCE");
            float rotation = rdr.GetFloat("ROTATION");
            float speed = rdr.GetFloat("SPEED");
            float visibility = rdr.GetFloat("VISIBILITY");
            float cannon_reload_speed = rdr.GetFloat("CANNON_RELOAD_SPEED");
            float crit_chance = rdr.GetFloat("CRIT_CHANCE");
            float cannon_force = rdr.GetFloat("CANNON_FORCE");

            ShipBaseStat stat = new ShipBaseStat();
            stat.level = level;
            stat.attack = attack;
            stat.health = health;
            stat.defence = defence;
            stat.rotation = rotation;
            stat.speed = speed;
            stat.visibility = visibility;
            stat.cannon_reload_speed = cannon_reload_speed;
            stat.crit_chance = crit_chance;
            stat.cannon_force = cannon_force;
            stats.Add(stat);
        }
        rdr.Close();
        return stats;
    }

    public List<Experience> ReadExperienceTable()
    {
        string sql = @"select LEVEL,FROM_,TO_
                       from experience";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Experience> experience = new List<Experience>();
        while (rdr.Read())
        {
            int level = rdr.GetInt32("LEVEL");
            int from = rdr.GetInt32("FROM_");
            int to = rdr.GetInt32("TO_");

            Experience exp = new Experience();
            exp.level = level;
            exp.from = from;
            exp.to = to;

            experience.Add(exp);
        }
        rdr.Close();
        return experience;
    }

    public List<Player> GetPlayers() {
        string sql = @"select ID,USERNAME
                       from player";

        var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Player> players = new List<Player>();
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            string username = rdr.GetString("USERNAME");            

            Player player = new Player();
            player.id = id;            

            players.Add(player);
        }
        rdr.Close();
        return players;
    }

    public void UpdateShipPosition(int id, float X, float Y, float Z, float Y_rot) {
        string sql = @"UPDATE player set X_SHIP=@x, Y_SHIP=@y, Z_SHIP=@z, Y_ROT_SHIP=@Y_rot WHERE id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;       
        
        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@X", X);
        cmd.Parameters.AddWithValue("@Y", Y);
        cmd.Parameters.AddWithValue("@Z", Z);
        cmd.Parameters.AddWithValue("@Y_rot", Y_rot);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();       
    }

    public void UpdatePlayerPosition(int id, float X, float Y, float Z, float Y_rot)
    {
        string sql = @"UPDATE player set X_PLAYER=@x, Y_PLAYER=@y, Z_PLAYER=@z, Y_ROT_PLAYER=@Y_rot WHERE id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@X", X);
        cmd.Parameters.AddWithValue("@Y", Y);
        cmd.Parameters.AddWithValue("@Z", Z);
        cmd.Parameters.AddWithValue("@Y_rot", Y_rot);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void UpdatePlayerGold(int id, float gold)
    {
        string sql = @"UPDATE player set GOLD=@gold WHERE id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@gold", gold);        
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void UpdatePlayerIsOnShip(int id, bool isOnShip)
    {
        string sql = @"UPDATE player set IS_ON_SHIP=@on_ship WHERE id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@on_ship", isOnShip);        
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public int Login(string username, string password) {
        string sql = @"select ID from player where username=@username and password=@password";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Player> players = new List<Player>();
        int id = 0;
        while (rdr.Read())
        {
            id = rdr.GetInt32("ID");                        
        }
        rdr.Close();
        return id;
    }

    public List<PlayerSkillLevel> ReadPlayerSkills(int playerID)
    {
        string sql = @"select a.id, a.skill_id, lvl, modifier, exp_start, exp_end, skill_name, experience
                        from skill_level as a
                        inner join skill as b
                        on a.skill_id=b.id
                        inner join player_skill_level as c
                        on a.id = c.skill_level_id
                        where c.player_id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", playerID);
        
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<PlayerSkillLevel> skills = new List<PlayerSkillLevel>();        
        while (rdr.Read())
        {
            int id = rdr.GetInt32("ID");
            int skill_id = rdr.GetInt32("SKILL_ID");
            int level = rdr.GetInt32("LVL");
            int modifier = rdr.GetInt32("MODIFIER");
            int exp_start = rdr.GetInt32("EXP_START");
            int exp_end = rdr.GetInt32("EXP_END");
            string skill_name = rdr.GetString("SKILL_NAME");
            int experience = rdr.GetInt32("EXPERIENCE");

            PlayerSkillLevel skill = new PlayerSkillLevel();
            skill.id = id;
            skill.skill_id = skill_id;
            skill.level = level;
            skill.modifier = modifier;
            skill.experience_start = exp_start;
            skill.experience_end = exp_end;
            skill.name = skill_name;
            skill.experience = experience;

            skills.Add(skill);
        }
        rdr.Close();

        return skills;
    }

    public List<ResourceSpawn> ReadResourceSpawns()
    {
        string sql = @"select* from resource as a
                        inner join resource_spawn as b
                        on a.id=b.resource_id";

        var cmd = new MySqlCommand(sql, con);        

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<ResourceSpawn> spawns = new List<ResourceSpawn>();
        while (rdr.Read())
        {
            Resource_ resource = new Resource_();
            resource.ITEM_ID = rdr.GetInt32("ITEM_ID");
            resource.RESOURCE_TYPE = rdr.GetInt32("RESOURCE_TYPE");
            resource.RESOURCE_HP = rdr.GetFloat("RESOURCE_HP");
            resource.RESOURCE_COUNT = rdr.GetInt32("RESOURCE_COUNT");
            resource.SKILL_TYPE = (SkillType)rdr.GetInt32("SKILL_TYPE");
            resource.EXPERIENCE = rdr.GetFloat("EXPERIENCE");

            ResourceSpawn spawn = new ResourceSpawn();
            spawn.RESOURCE = resource;
            spawn.RESPAWN_TIME = rdr.GetFloat("RESPAWN_TIME");
            spawn.X = rdr.GetFloat("X");
            spawn.Y = rdr.GetFloat("Y");
            spawn.Z = rdr.GetFloat("Z");            

            spawns.Add(spawn);
        }
        rdr.Close();

        return spawns;
    }

    public List<CraftingSpotSpawn> ReadCraftingSpots()
    {
        string sql = @"select* from crafting_spot_spawn";

        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<CraftingSpotSpawn> spawns = new List<CraftingSpotSpawn>();
        while (rdr.Read())
        {
            CraftingSpotSpawn craftingSpot = new CraftingSpotSpawn();
            craftingSpot.skillType = (SkillType)rdr.GetInt32("SKILL_TYPE");
            craftingSpot.x = rdr.GetFloat("X");
            craftingSpot.y = rdr.GetFloat("Y");
            craftingSpot.z = rdr.GetFloat("Z");
            craftingSpot.Y_rot = rdr.GetFloat("Y_ROT");
            craftingSpot.gameObjectType = rdr.GetInt32("GAME_OBJECT_TYPE");

            spawns.Add(craftingSpot);
        }
        rdr.Close();

        return spawns;
    }

    public List<SkillLevel> ReadSkillLevel()
    {
        string sql = @"select a.id, skill_id, lvl, modifier, exp_start, exp_end, skill_name, icon
                        from skill_level as a
                        inner join skill as b
                        on a.skill_id=b.id";

        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<SkillLevel> skills = new List<SkillLevel>();
        while (rdr.Read())
        {
            SkillLevel skillLevel = new SkillLevel();
            skillLevel.skill_level_id = rdr.GetInt32("ID");
            skillLevel.skill = (SkillType)rdr.GetInt32("SKILL_ID");
            skillLevel.modifier = rdr.GetInt32("MODIFIER");
            skillLevel.experienceStart = rdr.GetInt32("EXP_START");
            skillLevel.experienceEnd = rdr.GetInt32("EXP_END");
            skillLevel.level = rdr.GetInt32("LVL");
            skillLevel.skillName = rdr.GetString("SKILL_NAME");
            skillLevel.icon = rdr.GetString("ICON");
            
            skills.Add(skillLevel);
        }
        rdr.Close();

        return skills;
    }

    public void UpdateSkillExperience(int player_id, int skill_id, int experience)
    {
        string sql = @"update player_skill_level a
                        inner join skill_level as b
                        on a.skill_level_id=b.id
                        set a.experience=a.experience + @experience
                        where player_id=@player_id and skill_id=@skill_id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player_id);
        cmd.Parameters.AddWithValue("@skill_id", skill_id);
        cmd.Parameters.AddWithValue("@experience", experience);        
        cmd.ExecuteNonQuery();
    }

    public void DeletePlayerSkillLevel(int player_id, int skill_id, int lvl) {
        string sql = @"delete a
                        from player_skill_level as a
                        inner join skill_level as b
                        on a.skill_level_id = b.id
                        where player_id = @player_id and skill_id = @skill_id and lvl = @level";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player_id);
        cmd.Parameters.AddWithValue("@skill_id", skill_id);
        cmd.Parameters.AddWithValue("@level", lvl);
        cmd.ExecuteNonQuery();
    }

    public void InsertPlayerSkillLevel(int player_id, int skill_level_id, int experience) {
        string sql = @"insert into player_skill_level
                        (player_id, skill_level_id, experience)
                        select @player_id, @skill_level_id, @experience";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@player_id", player_id);
        cmd.Parameters.AddWithValue("@skill_level_id", skill_level_id);
        cmd.Parameters.AddWithValue("@experience", experience);
        cmd.ExecuteNonQuery();
    }

    public List<Recipe> ReadRecipes()
    {
        string sql = @"select a.id, a.recipe_name, a.time_to_craft, b.crafting_exp, b.icon_name, a.item_id, a.skill_id
                        from recipe as a
                        inner join item as b
                        on a.item_id=b.id";

        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Recipe> recipes = new List<Recipe>();
        while (rdr.Read())
        {
            Recipe recipe = new Recipe();
            recipe.id = rdr.GetInt32("ID");
            recipe.name = rdr.GetString("recipe_name");
            recipe.time_to_craft = rdr.GetFloat("time_to_craft");
            recipe.experience = rdr.GetInt32("crafting_exp");
            recipe.icon_name = rdr.GetString("icon_name");
            recipe.item_id = rdr.GetInt32("item_id");
            recipe.skill_id = rdr.GetInt32("skill_id");

            recipes.Add(recipe);
        }
        rdr.Close();        

        foreach (Recipe recipe in recipes) {
            string itemsSql = @"select a.id, b.id as item_id, b.name, b.icon_name, quantity 
                                from recipe_item_requirements as a
                                inner join item as b
                                on a.item_id = b.id
                                where a.recipe_id = @id";
            var itemsCmd = new MySqlCommand(itemsSql, con);
            recipe.items = new List<RecipeItemRequirement>();
            itemsCmd.Parameters.AddWithValue("@id", recipe.id);

            MySqlDataReader itemsRdr = itemsCmd.ExecuteReader();
            while (itemsRdr.Read())
            {
                RecipeItemRequirement item = new RecipeItemRequirement();
                item.id = itemsRdr.GetInt32("ID");
                item.item_id = itemsRdr.GetInt32("item_id");
                item.icon_name = itemsRdr.GetString("ICON_NAME");
                item.item_name = itemsRdr.GetString("NAME");
                item.quantity = itemsRdr.GetInt32("QUANTITY");
                recipe.items.Add(item);
            }
            itemsRdr.Close();

            string skillSql = @"select skill_level_id, lvl, skill_name, modifier
                                from recipe as a
                                inner join recipe_skill_requirements as b
                                on a.id=b.recipe_id
                                inner join skill_level as c
                                on b.skill_level_id=c.id
                                inner join skill as d
                                on c.skill_id=d.id
                                where a.id=@id";
            var skillCmd = new MySqlCommand(skillSql, con);
            recipe.skill = new RecipeSkillRequirement();
            skillCmd.Parameters.AddWithValue("@id", recipe.id);

            MySqlDataReader skillRdr = skillCmd.ExecuteReader();
            while (skillRdr.Read())
            {
                recipe.skill.skill_level_id = skillRdr.GetInt32("SKILL_LEVEL_ID");
                recipe.skill.level = skillRdr.GetInt32("LVL");
                recipe.skill.skill_name = skillRdr.GetString("SKILL_NAME");
                recipe.skill.modifier = skillRdr.GetInt32("MODIFIER");
            }
            skillRdr.Close();
        }

        rdr.Close();

        return recipes;
    }

    public List<SerializableObjects.Trader> ReadTraders()
    {
        string sql = @"select* from trader";

        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<SerializableObjects.Trader> traders = new List<SerializableObjects.Trader>();
        while (rdr.Read())
        {
            SerializableObjects.Trader trader = new SerializableObjects.Trader();
            trader.id = rdr.GetInt32("ID");
            trader.name = rdr.GetString("NAME");
            trader.x = rdr.GetFloat("X");
            trader.y = rdr.GetFloat("Y");
            trader.z = rdr.GetFloat("Z");
            trader.y_rot = rdr.GetFloat("Y_ROT");
            trader.item_respawn_time = rdr.GetFloat("ITEM_RESPAWN_TIME");
            trader.game_object_type = rdr.GetInt32("GAME_OBJECT_TYPE");

            traders.Add(trader);
        }
        rdr.Close();        

        foreach(SerializableObjects.Trader trader in traders)
        {
            string traderSql = @"select * from trader_inventory where trader_id = @id";
            var traderCmd = new MySqlCommand(traderSql, con);
            trader.inventory = new List<TraderItem>();
            traderCmd.Parameters.AddWithValue("@id", trader.id);

            MySqlDataReader traderRdr = traderCmd.ExecuteReader();
            while (traderRdr.Read())
            {
                TraderItem item = new TraderItem();
                item.item_id = traderRdr.GetInt32("ITEM_ID");
                item.quantity = traderRdr.GetInt32("QUANTITY");
                item.sell_price = traderRdr.GetFloat("SELL_PRICE");
                item.buy_price = traderRdr.GetFloat("BUY_PRICE");

                trader.inventory.Add(item);
            }
            traderRdr.Close();

            foreach (TraderItem item in trader.inventory)
            {
                item.item = NetworkManager.ItemToSerializable(ReadItem(item.item_id));
            }
        }        

        return traders;
    }

    public List<SerializableObjects.TradeBroker> ReadTradeBrokers()
    {
        string sql = @"select* from trade_broker";

        var cmd = new MySqlCommand(sql, con);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<SerializableObjects.TradeBroker> traders = new List<SerializableObjects.TradeBroker>();
        while (rdr.Read())
        {
            SerializableObjects.TradeBroker trader = new SerializableObjects.TradeBroker();
            trader.id = rdr.GetInt32("ID");            
            trader.x = rdr.GetFloat("X");
            trader.y = rdr.GetFloat("Y");
            trader.z = rdr.GetFloat("Z");
            trader.y_rot = rdr.GetFloat("Y_ROT");
            trader.game_object_type = rdr.GetInt32("GAME_OBJECT_TYPE");

            traders.Add(trader);
        }
        rdr.Close();

        return traders;
    }
    public SerializableObjects.Trader ReadTrader(int traderId)
    {
        string sql = @"select* from trader where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", traderId);

        MySqlDataReader rdr = cmd.ExecuteReader();

        SerializableObjects.Trader trader = null;
        while (rdr.Read())
        {
            SerializableObjects.Trader t = new SerializableObjects.Trader();
            t.id = rdr.GetInt32("ID");
            t.name = rdr.GetString("NAME");
            t.x = rdr.GetFloat("X");
            t.y = rdr.GetFloat("Y");
            t.z = rdr.GetFloat("Z");
            t.y_rot = rdr.GetFloat("Y_ROT");
            t.item_respawn_time = rdr.GetFloat("ITEM_RESPAWN_TIME");
            t.game_object_type = rdr.GetInt32("GAME_OBJECT_TYPE");

            trader =t;
        }
        rdr.Close();
        
        string traderSql = @"select * from trader_inventory where trader_id = @id";
        var traderCmd = new MySqlCommand(traderSql, con);
        trader.inventory = new List<TraderItem>();
        traderCmd.Parameters.AddWithValue("@id", trader.id);

        MySqlDataReader traderRdr = traderCmd.ExecuteReader();
        while (traderRdr.Read())
        {
            TraderItem item = new TraderItem();
            item.item_id = traderRdr.GetInt32("ITEM_ID");
            item.quantity = traderRdr.GetInt32("QUANTITY");
            item.sell_price = traderRdr.GetFloat("SELL_PRICE");
            item.buy_price = traderRdr.GetFloat("BUY_PRICE");

            trader.inventory.Add(item);
        }
        traderRdr.Close();

        foreach (TraderItem item in trader.inventory)
        {
            item.item = NetworkManager.ItemToSerializable(ReadItem(item.item_id));
        }

        return trader;
    }

    public List<Category> ReadCategories()
    {
        string sql = @"select* from category";

        var cmd = new MySqlCommand(sql, con);
        
        MySqlDataReader rdr = cmd.ExecuteReader();

        List<Category> categories = new List<Category>();
        while (rdr.Read())
        {
            Category category = new Category();
            category.id = rdr.GetInt32("ID");
            category.name = rdr.GetString("NAME");
            category.icon = rdr.GetString("ICON");

            categories.Add(category);
        }

        rdr.Close();
        return categories;
    }

    public List<TradeBrokerItem> ReadTradeBrokerItems(int playerId, int? categoryId, string name, bool showMyItems, bool showSoldItems) {
        string sql = @"select distinct username, a.player_id, a.id, price, quantity, c.id as item_id, c.name, c.icon_name, c.is_default_item, c.item_type,c.stackable,
                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED, CRIT_CHANCE, CANNON_FORCE, DROP_CHANCE, MAX_LOOT_QUANTITY, STACKABLE, SOLD,
                        MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
                        from trade_broker_items as a                        
                        inner join item as c
                        on a.item_id=c.id
                        inner join item_category as d
                        on d.item_id=c.id
                        inner join category as e
                        on d.category_id=e.id
                        inner join player as f
                        on a.player_id=f.id";
        
        if (categoryId.HasValue)
        {
            sql += " and e.id=@category_id";            
        }

        if (showMyItems)
        {
            sql += " and f.id = @player_id";
        }

        if (showSoldItems)
        {
            sql += " and f.id = @player_id";
            sql += " and a.parent_id is not null";
            sql += " and sold = true";
        }
        else {
            sql += " and sold=false";
        }

        if (name != null)
        {
            sql += " and c.name like '%"+name+"%'";            
        }

        var cmd = new MySqlCommand(sql, con);
        if(categoryId.HasValue)
            cmd.Parameters.AddWithValue("@category_id", categoryId.Value);
        if (showMyItems)
            cmd.Parameters.AddWithValue("@player_id", playerId);
        if(showSoldItems && !cmd.Parameters.Contains("@player_id"))
            cmd.Parameters.AddWithValue("@player_id", playerId);

        MySqlDataReader rdr = cmd.ExecuteReader();

        List<TradeBrokerItem> items = new List<TradeBrokerItem>();
        while (rdr.Read())
        {
            TradeBrokerItem broker_item = new TradeBrokerItem();

            int id = rdr.GetInt32("ID");
            int quantity = rdr.GetInt32("QUANTITY");
            float price = rdr.GetFloat("PRICE");                        
            string seller = rdr.GetString("USERNAME");
            int pId = rdr.GetInt32("PLAYER_ID");
            bool sold = rdr.GetBoolean("SOLD");

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);

            broker_item.item = NetworkManager.ItemToSerializable(item);
            broker_item.price = price;
            broker_item.quantity = quantity;
            broker_item.id = id;
            broker_item.seller = seller;
            broker_item.IsMyItem = playerId == pId;
            broker_item.sold = sold;
            items.Add(broker_item);
        }
        rdr.Close();
        return items;
    }

    public TradeBrokerItem ReadTradeBrokerItem(int playerId, int itemId)
    {
        string sql = @"select distinct username, a.player_id, a.id, price, quantity, c.id as item_id, c.name, c.icon_name, c.is_default_item, c.item_type,c.stackable,
                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED, CRIT_CHANCE, CANNON_FORCE, DROP_CHANCE, MAX_LOOT_QUANTITY, STACKABLE, SOLD, PARENT_ID,
                        MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
                        from trade_broker_items as a                        
                        inner join item as c
                        on a.item_id=c.id
                        inner join item_category as d
                        on d.item_id=c.id
                        inner join category as e
                        on d.category_id=e.id
                        inner join player as f
                        on a.player_id=f.id
                        where a.id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", itemId);
        MySqlDataReader rdr = cmd.ExecuteReader();        

        TradeBrokerItem broker_item = null;
        while (rdr.Read())
        {
            broker_item = new TradeBrokerItem();

            int id = rdr.GetInt32("ID");
            int quantity = rdr.GetInt32("QUANTITY");
            float price = rdr.GetFloat("PRICE");            
            string seller = rdr.GetString("USERNAME");
            int pId = rdr.GetInt32("PLAYER_ID");
            bool sold = rdr.GetBoolean("SOLD");
            int? parent_id;
            if (rdr.IsDBNull(rdr.GetOrdinal("PARENT_ID")))
            {
                parent_id = null;
            }
            else {
                parent_id = rdr.GetInt32("PARENT_ID");
            }

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);            

            broker_item.item = NetworkManager.ItemToSerializable(item);
            broker_item.price = price;
            broker_item.quantity = quantity;
            broker_item.id = id;
            broker_item.seller = seller;
            broker_item.seller_id = pId;
            broker_item.IsMyItem = playerId == pId;
            broker_item.sold = sold;
            broker_item.parent_id = parent_id;
        }
        rdr.Close();
        return broker_item;
    }

    public TradeBrokerItem ReadSoldTradeBrokerItem(int playerId, int itemId)
    {
        string sql = @"select distinct username, a.player_id, a.id, price, quantity, c.id as item_id, c.name, c.icon_name, c.is_default_item, c.item_type,c.stackable,
                        ATTACK,HEALTH,DEFENCE,ROTATION,SPEED,VISIBILITY,CANNON_RELOAD_SPEED, CRIT_CHANCE, CANNON_FORCE, DROP_CHANCE, MAX_LOOT_QUANTITY, STACKABLE, SOLD,
                        MAX_HEALTH, ENERGY, MAX_ENERGY, OVERTIME, BUFF_DURATION, COOLDOWN
                        from trade_broker_items as a                        
                        inner join item as c
                        on a.item_id=c.id
                        inner join item_category as d
                        on d.item_id=c.id
                        inner join category as e
                        on d.category_id=e.id
                        inner join player as f
                        on a.player_id=f.id
                        where a.parent_id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", itemId);
        MySqlDataReader rdr = cmd.ExecuteReader();

        TradeBrokerItem broker_item = null;
        while (rdr.Read())
        {
            broker_item = new TradeBrokerItem();

            int id = rdr.GetInt32("ID");
            int quantity = rdr.GetInt32("QUANTITY");
            float price = rdr.GetFloat("PRICE");            
            string seller = rdr.GetString("USERNAME");
            int pId = rdr.GetInt32("PLAYER_ID");
            bool sold = rdr.GetBoolean("SOLD");

            Item item = new Item();
            item.id = id;
            ReadItem(item, rdr);            

            broker_item.item = NetworkManager.ItemToSerializable(item);
            broker_item.price = price;
            broker_item.quantity = quantity;
            broker_item.id = id;
            broker_item.seller_id = pId;
            broker_item.seller = seller;
            broker_item.IsMyItem = playerId == pId;
            broker_item.sold = sold;
        }
        rdr.Close();
        return broker_item;
    }

    public void AddTradeBrokerItem(int playerId, int itemId, int quantity, float price, int? parent_id=null, bool sold = false)
    {
        string sql = @"insert into trade_broker_items(item_id, quantity, price, player_id, parent_id, sold)
                       select @item_id, @quantity, @price, @player_id, @parent_id, @sold";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@item_id", itemId);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@price", price);
        cmd.Parameters.AddWithValue("@player_id", playerId);
        cmd.Parameters.AddWithValue("@sold", sold);

        if (parent_id.HasValue)
            cmd.Parameters.AddWithValue("@parent_id", parent_id);
        else
            cmd.Parameters.AddWithValue("@parent_id", DBNull.Value);

        cmd.ExecuteNonQuery();        
    }

    public void UpdateTradeBrokerItem(int id, int quantity, bool sold=false)
    {
        string sql = @"update trade_broker_items set quantity=@quantity, sold=@sold where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@sold", sold);

        cmd.ExecuteNonQuery();
    }

    public void RemoveTradeBrokerItem(int id)
    {
        string sql = @"delete from trade_broker_items where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();
    }

    public void DropTradeBrokerItem(int id)
    {
        string sql = @"delete from trade_broker_items where id=@id";                        

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.CommandText = sql;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@id", id);        
        cmd.ExecuteNonQuery();
    }

    public bool TradeBrokerAllItemsCollected(int id) {
        string sql = @"select* from trade_broker_items where parent_id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", id);

        MySqlDataReader rdr = cmd.ExecuteReader();

        bool result = true;
        while (rdr.Read())
        {
            result = false;
            break;
        }
        rdr.Close();
        return result;
    }

    public InventorySlot InventoryAdd(Player player, Item item, int amount)
    {
        int id = 0;
        if (item.stackable)
        {
            id = GetPlayerItemId(player.dbid, item);
            if (id == 0)
            {
                id = AddPlayerItem(player.dbid, item);
            }
        }
        else
        {
            id = AddPlayerItem(player.dbid, item);
        }

        InventorySlot slot = player.inventory.Add(item, amount);
        if (slot == null)
        {
            AddTemporaryStorage(player.dbid, item, amount);            
        }
        else {
            slot.item.id = id;
            AddItemToInventory(player.dbid, slot);
        }
        return slot;
    }

    public void InventoryRemove(Player player, int item_id, int quantity) {
        InventorySlot slot = FindSlot(player.inventory, item_id);
        player.inventory.RemoveAmount(slot.slotID, quantity);

        if (slot.item == null)
        {
            RemoveInventoryItem(player.dbid, slot.slotID);
        }
        else
        {
            UpdateItemQuantity(player.dbid, slot);
        }
    }

    public InventorySlot FindSlot(Inventory inventory, int item_id)
    {
        foreach (InventorySlot slot in inventory.items)
        {
            if (slot.item != null && slot.item.item_id == item_id)
            {
                return slot;
            }
        }
        return null;
    }

    public void GetTemporaryStorage(int playerItemId, out int id, out int quantity) {
        id = 0;
        quantity = 0;
        string sql = @"select id, quantity from temporary_storage where item_id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", playerItemId);

        MySqlDataReader rdr = cmd.ExecuteReader();
        
        while (rdr.Read())
        {            
            id = rdr.GetInt32("ID");
            quantity = rdr.GetInt32("QUANTITY");
        }

        rdr.Close();        
    }

    public void AddTemporaryStorage(int playerId, Item item, int quantity)
    {
        int playerItemId;
        if (item.stackable)
        {
            playerItemId = GetPlayerItemId(playerId, item);
            if (playerItemId == 0)
            {
                playerItemId = AddPlayerItem(playerId, item);
            }
        }
        else
        {
            playerItemId = AddPlayerItem(playerId, item);
        }

        int id, store_quantity;
        GetTemporaryStorage(playerItemId, out id, out store_quantity);

        if (id == 0)
        {
            string sql = @"insert into temporary_storage(item_id, quantity)
                       select @item_id, @quantity";

            var cmd = new MySqlCommand(sql, con);
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@item_id", playerItemId);
            cmd.Parameters.AddWithValue("@quantity", quantity);

            cmd.ExecuteNonQuery();
        }
        else {
            string sql = @"update temporary_storage set quantity=@quantity where id=@id";                       

            var cmd = new MySqlCommand(sql, con);
            cmd.CommandText = sql;

            cmd.Parameters.AddWithValue("@quantity", store_quantity+quantity);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
    }

    public void RemoveTemporaryStorage(int id)
    {
        string sql = @"delete from temporary_storage where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", id);

        cmd.ExecuteNonQuery();
    }

    public int TotalItems(int playerId) {
        string sql = @"select count(*)+(select count(*) from temporary_storage as a
                        inner join player_item as b
                        on a.item_id=b.id
                        where b.player_id=@id) as count
                        from inventory as a
                        inner join inventory_slot as b
                        on a.slot_id=b.id
                        where a.player_id=@id and b.item_id is not null";        

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", playerId);

        MySqlDataReader rdr = cmd.ExecuteReader();

        int count = 0;
        while (rdr.Read())
        {
            count = rdr.GetInt32("count");            
        }
        rdr.Close();
        return count;
    }

    public Item GetTempStorageItem(int playerId, out int id, out int quantity)
    {
        string sql = @"select a.id, c.id as item_id, a.quantity
                        from temporary_storage as a
                        inner join player_item as b
                        on a.item_id=b.id
                        inner join item as c
                        on b.item_id=c.id
                        where b.player_id=@id
                        limit 1";

        var cmd = new MySqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@id", playerId);

        MySqlDataReader rdr = cmd.ExecuteReader();

        int item_id = 0;
        Item item = null;
        quantity = 1;
        id = 0;
        while (rdr.Read())
        {
            id = rdr.GetInt32("id");
            item_id = rdr.GetInt32("item_id");
            quantity = rdr.GetInt32("quantity");            
        }
        rdr.Close();

        if (item_id != 0) {
            item = ReadItem(item_id);
        }

        return item;
    }

    public void DiePlayerCharacter(int dbid)
    {
        string sql = @"update player set dead = true where id=@id";                       

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", dbid);        
        cmd.ExecuteNonQuery();
    }

    public void RespawnPlayerCharacter(int dbid)
    {
        string sql = @"update player set dead = false where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", dbid);
        cmd.ExecuteNonQuery();
    }

    public void SinkShip(int dbid)
    {
        string sql = @"update player set sunk = true where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", dbid);
        cmd.ExecuteNonQuery();
    }

    public void RespawnShip(int dbid)
    {
        string sql = @"update player set sunk = false where id=@id";

        var cmd = new MySqlCommand(sql, con);
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("@id", dbid);
        cmd.ExecuteNonQuery();
    }

    public void ReadItem(Item item, MySqlDataReader rdr) {        
        if(HasColumn(rdr, "ITEM_ID"))
            item.item_id = rdr.GetInt32("ITEM_ID");
        string name = rdr.GetString("NAME");
        string icon_name = rdr.GetString("ICON_NAME");
        string item_type = rdr.GetString("ITEM_TYPE");
        bool is_default_item = rdr.GetBoolean("IS_DEFAULT_ITEM");
        int attack = rdr.GetInt32("ATTACK");
        int health = rdr.GetInt32("HEALTH");
        int max_health = rdr.GetInt32("MAX_HEALTH");
        int defence = rdr.GetInt32("DEFENCE");
        int rotation = rdr.GetInt32("ROTATION");
        int speed = rdr.GetInt32("SPEED");
        int visibility = rdr.GetInt32("VISIBILITY");
        int cannon_reload_speed = rdr.GetInt32("CANNON_RELOAD_SPEED");
        int crit_chance = rdr.GetInt32("CRIT_CHANCE");
        int cannon_force = rdr.GetInt32("CANNON_FORCE");
        bool stackable = rdr.GetBoolean("STACKABLE");
        int energy = rdr.GetInt32("ENERGY");
        int max_energy = rdr.GetInt32("MAX_ENERGY");
        bool overtime = rdr.GetBoolean("OVERTIME");
        float buff_duration = rdr.GetFloat("BUFF_DURATION");
        float cooldown = rdr.GetFloat("COOLDOWN");

        int drop_chance = 0;
        if (!rdr.IsDBNull(rdr.GetOrdinal("DROP_CHANCE")))
        {
            drop_chance = rdr.GetInt32("DROP_CHANCE");
        }
        float max_loot_quantity = 0;
        if (!rdr.IsDBNull(rdr.GetOrdinal("MAX_LOOT_QUANTITY")))
        {
            max_loot_quantity = rdr.GetFloat("MAX_LOOT_QUANTITY");
        }
        
        item.name = name;
        item.iconName = icon_name;
        item.isDefaultItem = is_default_item;
        item.item_type = item_type;
        item.attack = attack;
        item.health = health;
        item.max_health = max_health;
        item.defence = defence;
        item.rotation = rotation;
        item.speed = speed;
        item.visibility = visibility;
        item.cannon_reload_speed = cannon_reload_speed;
        item.crit_chance = crit_chance;
        item.cannon_force = cannon_force;
        item.stackable = stackable;
        item.energy = energy;
        item.max_energy = max_energy;
        item.overtime = overtime;
        item.buff_duration = buff_duration;
        item.cooldown = cooldown;
        item.dropChance = drop_chance;
        item.maxLootQuantity = max_loot_quantity;
        item.cooldown = cooldown;
    }

    public static bool HasColumn(MySqlDataReader r, string columnName)
    {
        try
        {
            return r.GetOrdinal(columnName) >= 0;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }
}
