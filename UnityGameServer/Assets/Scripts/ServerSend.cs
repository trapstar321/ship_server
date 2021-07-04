using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using static SpawnManager;

public class ServerSend : MonoBehaviour
{
    public float lag = 200;
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
        //_packet.WriteLength();
        byte[] data = _packet.ToArray();
        /*GameServer.server.Send(GameServer.clients[_toClient].connectionId,
            new List<AsyncTCPServer.Message>() {
                new AsyncTCPServer.Message(1, data)
        });*/
        GameServer.server.Send(GameServer.clients[_toClient].ipPort, data);        
    }

    public static void SendTCPData(List<int> clients, Packet _packet)
    {
        //_packet.WriteLength();
        foreach (int clientId in clients)
            /*GameServer.server.Send(GameServer.clients[clientId].connectionId,
            new List<AsyncTCPServer.Message>() {
                new AsyncTCPServer.Message(1, _packet.ToArray())            
            });*/
            GameServer.server.Send(GameServer.clients[clientId].ipPort, _packet.ToArray());
    }

    public static void SendTCPData(List<Player> clients, Packet _packet)
    {
        //_packet.WriteLength();
        foreach (Player player in clients) { 
            /*GameServer.server.Send(GameServer.clients[player.id].connectionId,
            new List<AsyncTCPServer.Message>() {
                new AsyncTCPServer.Message(1, _packet.ToArray())*/
            GameServer.server.Send(GameServer.clients[player.id].ipPort, _packet.ToArray());
        };        
    }
    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        //_packet.WriteLength();
        for (int i = 1; i <= GameServer.MaxPlayers; i++)
        {
            /*GameServer.server.Send(GameServer.clients[i].connectionId,
             new List<AsyncTCPServer.Message>() {
                new AsyncTCPServer.Message(1, _packet.ToArray())
            });*/
            if(GameServer.clients[i].player)
                GameServer.server.Send(GameServer.clients[i].ipPort, _packet.ToArray());
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        //_packet.WriteLength();
        for (int i = 1; i <= GameServer.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                /*GameServer.server.Send(GameServer.clients[i].connectionId,
                    new List<AsyncTCPServer.Message>() {
                    new AsyncTCPServer.Message(1, _packet.ToArray())
                });*/
                if(GameServer.clients[i].player)
                    GameServer.server.Send(GameServer.clients[i].ipPort, _packet.ToArray());
            }
        }
    }

    private static void SendTCPDataRadius(Packet _packet, Vector3 position, float sendRadius)
    {
        //_packet.WriteLength();

        for (int i = 1; i <= GameServer.MaxPlayers; i++)
        {
            if (GameServer.clients[i].player != null && GameServer.clients[i].player.playerInstance!=null)
            {
                Vector3 targetPosition;
                if (GameServer.clients[i].player.data.is_on_ship)
                {
                    targetPosition = GameServer.clients[i].player.transform.position;
                }
                else
                {
                    targetPosition = GameServer.clients[i].player.playerInstance.transform.position;
                }

                float distance = Vector3.Distance(position, targetPosition);
                if (Math.Abs(distance) < sendRadius)
                {
                    /*GameServer.server.Send(GameServer.clients[i].connectionId,
                        new List<AsyncTCPServer.Message>() {
                            new AsyncTCPServer.Message(1, _packet.ToArray())
                    });*/
                    GameServer.server.Send(GameServer.clients[i].ipPort, _packet.ToArray());
                }
            }
        }
    }

    private static void SendTCPDataRadius(int exceptClient, Packet _packet, Vector3 position, float sendRadius)
    {
        //_packet.WriteLength();

        for (int i = 1; i <= GameServer.MaxPlayers; i++)
        {
            if (GameServer.clients[i].player != null)
            {
                if (i == exceptClient)
                {
                    continue;
                }

                Vector3 targetPosition;
                if (GameServer.clients[i].player.data.is_on_ship)
                {
                    targetPosition = GameServer.clients[i].player.transform.position;
                }
                else
                {
                    targetPosition = GameServer.clients[i].player.playerInstance.transform.position;
                }

                float distance = Vector3.Distance(position, targetPosition);
                if (Math.Abs(distance) < sendRadius)
                {
                    /*GameServer.server.Send(GameServer.clients[i].connectionId,
                        new List<AsyncTCPServer.Message>() {
                            new AsyncTCPServer.Message(1, _packet.ToArray())
                    });*/
                    GameServer.server.Send(GameServer.clients[i].ipPort, _packet.ToArray());
                }
            }
        }
    }

    IEnumerator MakeLag(int to, Packet packet, float ms)
    {
        yield return new WaitForSeconds(ms / 1000);
        /*GameServer.server.Send(GameServer.clients[to].connectionId,
                        new List<AsyncTCPServer.Message>() {
                            new AsyncTCPServer.Message(1, packet.ToArray())
                    });*/
        GameServer.server.Send(GameServer.clients[to].ipPort, packet.ToArray());
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
    public static void SpawnShip(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnShip))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.data);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.playerInstance.transform.position);
            _packet.Write(_player.playerInstance.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(PlayerInputs lastInput, int lastInputSequenceNumber, Player _player, float visibilityRadius)
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

    public void NPCPosition(ShipNPC npc, float visibilityRadius)
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
        int dbid = GameServer.clients[to].player.dbid;

        using (Packet _packet = new Packet((int)ServerPackets.inventory))
        {
            List<SerializableObjects.InventorySlot> items = new List<SerializableObjects.InventorySlot>();

            foreach (InventorySlot slot in inventory.items)
            {
                items.Add(NetworkManager.SlotToSerializable(slot));
            }

            _packet.Write(items);
            _packet.Write(mysql.TotalItems(dbid));
            SendTCPData(to, _packet);
        }
    }

    public static void InventorySlot(int to, InventorySlot inventorySlot)
    {
        int dbid = GameServer.clients[to].player.dbid;

        using (Packet _packet = new Packet((int)ServerPackets.inventorySlot))
        {
            _packet.Write(NetworkManager.SlotToSerializable(inventorySlot));
            SendTCPData(to, _packet);
        }
    }

    public static void InventoryItemCount(int to)
    {
        int dbid = GameServer.clients[to].player.dbid;

        using (Packet _packet = new Packet((int)ServerPackets.inventoryItemCount))
        {
            _packet.Write(mysql.TotalItems(dbid));
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
                it.Add(NetworkManager.ItemToSerializable(i));
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
                it.Add(NetworkManager.ItemToSerializable(i));
            }

            _packet.Write(it);
            SendTCPData(to, _packet);
        }
    }

    public static void AddToInventory(int to, InventorySlot slot)
    {
        int dbid = GameServer.clients[to].player.dbid;
        using (Packet _packet = new Packet((int)ServerPackets.addToInventory))
        {
            SerializableObjects.InventorySlot sslot = NetworkManager.SlotToSerializable(slot);

            _packet.Write(sslot);
            _packet.Write(mysql.TotalItems(dbid));
            SendTCPData(to, _packet);
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
            _packet.Write((int)spawn.objectType);

            if (spawn.objectType == ObjectType.RESOURCE)
            {
                Resource resource = spawn.gameObject.GetComponent<Resource>();
                _packet.Write(resource.respawning);
                _packet.Write(resource.gatheredTime);
                _packet.Write(resource.respawnTime);
                _packet.Write((int)resource.skill_type);
            }
            else if (spawn.objectType == ObjectType.TRADER)
            {
                Trader trader = spawn.gameObject.GetComponent<Trader>();
                _packet.Write(trader.id);
            }
            else if (spawn.objectType == ObjectType.CRAFTING_SPOT)
            {
                CraftingSpot craftingSpot = spawn.gameObject.GetComponent<CraftingSpot>();
                _packet.Write((int)craftingSpot.skillType);
            }
            else if (spawn.objectType == ObjectType.NPC) {
                NPCStartParams params_ = spawn.gameObject.GetComponent<NPC>().GetStartParams();                
                _packet.Write(params_);                
            }

            SendTCPData(_toClient, _packet);
        }
    }

    public static void OnGameStart(int _toClient, List<ShipBaseStat> shipStats, List<PlayerBaseStat> playerStats, List<Experience> exp, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.onGameStart))
        {
            _packet.Write(shipStats);
            _packet.Write(playerStats);
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

            SendTCPDataRadius(_packet, pos, NetworkManager.visibilityRadius);
        }
    }

    public static void TakeDamage(int receiver, Vector3 pos, float damage, string type, bool crit)
    {
        using (Packet _packet = new Packet((int)ServerPackets.takeDamage))
        {
            _packet.Write(receiver);
            _packet.Write(type);
            _packet.Write(damage);
            _packet.Write(crit);

            SendTCPDataRadius(_packet, pos, NetworkManager.visibilityRadius);
        }
    }

    public static void CannonRotate(int from, string direction, string side)
    {
        Player player = GameServer.clients[from].player;
        using (Packet _packet = new Packet((int)ServerPackets.cannonRotate))
        {
            _packet.Write(from);
            _packet.Write(direction);
            _packet.Write(side);

            SendTCPDataRadius(from, _packet, player.transform.position, NetworkManager.visibilityRadius);
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
        Player player = GameServer.clients[from].player;
        PlayerCharacter playerCharacter = player.playerInstance.GetComponent<PlayerCharacter>();
        using (Packet _packet = new Packet((int)ServerPackets.stats))
        {
            InitalizeStatsPacket(from, _packet, player, playerCharacter);

            //SendTCPDataRadius(_packet, player.transform.position, NetworkManager.visibilityRadius);
            SendTCPDataRadius(_packet, player.transform.position, NetworkManager.visibilityRadius);
        }

        using (Packet _packet = new Packet((int)ServerPackets.stats))
        {
            InitalizeStatsPacket(from, _packet, player, playerCharacter);

            //SendTCPDataRadius(_packet, player.transform.position, NetworkManager.visibilityRadius);
            SendTCPDataRadius(_packet, player.playerInstance.transform.position, NetworkManager.visibilityRadius);
        }

        if (player.group != null)
            GroupMembers(player.group.groupId);
    }

    private static void InitalizeStatsPacket(int from, Packet _packet, Player player, PlayerCharacter playerCharacter)
    {
        ShipBaseStat shipStats = new ShipBaseStat();
        shipStats.attack = player.attack;
        shipStats.health = player.health;
        shipStats.defence = player.defence;
        shipStats.rotation = player.rotation;
        shipStats.speed = player.speed;
        shipStats.visibility = player.visibility;
        shipStats.cannon_reload_speed = player.cannon_reload_speed;
        shipStats.crit_chance = player.crit_chance;
        shipStats.cannon_force = player.cannon_force;
        shipStats.max_health = player.maxHealth;

        PlayerBaseStat playerStats = new PlayerBaseStat();
        playerStats.attack = playerCharacter.attack;
        playerStats.health = playerCharacter.health;
        playerStats.defence = playerCharacter.defence;
        playerStats.speed = playerCharacter.speed;
        playerStats.crit_chance = playerCharacter.crit_chance;
        playerStats.energy = playerCharacter.energy;
        playerStats.max_health = playerCharacter.max_health;
        playerStats.maxEnergy = playerCharacter.max_energy;

        _packet.Write(from);
        _packet.Write(shipStats);
        _packet.Write(playerStats);
    }

    public static void Stats(int from, int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.stats))
        {
            Player player = GameServer.clients[from].player;
            PlayerCharacter playerCharacter = player.playerInstance.GetComponent<PlayerCharacter>();

            ShipBaseStat shipStats = new ShipBaseStat();
            shipStats.attack = player.attack;
            shipStats.health = player.health;
            shipStats.defence = player.defence;
            shipStats.rotation = player.rotation;
            shipStats.speed = player.speed;
            shipStats.visibility = player.visibility;
            shipStats.cannon_reload_speed = player.cannon_reload_speed;
            shipStats.crit_chance = player.crit_chance;
            shipStats.cannon_force = player.cannon_force;
            shipStats.max_health = player.maxHealth;

            PlayerBaseStat playerStats = new PlayerBaseStat();
            playerStats.attack = playerCharacter.attack;
            playerStats.health = playerCharacter.health;
            playerStats.defence = playerCharacter.defence;
            playerStats.speed = playerCharacter.speed;
            playerStats.crit_chance = playerCharacter.crit_chance;
            playerStats.energy = playerCharacter.energy;
            playerStats.max_health = playerCharacter.max_health;
            playerStats.maxEnergy = playerCharacter.max_energy;

            _packet.Write(from);
            _packet.Write(shipStats);
            _packet.Write(playerStats);

            SendTCPData(to, _packet);
        }
    }

    public static void NPCStats(int from)
    {
        NPC npc = GameServer.npcs[from];
        using (Packet _packet = new Packet((int)ServerPackets.npcStats))
        {
            NPCBaseStat stats = new NPCBaseStat();
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

            //SendTCPDataRadius(_packet, player.transform.position, NetworkManager.visibilityRadius);
            SendTCPDataRadius(_packet, npc.transform.position, NetworkManager.visibilityRadius);
        }
    }

    public static void NPCStats(int from, int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcStats))
        {
            NPC npc = GameServer.npcs[from];

            NPCBaseStat stats = new NPCBaseStat();
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

    public static void BaseStats(int to, List<ShipBaseStat> stats, string type)
    {
        using (Packet _packet = new Packet((int)ServerPackets.baseStats))
        {
            //type = NPC ili player
            _packet.Write(type);
            _packet.Write(stats);

            SendTCPData(to, _packet);
        }
    }

    public static void OnLootDropped(int to, long lootId, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.onLootDropped))
        {            
            _packet.Write(lootId);
            _packet.Write(position);
            SendTCPData(to, _packet);
        }
    }

    public static void ChatMessage(int from, Message message, int to = 0)
    {
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
                if (group.owner != GameServer.clients[to].player.dbid && group.players.Count < Group.maxPlayers)
                {
                    groups.Add(new SerializableObjects.Group()
                    {
                        groupId = group.groupId,
                        name = group.groupName,
                        owner = GameServer.FindPlayerByDBid(group.owner).data.username,
                        playerCount = group.players.Count
                    });
                }
            }

            _packet.Write(groups);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerAppliedToGroup(Player from, int to)
    {
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
                foreach (int dbid in group.players)
                {
                    if (GameServer.FindPlayerByDBid(dbid) != null)
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

            foreach (Group group in NetworkManager.groups.Values)
            {
                if (group.groupId == groupId)
                {
                    myGroup = group;
                    foreach (int dbid in group.players)
                    {
                        Player player = GameServer.FindPlayerByDBid(dbid);

                        if (player != null)
                        {
                            float currentHealth, maxHealth;

                            if (player.data.is_on_ship)
                            {
                                currentHealth = player.health;
                                maxHealth = player.maxHealth;
                            }
                            else
                            {
                                currentHealth = player.playerCharacter.health;
                                maxHealth = player.playerCharacter.max_health;
                            }

                            memberData.Add(new GroupMember()
                            {
                                playerId = player.id,
                                isOwner = dbid == group.owner ? true : false,
                                online = true,
                                name = player.data.username,
                                playerLvl = player.data.level,
                                currentHealth = currentHealth,
                                maxHealth = maxHealth,
                                isOnShip = player.data.is_on_ship
                            });
                            groupMembers.Add(player);
                            sendTo.Add(player.id);
                        }
                        else
                        {
                            PlayerData data = mysql.ReadPlayerData(dbid);

                            memberData.Add(new GroupMember()
                            {
                                isOwner = dbid == group.owner ? true : false,
                                online = false,
                                name = data.username,
                                playerLvl = data.level,
                                currentHealth = 0,
                                maxHealth = 0,
                                isOnShip = data.is_on_ship
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
                    owner = GameServer.FindPlayerByDBid(myGroup.owner).data.username,
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

    public static void LeaveShip(int from, Vector3 position, float Y_rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.leaveShip))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(Y_rotation);
            _packet.Write(GameServer.clients[from].player.data);
            //SendTCPData(to, _packet);
            SendTCPDataToAll(_packet);
        }
    }

    public static void EnterShip(int from, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enterShip))
        {
            _packet.Write(from);
            //SendTCPData(to, _packet);
            SendTCPDataToAll(_packet);
        }
    }

    public static void DestroyResource(int resourceId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.destroyResource))
        {
            _packet.Write(resourceId);
            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerInputs(int from, PlayerMovement.PlayerInputs input, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerInputs))
        {
            _packet.Write(from);
            _packet.Write(input.w);
            _packet.Write(input.leftShift);
            _packet.Write(input.jump);
            _packet.Write(input.leftMouseDown);
            _packet.Write(input.move);
            SendTCPDataRadius(from, _packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void AnimationInputs(int from, CharacterAnimationController.AnimationInputs input, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.animationInputs))
        {
            _packet.Write(from);
            _packet.Write(input.w);
            _packet.Write(input.leftShift);
            _packet.Write(input.jump);
            _packet.Write(input.leftMouseDown);
            _packet.Write(input.speed);
            _packet.Write(input.horizontal);
            _packet.Write(input.currentAbility);            
            SendTCPDataRadius(from, _packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void MouseLook(int from, float x, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.mouseLook))
        {
            _packet.Write(from);
            _packet.Write(x);
            SendTCPDataRadius(from, _packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void DestroyPlayerCharacter(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.destroyPlayerCharacter))
        {
            _packet.Write(from);
            SendTCPData(to, _packet);
        }
    }

    public static void InstantiatePlayerCharacter(int to, int from, Vector3 position, float Y_rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.instantiatePlayerCharacter))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(Y_rot);
            _packet.Write(GameServer.clients[from].player.data);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerSkills(int to)
    {
        Player player = GameServer.clients[to].player;
        List<PlayerSkillLevel> pSkills = player.skills;
        List<SkillLevel> skills = NetworkManager.skillLevel;

        using (Packet _packet = new Packet((int)ServerPackets.playerSkills))
        {
            _packet.Write(pSkills);
            _packet.Write(skills);
            SendTCPData(to, _packet);
        }
    }

    public static void Recipes(int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.recipes))
        {
            _packet.Write(NetworkManager.recipes);
            SendTCPData(to, _packet);
        }
    }

    public static void CraftStatus(int to, int amount, float time, string iconName, string itemName)
    {
        using (Packet _packet = new Packet((int)ServerPackets.craftStatus))
        {
            _packet.Write(amount);
            _packet.Write(time);
            _packet.Write(iconName);
            _packet.Write(itemName);
            SendTCPData(to, _packet);
        }
    }

    public static void RequestCraftingResponse(int to, CraftingSpot craftingSpot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.requestCraftingResponse))
        {
            if (craftingSpot != null)
            {
                _packet.Write(true);
                _packet.Write((int)craftingSpot.skillType);
                _packet.Write(GameServer.clients[to].player.skills);
            }
            else
            {
                _packet.Write(false);
            }
            SendTCPData(to, _packet);
        }
    }

    public static void TraderInventory(int to, SerializableObjects.Trader trader)
    {
        using (Packet _packet = new Packet((int)ServerPackets.traderInventory))
        {
            _packet.Write(trader);
            SendTCPData(to, _packet);
        }
    }

    public static void Categories(int to, List<Category> categories)
    {
        using (Packet _packet = new Packet((int)ServerPackets.categories))
        {
            _packet.Write(categories);
            SendTCPData(to, _packet);
        }
    }

    public static void TradeBrokerItems(int to, List<TradeBrokerItem> items)
    {
        using (Packet _packet = new Packet((int)ServerPackets.tradeBrokerItems))
        {
            _packet.Write(items);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerData(int to, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerData))
        {
            _packet.Write(data);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerTrade(PlayerTrade trade, Inventory inventory)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerTrade))
        {
            _packet.Write(trade);

            List<SerializableObjects.InventorySlot> items = new List<SerializableObjects.InventorySlot>();

            foreach (InventorySlot slot in inventory.items)
            {
                if (slot.item != null)
                    items.Add(NetworkManager.SlotToSerializable(slot));
            }
            _packet.Write(items);

            SendTCPData(GameServer.FindPlayerByUsername(trade.player1.username).id, _packet);
        }
    }

    public static void PlayerTradeCanceled(int to)
    {
        Message msg = new Message();
        msg.messageType = Message.MessageType.gameInfo;
        msg.text = "Trade canceled!";
        ChatMessage(0, msg, to);

        using (Packet _packet = new Packet((int)ServerPackets.playerTradeCanceled))
        {
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerTradeClose(int to)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerTradeClose))
        {
            SendTCPData(to, _packet);
        }
    }

    public static void Parameters(int to, Parameters p)
    {
        using (Packet _packet = new Packet((int)ServerPackets.parameters))
        {
            _packet.Write(p);
            SendTCPData(to, _packet);
        }
    }

    public static void IsOnShip(int to, int playerId, bool isOnShip)
    {
        using (Packet _packet = new Packet((int)ServerPackets.isOnShip))
        {
            _packet.Write(playerId);
            _packet.Write(isOnShip);
            SendTCPData(to, _packet);
        }
    }

    public static void TargetSelected(int to, int playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.targetSelected))
        {
            _packet.Write(playerId);
            SendTCPData(to, _packet);
        }
    }

    public static void PlayerCharacterPosition(int from, Vector3 position, Quaternion rotation, Quaternion childRotation, bool except)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerCharacterPosition))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(rotation);
            _packet.Write(childRotation);

            if (except)
            {                
                SendTCPDataRadius(from, _packet, position, NetworkManager.visibilityRadius);
            }
            else
                SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void NPCPosition(int from, Vector3 position, Quaternion rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcPosition))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(rotation);            
            
            SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void ShipPosition(int from, Vector3 position, Quaternion rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.shipPosition))
        {
            _packet.Write(from);
            _packet.Write(position);
            _packet.Write(rotation);
            SendTCPDataRadius(from, _packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void Jump(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.jump))
        {
            _packet.Write(from);
            SendTCPDataRadius(from, _packet, GameServer.clients[from].player.playerInstance.transform.position, NetworkManager.visibilityRadius);
        }
    }

    public static void StartCrafting(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startCrafting))
        {
            _packet.Write(from);
            SendTCPDataRadius(from, _packet, GameServer.clients[from].player.playerInstance.transform.position, NetworkManager.visibilityRadius);
        }
    }

    public static void StopCrafting(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.stopCrafting))
        {
            _packet.Write(from);
            SendTCPDataRadius(from, _packet, GameServer.clients[from].player.playerInstance.transform.position, NetworkManager.visibilityRadius);
        }
    }

    public static void ExperienceGained(int from, int experience)
    {
        using (Packet _packet = new Packet((int)ServerPackets.experienceGained))
        {
            _packet.Write(experience);
            SendTCPData(from, _packet);
        }
    }

    public static void ActivatePlayerCharacter(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activatePlayerCharacter))
        {
            GameObject playerCharacter = GameServer.clients[from].player.playerInstance;
            _packet.Write(from);
            _packet.Write(playerCharacter.transform.position);
            _packet.Write(playerCharacter.transform.rotation);
            SendTCPData(to, _packet);
        }
    }

    public static void DeactivatePlayerCharacter(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.deactivatePlayerCharacter))
        {
            _packet.Write(from);
            SendTCPData(to, _packet);
        }
    }

    public static void ActivateShip(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activateShip))
        {
            GameObject playerCharacter = GameServer.clients[from].player.gameObject;
            _packet.Write(from);
            _packet.Write(playerCharacter.transform.position);
            _packet.Write(playerCharacter.transform.rotation);
            SendTCPData(to, _packet);
        }
    }

    public static void DeactivateShip(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.deactivateShip))
        {
            _packet.Write(from);
            SendTCPData(to, _packet);
        }
    }

    public static void ActivateNPC(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activateNPC))
        {
            GameObject npc = GameServer.npcs[from].gameObject;
            _packet.Write(from);
            _packet.Write(npc.transform.position);
            _packet.Write(npc.transform.rotation);
            SendTCPData(to, _packet);
        }
    }

    public static void DeactivateNPC(int to, int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.deactivateNPC))
        {
            _packet.Write(from);
            SendTCPData(to, _packet);
        }
    }
    public static void DiePlayerCharacter(int from, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.diePlayerCharacter))
        {
            _packet.Write(from);
            _packet.Write(data);
            SendTCPDataToAll(_packet);
        }
    }

    public static void DieNPC(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.dieNPC))
        {
            _packet.Write(from);            
            SendTCPDataToAll(_packet);
        }
    }

    public static void RespawnPlayerCharacter(int from, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.respawnPlayerCharacter))
        {
            _packet.Write(from);
            _packet.Write(NetworkManager.instance.respawnPointCharacter.transform.position);
            _packet.Write(data);
            SendTCPDataToAll(_packet);
        }
    }

    public static void RespawnNPC(int from)
    {
        using (Packet _packet = new Packet((int)ServerPackets.respawnNPC))
        {
            _packet.Write(from);            
            SendTCPDataToAll(_packet);
        }
    }

    public static void DieShip(int from, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.dieShip))
        {
            _packet.Write(from);
            _packet.Write(data);
            SendTCPDataToAll(_packet);
        }
    }

    public static void RespawnShip(int from, PlayerData data)
    {
        using (Packet _packet = new Packet((int)ServerPackets.respawnShip))
        {
            _packet.Write(from);
            _packet.Write(NetworkManager.instance.respawnPointShip.transform.position);
            _packet.Write(data);
            SendTCPDataToAll(_packet);
        }
    }

    public static void SendTradeRequest(Player player, Player sender)
    {
        if (player != null)
        {
            string acceptLink = "trade_accept" + System.Guid.NewGuid().ToString();
            string declineLink = "trade_decline" + System.Guid.NewGuid().ToString();

            PlayerTrade trade = new PlayerTrade();
            trade.player1 = sender.data;
            trade.player2 = player.data;

            NetworkManager.tradeLinks.Add(acceptLink, trade);
            NetworkManager.tradeLinks.Add(declineLink, trade);

            string accept = $"<link={acceptLink}><color=green><u>Accept</u></color></link>";
            string decline = $"<link={declineLink}><color=#FF0006><u>Decline</u></color></link>";

            Message msg = new Message();
            msg.messageType = Message.MessageType.privateMessage;
            msg.text = $"{accept} or {decline} trade request from {sender.data.username}";
            ServerSend.ChatMessage(sender.id, msg, player.id);
        }
    }

    public static void ActivatePlayerMovement(int from, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activatePlayerMovement))
        {
            _packet.Write(from);
            SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void DeactivatePlayerMovement(int from, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.deactivatePlayerMovement))
        {
            _packet.Write(from);
            SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void PlayerAbilities(int to, Dictionary<string, PlayerAbility> abilities)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerAbilities))
        {
            _packet.Write(abilities);
            SendTCPData(to, _packet);
        }
    }

    public static void BuffAdded(int from, Vector3 position, Buff buff, Item item)
    {
        using (Packet _packet = new Packet((int)ServerPackets.buffAdded))
        {
            _packet.Write(from);
            _packet.Write(NetworkManager.ItemToSerializable(item));
            _packet.Write(buff);
            SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void Buffs(int from, int to)
    {
        List<Buff> buffs = GameServer.clients[from].player.playerCharacter.buffManager.buffs;       
        
        using (Packet _packet = new Packet((int)ServerPackets.buffs))
        {
            _packet.Write(from);
            _packet.Write(buffs);
            SendTCPData(to, _packet);
        }
    }

    public static void NPCSwitchState(int from, int state, int objectType, Vector3 position, float visibilityRadius) {
        using (Packet _packet = new Packet((int)ServerPackets.npcSwitchState))
        {
            _packet.Write(from);
            _packet.Write(state);
            _packet.Write(objectType);
            SendTCPDataRadius(_packet, position, visibilityRadius);
        }
    }

    public static void NPCDoAbility(int from, int ability, int objectType, Vector3 position, float visibilityRadius)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcDoAbility))
        {
            _packet.Write(from);
            _packet.Write(ability);
            _packet.Write(objectType);
            SendTCPDataRadius(_packet, position, visibilityRadius);
        }
    }

    public static void NPCTarget(int from, int target, int objectType, Vector3 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.npcTarget))
        {
            _packet.Write(from);            
            _packet.Write(target);
            _packet.Write(objectType);

            SendTCPDataRadius(_packet, position, NetworkManager.visibilityRadius);
        }
    }

    public static void CorrectState(int to, Vector3 position, long ticks) {
        using (Packet _packet = new Packet((int)ServerPackets.correctState)) {
            _packet.Write(position);
            _packet.Write(ticks);
            SendTCPData(to, _packet);
        }
    }

    public static void Ping(int to) {
        using (Packet _packet = new Packet((int)ServerPackets.ping))
        {
            _packet.Write(DateTime.UtcNow);            
            SendTCPData(to, _packet);
        }
    }

    public static void LoadLoot(int to, long lootId, List<SerializableObjects.ItemDrop> drop) {
        using (Packet _packet = new Packet((int)ServerPackets.loadLoot))
        {
            _packet.Write(lootId);
            _packet.Write(drop);
            SendTCPData(to, _packet);
        }
    }

    public static void RemoveLoot(int to, long lootId) {
        using (Packet _packet = new Packet((int)ServerPackets.removeLoot))
        {
            _packet.Write(lootId);
            SendTCPData(to, _packet);
        }
    }
}
#endregion