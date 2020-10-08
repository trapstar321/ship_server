using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
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

    private static void SendTCPDataRadius(Packet _packet, Vector3 position, float sendRadius)
    {
        _packet.WriteLength();

        for (int i = 1; i <= Server.MaxPlayers; i++)
        {            
            if (Server.clients[i].player != null)
            {
                float distance = Vector3.Distance(position, Server.clients[i].player.transform.position);
                if (Math.Abs(Vector3.Distance(position, Server.clients[i].player.transform.position)) < sendRadius)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }            
        }
    }

    private static void SendTCPDataRadius(int _exceptClient, Packet _packet, Vector3 position, float visibilityRadius) {
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
    public static void PlayerPosition(Player _player, float visibilityRadius)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            _packet.Write(_player.transform.rotation);

            //SendTCPDataToAll(_player.id, _packet);
            SendTCPDataRadius(_player.id, _packet, _player.transform.position, visibilityRadius);
        }
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

    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasItem);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ItemSpawned(int _spawnerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
        {
            _packet.Write(_spawnerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPickedUp(int _spawnerId, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectile(Projectile _projectile, int _thrownByPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_thrownByPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void ProjectileExploded(Projectile _projectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnemy(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToAll(SpawnEnemy_Data(_enemy, _packet));
        }
    }
    public static void SpawnEnemy(int _toClient, Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPData(_toClient, SpawnEnemy_Data(_enemy, _packet));
        }
    }

    private static Packet SpawnEnemy_Data(Enemy _enemy, Packet _packet)
    {
        _packet.Write(_enemy.id);
        _packet.Write(_enemy.transform.position);
        return _packet;
    }

    public static void EnemyPosition(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyHealth(Enemy _enemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
        {
            _packet.Write(_enemy.id);
            _packet.Write(_enemy.health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void WavesMesh(int to, MeshSerializable.MeshSerializable mesh)
    {        
        using (Packet _packet = new Packet((int)ServerPackets.wavesMesh))
        {
            _packet.Write(mesh);
            SendTCPData(to, _packet);            
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
                crit_chance = slot.item.crit_chance 
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
            crit_chance = item.crit_chance
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
    #endregion
}
