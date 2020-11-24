﻿using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend: MonoBehaviour
{
    public float lag=200;
    private static Mysql mysql;

    private void Awake()
    {
        mysql = FindObjectOfType<Mysql>();
    }

    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>    
    public static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    public static void SendTCPData(List<int> clients, Packet _packet) {
        _packet.WriteLength();
        foreach(int clientId in clients)
            Server.clients[clientId].tcp.SendData(_packet);
    }

    public static void SendTCPData(List<Player> clients, Packet _packet)
    {
        _packet.WriteLength();
        foreach (Player player in clients)
            Server.clients[player.id].tcp.SendData(_packet);
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
    
    private static void SendTCPDataRadiusStatic(Packet _packet, Vector3 position, float sendRadius)
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

    IEnumerator MakeLag(int to, Packet packet, float ms) {        
        yield return new WaitForSeconds(ms/1000);
        Server.clients[to].tcp.SendData(packet);
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

    private static void SendTCPDataRadiusStatic(int _exceptClient, Packet _packet, Vector3 position, float visibilityRadius)
    {
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
    public static void Hello(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.hello))
        {
            SendTCPData(_toClient, _packet);
        }
    }

    public static void Welcome(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void LoginFailed(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.loginFailed))
        {            
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
        _packet.Write(_player.transform.localRotation);

        //outputBuffer.Add(_packet);
        //SendTCPDataToAll(_packet);
        SendTCPDataRadius(_packet, _player.transform.position, visibilityRadius);
        //}
    }

    public void NPCPosition(EnemyAI npc, float visibilityRadius)
    {
        //using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        //{
        Packet _packet = new Packet((int)ServerPackets.npcPosition);
        _packet.Write(npc.id);        
        _packet.Write(npc.transform.position);
        _packet.Write(npc.transform.localRotation);
        SendTCPDataRadius(_packet, npc.transform.position, visibilityRadius);
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

    protected static SerializableObjects.ItemDrop ItemDropToSerializable(ItemDrop drop)
    {
        SerializableObjects.Item item = ItemToSerializable(drop.item);

        return new SerializableObjects.ItemDrop() { quantity = drop.quantity, item = item };
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

    public static void NPCShoot(int from, string position, Vector3 pos, Vector3 angle)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcShoot))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(angle);

            SendTCPDataRadiusStatic(_packet, pos, NetworkManager.visibilityRadius);
        }
    }

    public static void TakeDamage(int receiver, Vector3 pos, float damage, string type)
    {
        using (Packet _packet = new Packet((int)ServerPackets.takeDamage))
        {
            _packet.Write(receiver);
            _packet.Write(type);
            _packet.Write(damage);

            SendTCPDataRadiusStatic(_packet, pos, NetworkManager.visibilityRadius);
        }
    }

    public static void CannonRotate(int from, string direction, string side)
    {
        Player player = Server.clients[from].player;
        using (Packet _packet = new Packet((int)ServerPackets.cannonRotate))
        {
            _packet.Write(from);
            _packet.Write(direction);
            _packet.Write(side);
			
			SendTCPDataRadiusStatic(from, _packet, player.transform.position, NetworkManager.visibilityRadius);
		}
	}

    public static void CannonRotate(int from, int to, Quaternion rotation, string side)
    {
        using (Packet _packet = new Packet((int)ServerPackets.cannonRotateAngle))
        {
            _packet.Write(from);
            _packet.Write(rotation);
            _packet.Write(side);

            SendTCPData(to, _packet);
        }
    }

    public static void Stats(int from)
    {
        Player player = Server.clients[from].player;
        using (Packet _packet = new Packet((int)ServerPackets.stats))
        {
            BaseStat stats = new BaseStat();
            stats.attack = player.attack;
            stats.health = player.health;
            stats.defence = player.defence;
            stats.rotation = player.rotation;
            stats.speed = player.speed;
            stats.visibility = player.visibility;
            stats.cannon_reload_speed = player.cannon_reload_speed;
            stats.crit_chance = player.crit_chance;
            stats.cannon_force = player.cannon_force;
            stats.max_health = player.maxHealth;

            _packet.Write(from);
            _packet.Write(stats);

            SendTCPDataRadiusStatic(_packet, player.transform.position, NetworkManager.visibilityRadius);
        }

        if(player.group!=null)
            GroupMembers(player.group.groupId);
    }

    public static void Stats(int from, int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.stats))
        {
            Player player = Server.clients[from].player;

            BaseStat stats = new BaseStat();
            stats.attack = player.attack;
            stats.health = player.health;
            stats.defence = player.defence;
            stats.rotation = player.rotation;
            stats.speed = player.speed;
            stats.visibility = player.visibility;
            stats.cannon_reload_speed = player.cannon_reload_speed;
            stats.crit_chance = player.crit_chance;
            stats.cannon_force = player.cannon_force;
            stats.max_health = player.maxHealth;

            _packet.Write(from);
            _packet.Write(stats);

            SendTCPData(to, _packet);
        }
    }

    public static void NPCStats(int from, int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcStats))
        {
            EnemyAI npc = Server.npcs[from].GetComponent<EnemyAI>();

            BaseStat stats = new BaseStat();
            stats.attack = npc.attack;
            stats.health = npc.health;
            stats.defence = npc.defence;
            stats.rotation = npc.rotation;
            stats.speed = npc.speed;
            stats.visibility = npc.visibility;
            stats.cannon_reload_speed = npc.cannon_reload_speed;
            stats.crit_chance = npc.crit_chance;
            stats.cannon_force = npc.cannon_force;
            stats.max_health = npc.maxHealth;

            _packet.Write(from);
            _packet.Write(stats);

            SendTCPData(to, _packet);
        }
    }

    public static void BaseStats(int to, List<BaseStat> stats, string type) {
        using (Packet _packet = new Packet((int)ServerPackets.baseStats))
        {
            //type = NPC ili player
            _packet.Write(type);
            _packet.Write(stats);            

            SendTCPData(to, _packet);
        }
    }

    public static void OnLootDropped(int to, List<ItemDrop> items) {
        List<SerializableObjects.ItemDrop> toSend = new List<SerializableObjects.ItemDrop>();

        foreach (ItemDrop item in items) {
            toSend.Add(ItemDropToSerializable(item));
        }

        using (Packet _packet = new Packet((int)ServerPackets.onLootDropped))
        {
            _packet.Write(toSend);
            SendTCPData(to, _packet);
        }
    }

    public static void ChatMessage(int from, Message message, int to=0) {
        using (Packet _packet = new Packet((int)ServerPackets.chatMessage))
        {
            _packet.Write(message);

            if (to != 0)
            {
                SendTCPData(to, _packet);
            }
            else
            {
                SendTCPDataToAll(from, _packet);
            }
        }
    }

    public static void OnGameMessage(int to, Message message)
    {
        using (Packet _packet = new Packet((int)ServerPackets.onGameMessage))
        {
            _packet.Write(message);
            SendTCPData(to, _packet);
        }
    }

    public static void GroupCreateStatus(int to, bool status)
    {
        using (Packet _packet = new Packet((int)ServerPackets.groupCreateStatus))
        {
            _packet.Write(status);
            SendTCPData(to, _packet);
        }
    }

    public static void GroupList(int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.groupList))
        {
            List<SerializableObjects.Group> groups = new List<SerializableObjects.Group>();
            foreach (Group group in NetworkManager.groups.Values)
            {
                //dont send owned group
                if (group.owner != Server.clients[to].player.dbid && group.players.Count<Group.maxPlayers)
                {
                    groups.Add(new SerializableObjects.Group()
                    {
                        groupId = group.groupId,
                        name = group.groupName,
                        owner = Server.FindPlayerByDBid(group.owner).data.username,
                        playerCount = group.players.Count
                    });
                }
            }

            _packet.Write(groups);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerAppliedToGroup(Player from, int to) {
        using (Packet _packet = new Packet((int)ServerPackets.playerAppliedToGroup))
        {
            _packet.Write(from.id);
            _packet.Write(from.data.level);
            _packet.Write(from.data.username);
            SendTCPData(to, _packet);
        }
    }

    public static void GroupMembers(int groupId)
    {
        bool anyMemberOnline = false;
        foreach (Group group in NetworkManager.groups.Values)
        {
            if (group.groupId == groupId)
            {
                foreach (int dbid in group.players) {
                    if (Server.FindPlayerByDBid(dbid) != null)
                    {
                        anyMemberOnline = true;
                        break;
                    }
                }
                break;
            }
        }

        if (!anyMemberOnline)
            return;

        using (Packet _packet = new Packet((int)ServerPackets.groupMembers))
        {
            List<GroupMember> memberData = new List<GroupMember>();
            List<Player> groupMembers = new List<Player>();
            List<int> sendTo = new List<int>();
            Group myGroup = null;

            foreach (Group group in NetworkManager.groups.Values) {
                if (group.groupId == groupId) {
                    myGroup = group;
                    foreach (int dbid in group.players) {
                        Player player = Server.FindPlayerByDBid(dbid);

                        if (player != null)
                        {
                            memberData.Add(new GroupMember()
                            {
                                playerId = player.id,
                                isOwner = dbid==group.owner?true:false, 
                                online = true,
                                name = player.data.username,
                                playerLvl = player.data.level,
                                currentHealth = player.health,
                                maxHealth = player.maxHealth
                            });
                            groupMembers.Add(player);
                            sendTo.Add(player.id);
                        }
                        else {
                            PlayerData data = mysql.ReadPlayerData(dbid);

                            memberData.Add(new GroupMember()
                            {
                                isOwner = dbid == group.owner ? true : false,
                                online = false,
                                name = data.username,
                                playerLvl = data.level,
                                currentHealth = 0,
                                maxHealth = 0
                            });
                            groupMembers.Add(player);
                        }
                    }
                    break;
                }
            }            

            _packet.Write(memberData);
            if (myGroup != null)
            {
                _packet.Write(new SerializableObjects.Group()
                {
                    groupId = myGroup.groupId,
                    name = myGroup.groupName,
                    owner = Server.FindPlayerByDBid(myGroup.owner).data.username,
                    playerCount = myGroup.players.Count
                });
            }

            SendTCPData(sendTo, _packet);
        }
    }

    public static void KickedFromGroup(int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.kickedFromGroup))
        {            
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerList(int to, List<PlayerData> data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerList))
        {
            _packet.Write(data);
            SendTCPData(to, _packet);
        }
    }
}
#endregion