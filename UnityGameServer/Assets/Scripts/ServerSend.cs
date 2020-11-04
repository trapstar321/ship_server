using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend: MonoBehaviour
{
    public float lag=200;
    
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>    
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }
    
    private void SendTCPDataRadius(Packet _packet, Vector3 position, float sendRadius)
    {
        _packet.WriteLength();        

        for (int i = 1; i <= Server.MaxPlayers; i++)
        {            
            if (Server.clients[i].player != null)
            {
                float distance = Vector3.Distance(position, Server.clients[i].player.transform.position);
                if (Math.Abs(Vector3.Distance(position, Server.clients[i].player.transform.position)) < sendRadius)
                {
                    StartCoroutine(MakeLag(i, _packet, lag));
                    //Server.clients[i].tcp.SendData(_packet);
                }
            }            
        }
    }

    IEnumerator MakeLag(int to, Packet packet, float ms) {        
        yield return new WaitForSeconds(ms/1000);
        Server.clients[to].tcp.SendData(packet);
    }

    private void SendTCPDataRadius(int _exceptClient, Packet _packet, Vector3 position, float visibilityRadius) {
        _packet.WriteLength();

        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                if (Server.clients[i].player != null)
                {
                    float distance = Vector3.Distance(position, Server.clients[i].player.transform.position);
                    if (Math.Abs(Vector3.Distance(position, Server.clients[i].player.transform.position)) < visibilityRadius)
                    {
                        Server.clients[i].tcp.SendData(_packet);
                    }
                }
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public void PlayerPosition(PlayerInputs lastInput, int lastInputSequenceNumber, Player _player, float visibilityRadius)
    {
        //using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        //{
        Packet _packet = new Packet((int)ServerPackets.playerPosition);
        _packet.Write(_player.id);
        _packet.Write(lastInput.left);
        _packet.Write(lastInput.right);
        _packet.Write(lastInput.forward);
        _packet.Write(lastInputSequenceNumber);
        _packet.Write(_player.transform.position);
        _packet.Write(_player.transform.rotation);

        //outputBuffer.Add(_packet);
        //SendTCPDataToAll(_packet);
        SendTCPDataRadius(_packet, _player.transform.position, visibilityRadius);
        //}
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Inventory(int to, Inventory inventory)
    {
        using (Packet _packet = new Packet((int)ServerPackets.inventory))
        {
            List<SerializableObjects.InventorySlot> items = new List<SerializableObjects.InventorySlot>();

            foreach (InventorySlot slot in inventory.items) {
                items.Add(SlotToSerializable(slot)) ;
            }

            _packet.Write(items);
            SendTCPData(to, _packet);
        }
    }

    public static void ShipEquipment(int to, List<Item> items)
    {
        using (Packet _packet = new Packet((int)ServerPackets.shipEquipment))
        {
            List<SerializableObjects.Item> it = new List<SerializableObjects.Item>();

            foreach (Item i in items)
            {
                it.Add(ItemToSerializable(i));
            }

            _packet.Write(it);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerEquipment(int to, List<Item> items)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerEquipment))
        {
            List<SerializableObjects.Item> it = new List<SerializableObjects.Item>();

            foreach (Item i in items)
            {
                it.Add(ItemToSerializable(i));
            }

            _packet.Write(it);
            SendTCPData(to, _packet);
        }
    }

    public static void AddToInventory(int to, InventorySlot slot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addToInventory))
        {
            SerializableObjects.InventorySlot sslot = SlotToSerializable(slot);

            _packet.Write(sslot);
            SendTCPData(to, _packet);
        }
    }

    protected static SerializableObjects.InventorySlot SlotToSerializable(InventorySlot slot) {
        SerializableObjects.Item item = null;

        if (slot.item != null)
        {
            item = new SerializableObjects.Item()
            {
                id = slot.item.id,
                item_id = slot.item.item_id,
                iconName = slot.item.iconName,
                isDefaultItem = slot.item.isDefaultItem,
                name = slot.item.name,
                item_type = slot.item.item_type,
                attack = slot.item.attack,
                health = slot.item.health,
                defence = slot.item.defence,
                speed = slot.item.speed,
                visibility = slot.item.visibility,
                rotation = slot.item.rotation,
                cannon_reload_speed = slot.item.cannon_reload_speed,
                crit_chance = slot.item.crit_chance,
                cannon_force = slot.item.cannon_force
            };
        }

        return new SerializableObjects.InventorySlot()
        {
            slotID = slot.slotID,
            quantity = slot.quantity,
            item = item
        };
    }

    protected static SerializableObjects.Item ItemToSerializable(Item item)
    {
        return new SerializableObjects.Item()
        {
            id = item.id,
            item_id = item.item_id,
            iconName = item.iconName,
            isDefaultItem = item.isDefaultItem,
            name = item.name,
            item_type = item.item_type,
            attack = item.attack,
            health = item.health,
            defence = item.defence,
            speed = item.speed,
            visibility = item.visibility,
            rotation = item.rotation,
            cannon_reload_speed = item.cannon_reload_speed,
            crit_chance = item.crit_chance,
            cannon_force = item.cannon_force
        };
    }

    public static void Time(float time)
    {
        using (Packet _packet = new Packet((int)ServerPackets.time))
        {
            _packet.Write(time);
            SendUDPDataToAll(_packet);
        }
    }

    public static void SpawnGameObject(int _toClient, SpawnManager.Spawn spawn)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnGameObject))
        {
            _packet.Write(spawn.id);
            _packet.Write(Convert.ToInt32(spawn.type));
            _packet.Write(spawn.gameObject.transform.position);
            _packet.Write(spawn.gameObject.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void OnGameStart(int _toClient, List<BaseStat> stats, List<Experience> exp, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.onGameStart))
        {
            _packet.Write(stats);
            _packet.Write(exp);
            _packet.Write(data);            

            SendTCPData(_toClient, _packet);
        }
    }

    public void Shoot(int from, string position, Vector3 pos)
    {
        using (Packet _packet = new Packet((int)ServerPackets.shoot))
        {
            _packet.Write(from);
            _packet.Write(position);

            SendTCPDataRadius(from, _packet, pos, NetworkManager.visibilityRadius);
        }
    }

    public static void TakeDamage(int receiver, Vector3 pos, float damage)
    {
        using (Packet _packet = new Packet((int)ServerPackets.takeDamage))
        {
            _packet.Write(receiver);
            _packet.Write(damage);

            SendTCPDataToAll(_packet);
        }
    }

    public static void CannonRotate(int from, string direction, string side)
    {
        using (Packet _packet = new Packet((int)ServerPackets.cannonRotate))
        {
            _packet.Write(from);
            _packet.Write(direction);
            _packet.Write(side);
			
			SendTCPDataToAll(from, _packet);
		}
	}
	
    public static void HealthStats(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.healthStat))
        {
            float health = Server.clients[from].player.health;
            float maxHealth = Server.clients[from].player.maxHealth;
            _packet.Write(from);
            _packet.Write(health);
            _packet.Write(maxHealth);


            SendTCPDataToAll(from, _packet);
        }
    }

    public static void HealthStats(int from, int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.healthStat))
        {
            float health = Server.clients[from].player.health;
            float maxHealth = Server.clients[from].player.maxHealth;
            _packet.Write(from);
            _packet.Write(health);
            _packet.Write(maxHealth);

            SendTCPData(to, _packet);
        }
    }
}
#endregion