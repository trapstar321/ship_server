using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    public static ServerSend send;
    public static NetworkManager instance;
    public static Waves wavesScript;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    public static float visibilityRadius = 60;
    
    float positionAndRotationTick = 25;
    float playerPositionUpdateTick = 5;
    float respawnTradersTick = 10;

    Mysql mysql;
    public GameObject respawnPointCharacter;
    public GameObject respawnPointShip;

    public static Dictionary<int, Group> groups = new Dictionary<int, Group>();
    public static Dictionary<string, int> invitationLinks = new Dictionary<string, int>();
    public static List<SkillLevel> skillLevel;
    public static List<Recipe> recipes;
    public static Dictionary<int, List<SerializableObjects.Trader>> traders = new Dictionary<int, List<SerializableObjects.Trader>>();
    public static Dictionary<string, PlayerTrade> tradeLinks = new Dictionary<string, PlayerTrade>();
    public static Dictionary<int, PlayerTrade> trades = new Dictionary<int, PlayerTrade>();
    public static Dictionary<string, PlayerAbility> playerAbilities = new Dictionary<string, PlayerAbility>() {
        { "RollLeft", new PlayerAbility(){ multiplier=0f, abilityName="RollLeft", energy = 80} },
        { "RollRight", new PlayerAbility(){ multiplier=0f, abilityName="RollRight", energy = 80} },
        { "DSA_Top", new PlayerAbility(){ multiplier=2f, abilityName="DSA_Top", energy = 50} },
        { "DSA_Long", new PlayerAbility(){ multiplier=1.5f, abilityName="DSA_Long", energy = 30} },
        { "Stab", new PlayerAbility(){ multiplier=1f, abilityName="Stab", energy=10} },
        { "RollForward", new PlayerAbility(){ multiplier=0f, abilityName="RollForward", energy=80} }
    };

    public static float buffCheckPeriod = 1f;
    public static float energyGainPeriod = 1f;
    public static float energyGainAmount = 10f;

    public class PacketData {
        public int type;
        public Packet packet;
    }

    public static Dictionary<int, List<PacketData>> buffer = new Dictionary<int, List<PacketData>>();

    public static string[] item_buff_properties = new string[] {
        "attack", "health", "defence", "energy", "rotation", "speed",
        "visibility", "cannon_reload_speed", "crit_chance", "cannon_force",
        "max_health", "max_energy"
    };

    public static string[] ship_buff_properties = new string[] {
        "rotation", "speed", "visibility", "cannon_reload_speed", "crit_chance", "cannon_force"
    };

    public static string[] player_buff_properties = new string[] {
        "attack", "health", "defence", "energy", "speed",
        "crit_chance", "max_health", "max_energy"
    };

    public static Parameters parameters = new Parameters()
    {
        visibilityRadius = NetworkManager.visibilityRadius,
        inventorySize = Inventory.space,
        energyGainAmount = NetworkManager.energyGainAmount,
        buffCheckPeriod = NetworkManager.buffCheckPeriod,
        energyGainPeriod = NetworkManager.energyGainPeriod,
        item_buff_properties = NetworkManager.item_buff_properties,
        ship_buff_properties = NetworkManager.ship_buff_properties,
        player_buff_properties = NetworkManager.player_buff_properties
    };

    private void Awake()
    {
        send = FindObjectOfType<ServerSend>();
        StartCoroutine(Tick());
        StartCoroutine(NPCPositionAndRotationTick());
        StartCoroutine(Respawn());
        StartCoroutine(UpdatePlayerPosition());
        StartCoroutine(RespawnTraders());
        wavesScript = GameObject.FindWithTag("Waves").GetComponent<Waves>();

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        mysql = FindObjectOfType<Mysql>();        
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        Server.Stop();
    }

    public Player InstantiatePlayer(float x, float y, float z)
    {
        return Instantiate(playerPrefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<Player>();
    }

    int moveCount = 1;
    IEnumerator Tick()
    {
        while (true)
        {
            foreach (Client client in Server.clients.Values)
            {                
                if (buffer.ContainsKey(client.id))
                {                    
                    ProcessBuffer(client);
                    //ProcessMovementPackets(client);
                }
            }
            yield return new WaitForSeconds(1 / positionAndRotationTick);
        }
    }

    IEnumerator NPCPositionAndRotationTick()
    {        
        while (true)
        {
            foreach (NPC npc in Server.npcs.Values)
            {
                ServerSend.NPCPosition(npc.id, npc.transform.position, npc.transform.rotation);                    
            }
            yield return new WaitForSeconds(1 / positionAndRotationTick);
        }        
    }

    IEnumerator UpdatePlayerPosition()
    {
        while (true)
        {
            foreach (Client client in Server.clients.Values)
            {
                Player player = client.player;

                if (player != null)
                {
                    mysql.UpdateShipPosition(player.dbid, player.transform.position.x, player.transform.position.y, player.transform.position.z, player.transform.eulerAngles.y);

                    if (!player.data.is_on_ship)
                    {
                        Transform transform = player.playerInstance.transform;
                        mysql.UpdatePlayerPosition(player.dbid, transform.position.x, transform.position.y, transform.position.z, transform.eulerAngles.y, player.playerCharacter.pirate.transform.eulerAngles.y);
                    }
                }
            }
            yield return new WaitForSeconds(playerPositionUpdateTick);
        }
    }

    public static void AddPacket(int _fromClient, int type, Packet packet) {
        if (!buffer.ContainsKey(_fromClient))
            buffer.Add(_fromClient, new List<PacketData>());

        buffer[_fromClient].Add(new PacketData() { type=type, packet=packet});
    }

    void ProcessBuffer(Client client) {
        int end = buffer[client.id].Count;            

        foreach (PacketData packet in buffer[client.id]) {
            try
            {
                switch (packet.type)
                {
                    /*case (int)ClientPackets.welcomeReceived:
                        ServerHandle.WelcomeReceived(client.id, packet.packet);
                        break;*/
                    case (int)ClientPackets.login:
                        ServerHandle.Login(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerMovement:
                        ServerHandle.PlayerMovement(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.joystick:
                        ServerHandle.Joystick(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.position:
                        ServerHandle.Position(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.test:
                        ServerHandle.test(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.getInventory:
                        ServerHandle.GetInventory(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.dropItem:
                        ServerHandle.DropItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.dragAndDrop:
                        ServerHandle.DragAndDrop(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.searchChest:
                        ServerHandle.SearchChest(client.id, packet.packet);
                        break;                    
                    case (int)ClientPackets.getShipEquipment:
                        ServerHandle.GetShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.replaceShipEquipment:
                        ServerHandle.ReplaceShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.unequipItem:
                        ServerHandle.UnequipItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.removeItemFromInventory:
                        ServerHandle.RemoveItemFromInventory(client.id, packet.packet);
                        break;                    
                    case (int)ClientPackets.getPlayerEquipment:
                        ServerHandle.GetPlayerEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.replacePlayerEquipment:
                        ServerHandle.ReplacePlayerEquipment(client.id, packet.packet);
                        break;
                    /*case (int)ClientPackets.onGameStart:
                        ServerHandle.OnGameStart(client.id, packet.packet);
                        break;*/
                    case (int)ClientPackets.shoot:
                        ServerHandle.Shoot(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.cannonRotate:
                        ServerHandle.CannonRotate(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.collectLoot:
                        ServerHandle.CollectLoot(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.discardLoot:
                        ServerHandle.DiscardLoot(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.chatMessage:
                        ServerHandle.ChatMessage(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.createGroup:
                        ServerHandle.CreateGroup(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.getGroupList:
                        ServerHandle.GetGroupList(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.applyToGroup:
                        ServerHandle.ApplyToGroup(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.acceptGroupApplicant:
                        ServerHandle.AcceptGroupApplicant(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.declineGroupApplicant:
                        ServerHandle.DeclineGroupApplicant(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.kickGroupMember:
                        ServerHandle.KickGroupMember(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.leaveGroup:
                        ServerHandle.LeaveGroup(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.getPlayerList:
                        ServerHandle.GetPlayerList(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.invitePlayer:
                        ServerHandle.InvitePlayer(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.acceptGroupInvitation:
                        ServerHandle.AcceptGroupInvitation(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.declineGroupInvitation:
                        ServerHandle.DeclineGroupInvitation(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.leaveEnterShip:
                        ServerHandle.LeaveEnterShip(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerInputs:
                        ServerHandle.PlayerInputs(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.animationInputs:
                        ServerHandle.AnimationInputs(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.mouseX:
                        ServerHandle.MouseX(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.gatherResource:
                        ServerHandle.GatherResource(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerSkills:
                        ServerSend.PlayerSkills(client.id);
                        break;
                    case (int)ClientPackets.makeSelected:
                        ServerHandle.CraftSelected(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.cancelCrafting:
                        ServerHandle.CancelCrafting(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.requestCrafting:
                        ServerHandle.RequestCrafting(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.traderInventoryRequest:
                        ServerHandle.TraderInventoryRequest(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.buyItem:
                        ServerHandle.BuyItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.sellItem:
                        ServerHandle.SellItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.tradeBrokerRequest:
                        ServerHandle.TradeBrokerRequest(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.readTradeBrokerItems:
                        ServerHandle.ReadTradeBrokerItems(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.registerItemOnBroker:
                        ServerHandle.RegisterItemOnBroker(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.removeItemFromBroker:
                        ServerHandle.RemoveItemFromBroker(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.buyItemFromBroker:
                        ServerHandle.BuyItemFromBroker(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.collectFromBroker:
                        ServerHandle.CollectFromBroker(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.tradeRequest:
                        ServerHandle.TradeRequest(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.acceptTrade:
                        ServerHandle.AcceptTrade(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.declineTrade:
                        ServerHandle.DeclineTrade(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.cancelPlayerTrade:
                        ServerHandle.CancelPlayerTrade(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerTradeAddItem:
                        ServerHandle.PlayerTradeAddItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerTradeRemoveItem:
                        ServerHandle.PlayerTradeRemoveItem(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerTradeGoldChanged:
                        ServerHandle.PlayerTradeGoldChanged(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerTradeAccept:
                        ServerHandle.PlayerTradeAccept(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.isOnShip:
                        ServerHandle.IsOnShip(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.switchTarget:
                        ServerHandle.SwitchTarget(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.playerCharacterPosition:
                        ServerHandle.PlayerCharacterPosition(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.jump:
                        ServerHandle.Jump(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.startCrafting:
                        ServerSend.StartCrafting(client.id);
                        break;
                    case (int)ClientPackets.stopCrafting:
                        ServerSend.StopCrafting(client.id);
                        break;
                    case (int)ClientPackets.shipPosition:
                        ServerHandle.ShipPosition(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.addBuff:
                        ServerHandle.AddBuff(client.id, packet.packet);
                        break;
                }
            }
            catch (Exception ex) {
                Debug.LogError("NetworkManager.cs ProcessBuffer(): "+ex.Message+" "+ex.StackTrace);
            }
        }

        buffer[client.id].RemoveRange(0, end);
    }

    /*void ProcessMovementPackets(Client client) {
        int lastInputSequenceNumber = 0;
        PlayerInputs lastInput = null;
        //Debug.Log("To process: "+client.inputBuffer.Count);

        int end = client.inputBuffer.Count;
        for (int i = 0; i < end; i++)
        {
            //Debug.Log("SN " + client.inputBuffer[i].inputSequenceNumber);
            PlayerInputs input = client.inputBuffer[i];
            lastInput = input;
            lastInputSequenceNumber = input.inputSequenceNumber;
            client.player.Move(new BoatMovement.MovementOrder() { input = input, lastInputSequenceNumber = lastInputSequenceNumber, player = client.player});
            Debug.Log("SN " + input.inputSequenceNumber + " moveCount = " + moveCount + $" position={client.player.transform.position} move {input.left},{input.right},{input.forward}");
            moveCount += 1;
            //client.inputBuffer.RemoveAt(i);                        
        }
        if (end != 0)
            client.inputBuffer.RemoveRange(0, end);

        //if (lastInputSequenceNumber != 0)
        //{
            //client.lastInputSequenceNumber = lastInputSequenceNumber;
            //send.PlayerPosition(lastInput, client.lastInputSequenceNumber, client.player, visibilityRadius);
            //Debug.Log("SN " + client.lastInputSequenceNumber + ", position=" + client.player.transform.position);
        //}
        //Debug.Log("LSN:" + client.lastInputSequenceNumber);
    }*/

    public static void PlayerDisconnected(int id) {
        buffer.Remove(id);
    }

    public static SkillLevel FindSkill(SkillType skillType, int level) {
        foreach (SkillLevel skillLevel in skillLevel) {
            if (skillLevel.skill == skillType && skillLevel.level == level) {
                return skillLevel;
            }
        }
        return null;
    }

    public static Recipe FindRecipe(int recipeId) {
        foreach (Recipe recipe in recipes) {
            if (recipe.id == recipeId) {
                return recipe;
            }
        }
        return null;
    }

    public static SerializableObjects.Trader FindTrader(int from, int traderId) {
        foreach (SerializableObjects.Trader trader in traders[from])
        {
            if (trader.id == traderId)
            {
                return trader;
            }
        }
        return null;
    }

    public static SerializableObjects.TraderItem FindTraderItem(int from, int traderId, int item_id)
    {
        SerializableObjects.Trader trader = FindTrader(from, traderId);
        foreach (SerializableObjects.TraderItem item in trader.inventory)
        {
            if (item.item_id==item_id)
            {
                return item;
            }
        }
        return null;
    }

    IEnumerator Respawn()
    {
        RespawnTraders();
        yield return new WaitForSeconds(5);
    }

    IEnumerator RespawnTraders() {
        Action ac = () =>
        {   
            foreach (int playerId in traders.Keys)
            {
                List<SerializableObjects.Trader> traders = NetworkManager.traders[playerId];

                for (int i = 0; i < traders.Count; i++)
                {
                    SerializableObjects.Trader trader = traders[i];
                    //if ((DateTime.Now - trader.respawned).TotalMinutes > trader.item_respawn_time)
                    //{
                    trader = mysql.ReadTrader(trader.id);
                    trader.respawned = DateTime.Now;
                    traders[i] = trader;
                    //}
                }
            }            
        };
        Task task = null;

        while (true)
        {   
            if (task==null || task.Status!=TaskStatus.Running)
            {
                task = new Task(ac);
                task.Start();
            }                                         
            
            yield return new WaitForSeconds(respawnTradersTick);
        }
    }

    public static SerializableObjects.Item ItemToSerializable(Item item)
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
            cannon_force = item.cannon_force,
            stackable = item.stackable,
            energy = item.energy,
            max_energy = item.max_energy,
            max_health = item.max_health,
            overtime = item.overtime,
            buff_duration = item.buff_duration,
            cooldown = item.cooldown
        };
    }
    public static Item SerializableToItem(SerializableObjects.Item item)
    {
        return new Item()
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
            cannon_force = item.cannon_force,
            stackable = item.stackable,
            energy = item.energy,
            max_energy = item.max_energy,
            max_health = item.max_health,
            overtime = item.overtime,
            buff_duration = item.buff_duration,
            cooldown = item.cooldown
        };
    }
}