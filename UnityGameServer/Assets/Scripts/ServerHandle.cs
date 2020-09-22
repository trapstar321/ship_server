using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle: MonoBehaviour
{
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

        ServerSend.WavesMesh(_fromClient, NetworkManager.wavesScript.GenerateMesh());
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

    public static void DropItem(int from, Packet packet) {
        Mysql mysql = FindObjectOfType<Mysql>();
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();

        inventory.Remove(slot.slotID);        

        InventorySlot s = SlotFromSerializable(slot);
        mysql.DropItem(from, s);
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

    protected static InventorySlot SlotFromSerializable(SerializableObjects.InventorySlot slot) {
        InventorySlot s = new InventorySlot();

        if (slot.item != null)
        {
            Item item = new Item();
            item.id = slot.item.id;
            item.name = slot.item.name;
            s.item = item;
        }

        s.quantity = slot.quantity;
        s.slotID = slot.slotID;

        return s;
    }

    public static void SearchChest(int from, Packet packet) {
        Server.clients[from].player.SearchChest();
    }
}
