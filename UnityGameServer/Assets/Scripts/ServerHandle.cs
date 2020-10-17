using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle: MonoBehaviour
{
    private static SpawnManager spawnManager;

    private void Awake()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        //string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame("username");
        //ServerSend.WavesMesh(_fromClient, NetworkManager.wavesScript.GenerateMesh());
        spawnManager.SendAllGameObjects(_fromClient);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();            
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    public static void Position(int _fromClient, Packet _packet)
    {
        Vector3 position = _packet.ReadVector3();
        Quaternion rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetPosition(position, rotation);
    }

    public static void Joystick(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        float vertical = _packet.ReadFloat();
        float horizontal = _packet.ReadFloat();

        Server.clients[_fromClient].player.SetInput(vertical, horizontal);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void test(int _fromClient, Packet _packet)
    {
        string data =_packet.ReadString();
        Debug.Log(data);
    }

    public static void GetInventory(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        List<InventorySlot> slots = mysql.ReadInventory(from);
        Inventory inventory = Server.clients[from].player.inventory;

        foreach (InventorySlot s in slots) {
            inventory.Add(s);    
        }
        
        ServerSend.Inventory(from, inventory);

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void GetShipEquipment(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        List<Item> items = mysql.ReadShipEquipment(from);
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;

        foreach (Item item in items)
        {
            equipment.Add(item);
        }

        ServerSend.ShipEquipment(from, items);

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void GetPlayerEquipment(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        List<Item> items = mysql.ReadPlayerEquipment(from);
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;

        foreach (Item item in items)
        {
            equipment.Add(item);
        }

        ServerSend.PlayerEquipment(from, items);

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void DropItem(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();

        inventory.Remove(slot.slotID);        

        InventorySlot s = SlotFromSerializable(slot);
        mysql.DropItem(from, s);
    }

    public static void AddItemToInventory(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment sequipment = Server.clients[from].player.ship_equipment;
        PlayerEquipment pequipment = Server.clients[from].player.player_equipment;

        string eq = packet.ReadString();
        SerializableObjects.Item item = packet.ReadItem();

        Item it = null;

        if(eq.Equals("ship_equipment"))
            it = sequipment.GetItem(item.item_type);
        else if(eq.Equals("player_equipment"))
            it = pequipment.GetItem(item.item_type);

        InventorySlot sl = inventory.Add(it);
        mysql.AddItemToInventory(from, sl);
    }

    public static void RemoveItemFromInventory(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;

        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();
        mysql.RemoveInventoryItem(from, slot.slotID);
        inventory.Remove(slot.slotID);
    }

    public static void ReplaceShipEquipment(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);
        
        mysql.RemoveInventoryItem(from, slot.slotID);
        mysql.AddShipEquipment(from, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(from, slot);
        }
        equipment.Add(new_);        
    }

    public static void ReplacePlayerEquipment(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);

        mysql.RemoveInventoryItem(from, slot.slotID);
        mysql.AddPlayerEquipment(from, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(from, slot);
        }
        equipment.Add(new_);
    }

    public static void DragAndDrop(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot1 = packet.ReadInventorySlot();
        SerializableObjects.InventorySlot slot2 = packet.ReadInventorySlot();        

        InventorySlot s1 = SlotFromSerializable(slot1);
        InventorySlot s2 = SlotFromSerializable(slot2);

        InventorySlot slot_1 = inventory.FindSlot(s1.slotID);
        InventorySlot slot_2 = inventory.FindSlot(s2.slotID);        

        if (slot_1.item != null && slot_2.item != null)
        {
            if (slot_1.item.item_id == slot_2.item.item_id)
            {
                mysql.DragAndDrop_Stack(from, slot_1, slot_2);
            }
            else
            {
                mysql.DragAndDrop_Change(from, slot_1, slot_2);
            }
        }
        else if (slot_1.item == null || slot_2.item == null)
            mysql.DragAndDrop_Move(from, slot_1, slot_2);

        inventory.DragAndDrop(slot_1, slot_2);
    }

    public static void AddShipEquipment(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();

        InventorySlot sl = SlotFromSerializable(slot);
        sl = inventory.FindSlot(sl.slotID);

        if (sl.item != null)
        {
            equipment.Add(sl.item);
            mysql.AddShipEquipment(from, sl.item);
        }
    }

    public static void RemoveShipEquipment(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.Item item = packet.ReadItem();

        Item it = ItemFromSerializable(item);        

        if (it != null) {
            equipment.Remove(it);
            mysql.RemoveShipEquipment(from, it);
        }
    }

    public static void RemovePlayerEquipment(int from, Packet packet)
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.Item item = packet.ReadItem();

        Item it = ItemFromSerializable(item);

        if (it != null)
        {
            equipment.Remove(it);
            mysql.RemovePlayerEquipment(from, it);
        }
    }

    protected static Item ItemFromSerializable(SerializableObjects.Item it)
    {        
        Item item = new Item();
        item.id = it.id;
        item.name = it.name;
        item.item_type = it.item_type;           

        return item;
    }

    protected static InventorySlot SlotFromSerializable(SerializableObjects.InventorySlot slot)
    {
        InventorySlot s = new InventorySlot();

        if (slot.item != null)
        {
            Item item = new Item();
            item.id = slot.item.id;
            item.name = slot.item.name;
            item.item_type = slot.item.item_type;
            s.item = item;            
        }

        s.quantity = slot.quantity;
        s.slotID = slot.slotID;

        return s;
    }

    public static void SearchChest(int from, Packet packet) {
        Server.clients[from].player.SearchChest();
    }

    public static void OnGameStart(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();

        List<BaseStat> stats = mysql.ReadBaseStatsTable();
        List<Experience> exp = mysql.ReadExperienceTable();
        PlayerData data = mysql.ReadPlayerData(from);        

        ServerSend.OnGameStart(from, stats, exp, data);

        Player player = Server.clients[from].player;
        player.OnGameStart(stats, exp, data);
    }

    public static void Shoot(int from, Packet packet) {
        CannonShot shootScript = Server.clients[from].player.GetComponent<CannonShot>();
        shootScript.Shoot(packet.ReadString());
    }

    public static void CannonRotate(int from, Packet packet)
    {
        CannonController cannonRotate = Server.clients[from].player.GetComponent<CannonController>();
        cannonRotate.CannonRotate(packet.ReadString(), packet.ReadString());
    }
}
