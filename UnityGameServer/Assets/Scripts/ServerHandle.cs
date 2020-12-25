using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class ServerHandle : MonoBehaviour
{
    private static SpawnManager spawnManager;
    private static Mysql mysql;
    private static Chat chat;
    private static List<Crafting> craftingList = new List<Crafting>();

    private void Awake()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        mysql = FindObjectOfType<Mysql>();
        chat = FindObjectOfType<Chat>();
    }

    public static void Welcome(int _fromClient, int dbid)
    {
        //int _clientIdCheck = _packet.ReadInt();
        //string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        /*if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }*/
        ServerSend.Welcome(_fromClient);
        PlayerData data = mysql.ReadPlayerData(dbid);
        Server.clients[_fromClient].SendIntoGame(data, "username", dbid);
        //ServerSend.WavesMesh(_fromClient, NetworkManager.wavesScript.GenerateMesh());

        ServerSend.Time(Time.time);
        NetworkManager.SendNPCBaseStats(_fromClient);
        spawnManager.SendAllGameObjects(_fromClient);
        ServerSend.Recipes(_fromClient);

        //send current stats to all
        //send stats from all to player
        //NetworkManager.SendStats(_fromClient);          
    }

    public static void Login(int _fromClient, Packet _packet)
    {
        string username = _packet.ReadString();
        string password = _packet.ReadString();

        int dbid = mysql.Login(username, password);

        if (dbid == 0)
        {
            ServerSend.LoginFailed(_fromClient);
        }
        else
        {
            Welcome(_fromClient, dbid);
        }
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
        int inputSequenceNumber = _packet.ReadInt();
        //Debug.Log("ISN: " + inputSequenceNumber);
        bool left = _packet.ReadBool();
        bool right = _packet.ReadBool();
        bool forward = _packet.ReadBool();

        //Server.clients[_fromClient].player.Move(new Vector3(left ? 1 : 0, right ? 1 : 0, forward ? 1 : 0));                
        Client client = Server.clients[_fromClient];
        client.inputBuffer.Add(new PlayerInputs() { left = left, right = right, forward = forward, inputSequenceNumber = inputSequenceNumber });
    }

    public static void Joystick(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        float vertical = _packet.ReadFloat();
        float horizontal = _packet.ReadFloat();

        Server.clients[_fromClient].player.SetInput(vertical, horizontal);
    }

    public static void test(int _fromClient, Packet _packet)
    {
        string data = _packet.ReadString();
        Debug.Log(data);
    }

    public static void GetInventory(int from, Packet packet)
    {
        Inventory inventory = Server.clients[from].player.inventory;

        ServerSend.Inventory(from, inventory);

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void GetShipEquipment(int from, Packet packet)
    {
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;
        ServerSend.ShipEquipment(from, equipment.Items());

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void GetPlayerEquipment(int from, Packet packet)
    {
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;

        ServerSend.PlayerEquipment(from, equipment.Items());

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void DropItem(int from, Packet packet)
    {
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();

        inventory.Remove(slot.slotID);

        InventorySlot s = SlotFromSerializable(slot);
        mysql.DropItem(Server.clients[from].player.dbid, s);
    }

    public static void AddItemToInventory(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment sequipment = Server.clients[from].player.ship_equipment;
        PlayerEquipment pequipment = Server.clients[from].player.player_equipment;

        string eq = packet.ReadString();
        SerializableObjects.Item item = packet.ReadItem();

        Item it = null;

        if (eq.Equals("ship_equipment"))
            it = sequipment.GetItem(item.item_type);
        else if (eq.Equals("player_equipment"))
            it = pequipment.GetItem(item.item_type);

        InventorySlot sl = inventory.Add(it);
        mysql.AddItemToInventory(dbid, sl);
    }

    public static void RemoveItemFromInventory(int from, Packet packet)
    {
        Inventory inventory = Server.clients[from].player.inventory;

        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();
        mysql.RemoveInventoryItem(from, slot.slotID);
        inventory.Remove(slot.slotID);
    }

    public static void ReplaceShipEquipment(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);

        mysql.RemoveInventoryItem(dbid, slot.slotID);
        mysql.AddShipEquipment(dbid, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(dbid, slot);
        }
        equipment.Add(new_);
    }

    public static void ReplacePlayerEquipment(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        Inventory inventory = Server.clients[from].player.inventory;
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);

        mysql.RemoveInventoryItem(dbid, slot.slotID);
        mysql.AddPlayerEquipment(dbid, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(dbid, slot);
        }
        equipment.Add(new_);
    }

    public static void DragAndDrop(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot1 = packet.ReadInventorySlot();
        SerializableObjects.InventorySlot slot2 = packet.ReadInventorySlot();

        InventorySlot s1 = SlotFromSerializable(slot1);
        InventorySlot s2 = SlotFromSerializable(slot2);

        InventorySlot slot_1 = inventory.FindSlot(s1.slotID);
        InventorySlot slot_2 = inventory.FindSlot(s2.slotID);

        if (slot_1.item != null && slot_2.item != null)
        {
            if (slot_1.item.item_id == slot_2.item.item_id && slot_1.item.stackable)
            {
                mysql.DragAndDrop_Stack(dbid, slot_1, slot_2);
            }
            else
            {
                mysql.DragAndDrop_Change(dbid, slot_1, slot_2);
            }
        }
        else if (slot_1.item == null || slot_2.item == null)
            mysql.DragAndDrop_Move(dbid, slot_1, slot_2);

        inventory.DragAndDrop(slot_1, slot_2);
    }

    public static void AddShipEquipment(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();

        InventorySlot sl = SlotFromSerializable(slot);
        sl = inventory.FindSlot(sl.slotID);

        if (sl.item != null)
        {
            equipment.Add(sl.item);
            mysql.AddShipEquipment(dbid, sl.item);
        }
    }

    public static void RemoveShipEquipment(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        string type = packet.ReadString();

        Item item = equipment.GetItem(type);

        if (item != null)
        {
            equipment.Remove(item);
            mysql.RemoveShipEquipment(dbid, item);
        }
    }

    public static void RemovePlayerEquipment(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        PlayerEquipment equipment = Server.clients[from].player.player_equipment;
        Inventory inventory = Server.clients[from].player.inventory;
        string type = packet.ReadString();

        Item item = equipment.GetItem(type);

        if (item != null)
        {
            equipment.Remove(item);
            mysql.RemovePlayerEquipment(dbid, item);
        }
    }

    protected static Item ItemFromSerializable(SerializableObjects.Item it)
    {
        Item item = new Item();
        item.id = it.id;
        item.name = it.name;
        item.item_type = it.item_type;
        item.stackable = it.stackable;

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
            item.stackable = slot.item.stackable;
            s.item = item;
        }

        s.quantity = slot.quantity;
        s.slotID = slot.slotID;

        return s;
    }

    public static void SearchChest(int from, Packet packet)
    {
        Server.clients[from].player.SearchChest();
    }

    /*public static void OnGameStart(int from, Packet packet) {
        Player player = Server.clients[from].player;
        List<BaseStat> stats = player.stats;
        List<Experience> exp = player.exp;
        PlayerData data = player.data;        

        ServerSend.OnGameStart(from, stats, exp, data);        
    }*/

    public static void Shoot(int from, Packet packet)
    {
        CannonShot shootScript = Server.clients[from].player.GetComponent<CannonShot>();
        shootScript.Shoot(packet.ReadString());
    }

    public static void CannonRotate(int from, Packet packet)
    {
        CannonController cannonRotate = Server.clients[from].player.GetComponent<CannonController>();
        cannonRotate.CannonRotate(packet.ReadString(), packet.ReadString());
    }

    public static void CollectLoot(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        List<int> items = packet.ReadIntList();

        Player player = Server.clients[from].player;

        bool ItemInList(ItemDrop drop)
        {
            foreach (int item_id in items)
                if (item_id == drop.item.item_id)
                    return true;
            return false;
        }

        if (player.lootCache != null)
        {
            foreach (ItemDrop drop in player.lootCache)
            {
                if (ItemInList(drop))
                {
                    int id = 0;
                    //player_item tabela
                    //add player item 
                    //if resource provjeriti da li postoji, ako postoji ne dodati
                    if (drop.item.stackable)
                    {
                        id = mysql.GetPlayerItemId(dbid, drop.item);
                        if (id == 0)
                        {
                            id = mysql.AddPlayerItem(dbid, drop.item);
                        }
                    }
                    else
                    {
                        id = mysql.AddPlayerItem(dbid, drop.item);
                    }

                    //inventory na serveru
                    //dodati u sljedeći prazan slot
                    //ako je resource i postoji item povećati quantity                        
                    InventorySlot slot = player.inventory.Add(drop.item, drop.quantity);
                    slot.item.id = id;

                    //dodati inventory slot u bazu ako ne postoji
                    //ako postoji update-ati
                    mysql.AddItemToInventory(dbid, slot);
                }
            }
        }

        player.lootCache = null;

        ServerSend.Inventory(from, player.inventory);
    }

    public static void DiscardLoot(int from, Packet packet)
    {

    }

    public static void ChatMessage(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        Message message = packet.ReadMessage();
        message.from = player.data.username;
        chat.OnChatMessage(from, message);
    }

    public static void CreateGroup(int from, Packet packet)
    {
        Player player = Server.clients[from].player;

        if (player.ownedGroup != null)
        {
            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = "You already own a group!";
            ServerSend.OnGameMessage(from, msg);
            ServerSend.GroupCreateStatus(from, false);
        }
        else
        {
            string groupName = packet.ReadString();
            Group group = new Group(groupName, player);

            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = "Group created!";
            ServerSend.OnGameMessage(from, msg);
            ServerSend.GroupCreateStatus(from, true);
        }
    }

    public static void GetGroupList(int from, Packet packet)
    {
        ServerSend.GroupList(from);
    }

    public static void ApplyToGroup(int from, Packet packet)
    {
        int groupId = packet.ReadInt();

        Player applicant = Server.clients[from].player;

        if (applicant.group != null || (NetworkManager.groups.ContainsKey(groupId) && NetworkManager.groups[groupId].players.Count == Group.maxPlayers))
            return;

        bool groupFound = false;
        foreach (Group group in NetworkManager.groups.Values)
        {
            if (group.groupId == groupId)
            {
                groupFound = true;
                Player owner = Server.FindPlayerByDBid(group.owner);

                if (owner != null)
                {
                    ServerSend.PlayerAppliedToGroup(applicant, owner.id);
                }
            }
        }

        if (!groupFound)
        {
            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = "Group does not exits anymore!";
            ServerSend.OnGameMessage(from, msg);
            ServerSend.GroupList(from);
        }
    }

    public static void AcceptGroupApplicant(int from, Packet packet)
    {
        int applicantId = packet.ReadInt();

        Message msg = new Message();
        msg.messageType = Message.MessageType.gameInfo;
        msg.text = "Welcome to group!";
        ServerSend.OnGameMessage(applicantId, msg);

        Player owner = Server.clients[from].player;
        Player applicant = Server.clients[applicantId].player;
        Group group = owner.group;

        if (group != null)
        {
            group.AddPlayer(applicant);
        }
    }

    public static void DeclineGroupApplicant(int from, Packet packet)
    {
        int applicantId = packet.ReadInt();

        Message msg = new Message();
        msg.messageType = Message.MessageType.gameInfo;
        msg.text = "Your group request has been declined!";
        ServerSend.OnGameMessage(applicantId, msg);
    }

    public static void KickGroupMember(int from, Packet packet)
    {
        Group group = Server.clients[from].player.group;
        Player player = Server.clients[packet.ReadInt()].player;

        group.RemovePlayer(player);

        Message msg = new Message();
        msg.messageType = Message.MessageType.gameInfo;
        msg.text = "You have been kicked from group!";
        ServerSend.OnGameMessage(player.id, msg);
        ServerSend.KickedFromGroup(player.id);
    }

    public static void LeaveGroup(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        Group group = Server.clients[from].player.group;

        foreach (int dbid in group.players)
        {
            if (dbid != player.dbid)
            {
                Player otherPlayer = Server.FindPlayerByDBid(dbid);
                if (otherPlayer != null)
                {
                    Message msg = new Message();
                    msg.messageType = Message.MessageType.gameInfo;
                    msg.text = "Player " + player.data.username + " left group!";
                    ServerSend.OnGameMessage(player.id, msg);
                }
            }
        }

        if (group.owner == player.dbid)
            player.TransferGroupOwner();

        group.RemovePlayer(player);
        ServerSend.GroupMembers(group.groupId);

        if (group.players.Count == 0)
            group.Disband();
    }

    public static void GetPlayerList(int from, Packet packet)
    {
        List<PlayerData> players = new List<PlayerData>();

        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null && client.player.id != from)
            {
                players.Add(client.player.data);
            }
        }

        ServerSend.PlayerList(from, players);
    }

    public static void InvitePlayer(int from, Packet packet)
    {
        Group group = null;
        string username = packet.ReadString();
        foreach (Group g in NetworkManager.groups.Values)
        {
            if (g.owner == Server.clients[from].player.dbid)
                group = g;
        }

        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null && client.player.data.username == username)
            {
                string acceptLink = "invite_accept" + System.Guid.NewGuid().ToString();
                string declineLink = "invite_decline" + System.Guid.NewGuid().ToString();
                NetworkManager.invitationLinks.Add(acceptLink, group.groupId);
                NetworkManager.invitationLinks.Add(declineLink, group.groupId);

                string accept = $"<link={acceptLink}><color=green><u>Accept</u></color></link>";
                string decline = $"<link={declineLink}><color=#FF0006><u>Decline</u></color></link>";

                Message msg = new Message();
                msg.messageType = Message.MessageType.privateMessage;
                msg.text = $"{accept} or {decline} invite from {client.player.data.username} (lvl {client.player.data.level})";
                ServerSend.ChatMessage(from, msg, client.player.id);
            }
        }
    }

    public static void AcceptGroupInvitation(int from, Packet packet)
    {
        string link = packet.ReadString();

        if (NetworkManager.invitationLinks.ContainsKey(link))
        {
            int groupId = NetworkManager.invitationLinks[link];
            NetworkManager.invitationLinks.Remove(link);
            if (NetworkManager.groups.ContainsKey(groupId))
            {
                Group group = NetworkManager.groups[groupId];
                int applicantId = from;

                Message msg = new Message();
                msg.messageType = Message.MessageType.gameInfo;
                msg.text = "Welcome to group!";
                ServerSend.OnGameMessage(applicantId, msg);

                Player applicant = Server.clients[applicantId].player;

                if (group != null)
                {
                    group.AddPlayer(applicant);
                }
            }
            else
            {
                Message msg = new Message();
                msg.messageType = Message.MessageType.gameInfo;
                msg.text = "Group does not exist anymore!";
                ServerSend.OnGameMessage(from, msg);
            }
        }
    }

    public static void DeclineGroupInvitation(int from, Packet packet)
    {
        string link = packet.ReadString();

        if (NetworkManager.invitationLinks.ContainsKey(link))
        {
            int groupId = NetworkManager.invitationLinks[link];
            Group group = NetworkManager.groups[groupId];
            Player player = Server.clients[from].player;

            if (group != null)
            {
                Message msg = new Message();
                msg.messageType = Message.MessageType.privateMessage;
                msg.text = $"Player {player.data.username} decline group invitation!";
                ServerSend.ChatMessage(from, msg, Server.FindPlayerByDBid(group.owner).id);
            }
        }
    }

    public static void LeaveEnterShip(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        player.LeaveEnterShip();
    }

    public static void PlayerInputs(int from, Packet packet)
    {
        Player player = Server.clients[from].player;

        if (player.playerInstance != null)
        {
            Vector3 position = player.playerInstance.transform.position;
            PlayerMovement movement = player.playerInstance.GetComponent<PlayerMovement>();
            CharacterAnimationController animationController = player.playerInstance.GetComponentInChildren<CharacterAnimationController>();
            bool w = packet.ReadBool();
            bool leftShift = packet.ReadBool();
            bool jump = packet.ReadBool();
            bool leftMouseDown = packet.ReadBool();
            Vector3 move = packet.ReadVector3();

            PlayerMovement.PlayerInputs input = new PlayerMovement.PlayerInputs() { w = w, leftShift = leftShift, jump = jump, move = move, leftMouseDown = leftMouseDown };
            movement.buffer.Add(input);

            ServerSend.PlayerInputs(from, input, position);
        }
    }

    public static void AnimationInputs(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        if (player.playerInstance != null)
        {
            Vector3 position = player.playerInstance.transform.position;

            PlayerMovement movement = player.playerInstance.GetComponent<PlayerMovement>();
            CharacterAnimationController animationController = player.playerInstance.GetComponentInChildren<CharacterAnimationController>();

            bool w = packet.ReadBool();
            bool leftShift = packet.ReadBool();
            bool jump = packet.ReadBool();
            bool leftMouseDown = packet.ReadBool();

            CharacterAnimationController.AnimationInputs input = new CharacterAnimationController.AnimationInputs() { w = w, leftShift = leftShift, jump = jump, leftMouseDown = leftMouseDown };
            animationController.buffer.Add(input);

            ServerSend.AnimationInputs(from, input, position);
        }
    }

    public static void MouseX(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        if (player.playerInstance != null)
        {
            Vector3 position = player.playerInstance.transform.position;

            mouseLook look = player.playerInstance.GetComponent<mouseLook>();
            float x = packet.ReadFloat();
            look.buffer.Add(x);

            ServerSend.MouseLook(from, x, position);
        }
    }

    public static void GatherResource(int from, Packet packet)
    {
        //TODO: respawn time
        int resourceID = packet.ReadInt();

        GameObject gameObject = spawnManager.objects[resourceID].gameObject;
        Resource resource = gameObject.GetComponent<Resource>();
        Player player = Server.clients[from].player;
        PlayerSkillLevel skill = player.FindSkill(resource.skill_type);

        if (spawnManager.objects.ContainsKey(resourceID) && player.playerInstance.GetComponent<PlayerMovement>().gatheringEnabled)
        {
            int numberOfResource = 0;
            int experienceGained = 0;
            resource.GatherResource(skill.modifier, out numberOfResource, out experienceGained);
            mysql.UpdateSkillExperience(player.dbid, (int)resource.skill_type, experienceGained);
            player.ExperienceGained(resource.skill_type, experienceGained, player);
            player.skills = mysql.ReadPlayerSkills(player.dbid);

            if (numberOfResource > 0)
            {
                int id = mysql.GetPlayerItemId(player.dbid, resource.item);
                if (id == 0)
                {
                    id = mysql.AddPlayerItem(player.dbid, resource.item);
                }

                InventorySlot slot = player.inventory.Add(resource.item, numberOfResource);
                slot.item.id = id;

                //dodati inventory slot u bazu ako ne postoji
                //ako postoji update-ati
                mysql.AddItemToInventory(player.dbid, slot);

                ServerSend.AddToInventory(from, slot);

                if (resource.Empty())
                {
                    ServerSend.DestroyResource(resourceID);
                    //trebamo proći kroz sve kliente koji su gatherali taj resource i stopirati i njihovu animaciju
                    GameObject simpleCharacter = player.playerInstance.transform.Find("simpleCharacter_v2").gameObject;
                    CharacterAnimationController animationController = simpleCharacter.GetComponent<CharacterAnimationController>();
                    animationController.choping = false;
                    resource.Gathered();
                }
            }
        }
    }

    public static void CraftSelected(int from, Packet packet)
    {
        int recipeID = packet.ReadInt();
        int makeAmount = packet.ReadInt();       

        Item craftingItem = null;
        Recipe recipe = NetworkManager.FindRecipe(recipeID);
        craftingItem = mysql.ReadItem(recipe.item_id);

        GameObject go = Resources.Load("Prefabs/Crafting", typeof(GameObject)) as GameObject;
        go = Instantiate(go);
        Crafting crafting = go.GetComponent<Crafting>();
        crafting.Initialize(from, recipeID, mysql);
        craftingList.Add(crafting);
        int maxAmount = crafting.GetMaxCraftAmount();

        PlayerSkillLevel level = Server.clients[from].player.FindSkillRequirement(recipe.skill.skill_level_id);
        if (makeAmount <= maxAmount && Server.clients[from].player.HasSkillRequirement(recipe.skill.skill_level_id)) {
            crafting.Craft(makeAmount, level.modifier, recipe.time_to_craft);            
        }
    }

    public static void CancelCrafting(int from, Packet packet)
    {
        foreach(Crafting craft in craftingList)
        {
            if(craft.from == from)
            {
                craft.Stop();
            }
        }
    }

    public static void RequestCrafting(int from, Packet packet) {
        CraftingSpot craftingSpot = Server.clients[from].player.playerInstance.GetComponent<PlayerMovement>().craftingSpot;        
        ServerSend.RequestCraftingResponse(from, craftingSpot);        
    }
}
