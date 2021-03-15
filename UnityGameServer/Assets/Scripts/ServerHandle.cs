using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Linq;

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
        ServerSend.Parameters(_fromClient, new Parameters() { visibilityRadius = NetworkManager.visibilityRadius, inventorySize=Inventory.space });
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
        PlayerEquipment equipment = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>().equipment;

        ServerSend.PlayerEquipment(from, equipment.Items());

        /*Item wood = new Item();
        wood.name = "Wood log";
        wood.iconName = "wood.png";
        InventorySlot slot = inventory.Add(wood);

        ServerSend.AddToInventory(from, slot);*/
    }

    public static void DropItem(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        Inventory inventory = player.inventory;
        SerializableObjects.InventorySlot slot = packet.ReadInventorySlot();
        int quantity = packet.ReadInt();

        if (slot.slot_type.Length > 0)
        {
            if (slot.slot_type.Equals("player_equipment"))
            {
                Item item = NetworkManager.SerializableToItem(slot.item);
                PlayerEquipment pequipment = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>().equipment;
                mysql.RemovePlayerEquipment(player.dbid, item);
                pequipment.Remove(item);
            }
            else if (slot.slot_type.Equals("ship_equipment")) {
                Item item = NetworkManager.SerializableToItem(slot.item);
                ShipEquipment sequipment = Server.clients[from].player.ship_equipment;
                mysql.RemoveShipEquipment(player.dbid, item);
                sequipment.Remove(item);
            }
        }
        else
        { 
            InventorySlot s = inventory.FindSlot(slot.slotID);

            if (!s.item.stackable || (s.item.stackable && s.quantity == quantity))
            {
                inventory.Remove(slot.slotID);
                mysql.DropItem(Server.clients[from].player.dbid, s);

                quantity = 1;
                int id = 0;
                Item tempItem = mysql.GetTempStorageItem(player.dbid, out id, out quantity);
                if (tempItem != null)
                {
                    mysql.RemoveTemporaryStorage(id);
                    mysql.InventoryAdd(player, tempItem, quantity);
                }
                ServerSend.Inventory(from, inventory);
            }
            else {
                inventory.RemoveAmount(s.slotID, quantity);
                mysql.UpdateItemQuantity(player.dbid, s);
            }
        }
    }

    public static void UnequipItem(int from, Packet packet)
    {
        int dbid = Server.clients[from].player.dbid;
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment sequipment = Server.clients[from].player.ship_equipment;
        PlayerEquipment pequipment = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>().equipment;

        string eq = packet.ReadString();
        string type = packet.ReadString();        
        SerializableObjects.Item item = packet.ReadItem();

        Item it = null;

        if (eq.Equals("ship_equipment"))
            it = sequipment.GetItem(item.item_type);
        else if (eq.Equals("player_equipment"))
            it = pequipment.GetItem(item.item_type);

        InventorySlot sl = inventory.Add(it);
        if (sl != null) { 
            mysql.AddItemToInventory(dbid, sl);

            if (eq.Equals("ship_equipment"))
                it = sequipment.GetItem(type);
            else if (eq.Equals("player_equipment"))
                it = pequipment.GetItem(type);

            if (it != null)
            {
                if (eq.Equals("ship_equipment"))
                {
                    sequipment.Remove(it);
                    mysql.RemoveShipEquipment(dbid, it);
                }
                else if (eq.Equals("player_equipment")) {
                    pequipment.Remove(it);
                    mysql.RemovePlayerEquipment(dbid, it);
                }
            }            
        }

        ServerSend.InventoryItemCount(from);
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
        Player player = Server.clients[from].player;
        Inventory inventory = Server.clients[from].player.inventory;
        ShipEquipment equipment = Server.clients[from].player.ship_equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);

        mysql.RemoveInventoryItem(player.dbid, slot.slotID);
        mysql.AddShipEquipment(player.dbid, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(player.dbid, slot);
        }else {
            int quantity = 1;
            int id = 0;
            Item tempItem = mysql.GetTempStorageItem(player.dbid, out id, out quantity);
            if (tempItem != null)
            {
                mysql.RemoveTemporaryStorage(id);
                slot = mysql.InventoryAdd(player, tempItem, quantity);
                ServerSend.AddToInventory(player.id, slot);
            }
        }
        equipment.Add(new_);
        ServerSend.InventoryItemCount(from);
    }

    public static void ReplacePlayerEquipment(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        Inventory inventory = Server.clients[from].player.inventory;
        PlayerEquipment equipment = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>().equipment;

        SerializableObjects.InventorySlot sl = packet.ReadInventorySlot();
        InventorySlot slot = inventory.FindSlot(sl.slotID);

        Item new_ = slot.item;
        Item old = equipment.GetItem(new_.item_type);

        mysql.RemoveInventoryItem(player.dbid, slot.slotID);
        mysql.AddPlayerEquipment(player.dbid, new_);

        inventory.Remove(slot.slotID);
        if (old != null)
        {
            slot = inventory.Add(old);
            mysql.AddItemToInventory(player.dbid, slot);
        }
        else {
            int quantity = 1;
            int id = 0;
            Item tempItem = mysql.GetTempStorageItem(player.dbid, out id, out quantity);
            if (tempItem != null)
            {
                mysql.RemoveTemporaryStorage(id);
                slot = mysql.InventoryAdd(player, tempItem, quantity);                
                ServerSend.AddToInventory(player.id, slot);
            }
        }
        equipment.Add(new_);
        ServerSend.InventoryItemCount(from);
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
        CannonShot shootScript = Server.clients[from].player.cannonShot;
        shootScript.Shoot(packet.ReadString());
    }

    public static void CannonRotate(int from, Packet packet)
    {
        CannonController cannonRotate = Server.clients[from].player.cannonController;
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
                    mysql.InventoryAdd(player, drop.item, drop.quantity);
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
            float speed = packet.ReadFloat();
            float horizontal = packet.ReadFloat();
            string attackName = packet.ReadString();
            string rollDirection = packet.ReadString();

            CharacterAnimationController.AnimationInputs input = new CharacterAnimationController.AnimationInputs() { 
                w = w, leftShift = leftShift, jump = jump, leftMouseDown = leftMouseDown, 
                speed=speed, horizontal = horizontal, attackName = attackName, rollDirection = rollDirection };
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
        PlayerCharacter playerCharacter = player.playerInstance.GetComponent<PlayerCharacter>();

        if (spawnManager.objects.ContainsKey(resourceID) && 
            playerCharacter.gatheringEnabled &&
            !playerCharacter.currentResource.GetComponentInParent<Resource>().respawning)
        {
            int numberOfResource = 0;
            int experienceGained = 0;
            resource.GatherResource(skill.modifier, out numberOfResource, out experienceGained);
            mysql.UpdateSkillExperience(player.dbid, (int)resource.skill_type, experienceGained);
            player.ExperienceGained(resource.skill_type, experienceGained, player);
            player.skills = mysql.ReadPlayerSkills(player.dbid);

            ServerSend.ExperienceGained(from, experienceGained);

            if (numberOfResource > 0)
            {
                InventorySlot slot = mysql.InventoryAdd(player, resource.item, numberOfResource);
                /*if (slot != null)
                {
                    ServerSend.AddToInventory(from, slot);
                }*/

                ServerSend.Inventory(from, player.inventory);

                if (resource.Empty())
                {
                    ServerSend.DestroyResource(resourceID);
                    //trebamo proći kroz sve kliente koji su gatherali taj resource i stopirati i njihovu animaciju
                    GameObject simpleCharacter = player.playerInstance.transform.Find("Pirate_01").gameObject;
                    CharacterAnimationController animationController = simpleCharacter.GetComponent<CharacterAnimationController>();
                    animationController.gathering = false;
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

        PlayerSkillLevel level = Server.clients[from].player.FindSkillRequirement(recipe.skill_id, recipe.skill.level);
        if (makeAmount <= maxAmount && Server.clients[from].player.HasSkillRequirement(recipe.skill_id, recipe.skill.level)) {
            crafting.Craft(makeAmount, level.modifier, recipe.time_to_craft);
        }
    }

    public static void CancelCrafting(int from, Packet packet)
    {
        foreach (Crafting craft in craftingList)
        {
            if (craft.from == from)
            {
                craft.Stop();
            }
        }
    }

    public static void RequestCrafting(int from, Packet packet) {
        CraftingSpot craftingSpot = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>().craftingSpot;
        ServerSend.RequestCraftingResponse(from, craftingSpot);
    }

    public static void TraderInventoryRequest(int from, Packet packet)
    {
        Player player = Server.clients[from].player;
        List<SerializableObjects.Trader> traders = NetworkManager.traders[player.dbid];
        int traderId = packet.ReadInt();
        foreach (SerializableObjects.Trader trader in traders)
        {
            if (trader.id == traderId)
            {
                ServerSend.TraderInventory(from, trader);
            }
        }
    }

    public static void TradeBrokerRequest(int from, Packet packet)
    {
        List<Category> categories = mysql.ReadCategories();
        ServerSend.Categories(from, categories);
    }

    public static void BuyItem(int from, Packet packet)
    {
        int itemID = packet.ReadInt();
        int amount = packet.ReadInt();

        if (amount == 0)
            return;

        Player player = Server.clients[from].player;
        PlayerCharacter playerCharacter = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>();
        if (playerCharacter.trader != null) {
            SerializableObjects.TraderItem traderItem = NetworkManager.FindTraderItem(player.dbid, playerCharacter.trader.id, itemID);
            SerializableObjects.Trader trader = NetworkManager.FindTrader(player.dbid, playerCharacter.trader.id);
            float totalPrice = traderItem.sell_price * amount;

            if (traderItem != null && totalPrice < player.data.gold && amount <= traderItem.quantity)
            {
                traderItem.quantity -= amount;
                mysql.InventoryAdd(player, NetworkManager.SerializableToItem(traderItem.item), amount);

                player.data.gold -= totalPrice;
                mysql.UpdatePlayerGold(player.dbid, player.data.gold);

                ServerSend.Inventory(from, player.inventory);
                ServerSend.PlayerData(from, player.data);
                ServerSend.TraderInventory(from, trader);
            }
        }
    }

    public static void SellItem(int from, Packet packet)
    {
        int itemID = packet.ReadInt();
        int amount = packet.ReadInt();

        if (amount == 0)
            return;

        Player player = Server.clients[from].player;
        PlayerCharacter playerCharacter = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>();
        SerializableObjects.Trader trader = NetworkManager.FindTrader(from, playerCharacter.trader.id);
        SerializableObjects.TraderItem traderItem = NetworkManager.FindTraderItem(from, playerCharacter.trader.id, itemID);

        if (playerCharacter.trader != null &&
            player.inventory.HasQuantity(itemID, amount) &&
            traderItem != null) {
            float totalPrice = traderItem.buy_price * amount;

            player.data.gold += totalPrice;
            mysql.UpdatePlayerGold(player.dbid, player.data.gold);
            mysql.InventoryRemove(player, itemID, amount);

            ServerSend.Inventory(from, player.inventory);
            ServerSend.PlayerData(from, player.data);
            ServerSend.TraderInventory(from, trader);
        }
        //ukloniti item iz inventory-ja        
    }

    public static void ReadTradeBrokerItems(int from, Packet packet) {
        int categoryId = packet.ReadInt();
        string name = packet.ReadString();
        bool showMyItems = packet.ReadBool();
        bool showSoldItems = packet.ReadBool();

        Player player = Server.clients[from].player;

        List<TradeBrokerItem> items = mysql.ReadTradeBrokerItems(player.dbid, categoryId, name.Length == 0 ? null : name, showMyItems, showSoldItems);
        ServerSend.TradeBrokerItems(from, items);
        ServerSend.Inventory(from, Server.clients[from].player.inventory);
    }

    public static void RegisterItemOnBroker(int from, Packet packet) {
        SerializableObjects.Item item = packet.ReadItem();
        int quantity = packet.ReadInt();
        float price = packet.ReadFloat();
        int category = packet.ReadInt();
        string itemName = packet.ReadString();
        bool showMyItems = packet.ReadBool();
        bool showSoldItems = packet.ReadBool();

        Player player = Server.clients[from].player;
        PlayerCharacter playerCharacter = Server.clients[from].player.playerInstance.GetComponent<PlayerCharacter>();

        //dodati u trade_broker_items
        if (playerCharacter.tradeBrokerEnabled &&
            item!=null &&
            player.inventory.HasQuantity(item.item_id, quantity) &&
            quantity>0 &&
            price>0){                                     
            
            mysql.InventoryRemove(player, item.item_id, quantity);
            mysql.AddTradeBrokerItem(player.dbid, item.item_id, quantity, price);

            ServerSend.Inventory(from, player.inventory);
            ServerSend.PlayerData(from, player.data);
            ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
        }
    }

    public static void RemoveItemFromBroker(int from, Packet packet) {
        int id = packet.ReadInt();
        int category = packet.ReadInt();
        string itemName = packet.ReadString();
        bool showMyItems = packet.ReadBool();
        bool showSoldItems = packet.ReadBool();

        Player player = Server.clients[from].player;
        TradeBrokerItem item = mysql.ReadTradeBrokerItem(player.dbid, id);
        if (item.IsMyItem) {
            mysql.DropTradeBrokerItem(id);
            mysql.InventoryAdd(player, NetworkManager.SerializableToItem(item.item), item.quantity);

            ServerSend.Inventory(from, player.inventory);
            ServerSend.PlayerData(from, player.data);
            ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
        }    
    }

    public static void BuyItemFromBroker(int from, Packet packet)
    {
        int id = packet.ReadInt();
        int quantity = packet.ReadInt();
        int category = packet.ReadInt();
        string itemName = packet.ReadString();
        bool showMyItems = packet.ReadBool();
        bool showSoldItems = packet.ReadBool();

        Player player = Server.clients[from].player;
        TradeBrokerItem item = mysql.ReadTradeBrokerItem(player.dbid, id);

        if (item != null && !item.sold && player.data.gold>=item.price*quantity)
        {
            if (quantity > item.quantity)
            {
                Message msg = new Message();
                msg.messageType = Message.MessageType.gameInfo;
                msg.text = "Only " + item.quantity + "x " + item.item.name + " available!";
                ServerSend.OnGameMessage(from, msg);
                ServerSend.Inventory(from, player.inventory);
                ServerSend.PlayerData(from, player.data);
                ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
                return;
            }

            mysql.UpdateTradeBrokerItem(id, item.quantity - quantity, item.quantity - quantity == 0);
            TradeBrokerItem soldItem = mysql.ReadSoldTradeBrokerItem(player.dbid, item.id);

            if (soldItem != null)
            {
                mysql.UpdateTradeBrokerItem(soldItem.id, soldItem.quantity + quantity, true);
            }
            else
            {
                mysql.AddTradeBrokerItem(item.seller_id, item.item.item_id, quantity, item.price, item.id, true);
            }

            player.data.gold -= item.price * quantity;
            mysql.UpdatePlayerGold(player.dbid, player.data.gold);
            mysql.InventoryAdd(player, NetworkManager.SerializableToItem(item.item), quantity);

            Player seller = Server.FindPlayerByDBid(item.seller_id);
            if (seller != null)
            {
                Message message = new Message();
                message.messageType = Message.MessageType.gameInfo;
                message.text = "Your item " + item.item.name + " has been sold!";
                ServerSend.OnGameMessage(Server.FindPlayerByDBid(item.seller_id).id, message);                
            }
            ServerSend.PlayerData(from, player.data);
            ServerSend.Inventory(from, player.inventory);
            ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
        }
        else if (item == null)
        {
            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = "Item has been removed!";
            ServerSend.OnGameMessage(from, msg);
            ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
        }
        else if (item != null && item.sold) {
            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = "Item has been sold!";
            ServerSend.OnGameMessage(from, msg);
            ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
        }
    }

    public static void CollectFromBroker(int from, Packet packet)
    {
        int id = packet.ReadInt();        
        int category = packet.ReadInt();
        string itemName = packet.ReadString();
        bool showMyItems = packet.ReadBool();
        bool showSoldItems = packet.ReadBool();

        Player player = Server.clients[from].player;
        TradeBrokerItem item = mysql.ReadTradeBrokerItem(player.dbid, id);

        if (item != null && item.parent_id.HasValue) {
            mysql.RemoveTradeBrokerItem(item.id);

            player.data.gold += item.price * item.quantity;
            mysql.UpdatePlayerGold(player.dbid, player.data.gold);

            if (mysql.TradeBrokerAllItemsCollected(item.parent_id.Value)) {
                mysql.RemoveTradeBrokerItem(item.parent_id.Value);
            }
        }

        ServerSend.PlayerData(from, player.data);
        ServerSend.Inventory(from, player.inventory);
        ServerSend.TradeBrokerItems(from, mysql.ReadTradeBrokerItems(player.dbid, category, itemName, showMyItems, showSoldItems));
    }

    public static void TradeRequest(int from, Packet packet) {
        string username = packet.ReadString();
        Player sender = Server.clients[from].player;
        Player player = Server.FindPlayerByUsername(username);

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
            ServerSend.ChatMessage(from, msg, player.id);
        }
    }

    public static void AcceptTrade(int from, Packet packet)
    {
        string link = packet.ReadString();
        if (NetworkManager.tradeLinks.ContainsKey(link))
        {
            PlayerTrade trade = NetworkManager.tradeLinks[link];
            NetworkManager.tradeLinks.Remove(link);

            PlayerTrade trade1 = new PlayerTrade();
            trade1.player1 = trade.player1;
            trade1.player2 = trade.player2;
            trade1.items1 = new List<SerializableObjects.InventorySlot>();
            trade1.items2 = new List<SerializableObjects.InventorySlot>();

            PlayerTrade trade2 = new PlayerTrade();
            trade2.player1 = trade.player2;
            trade2.player2 = trade.player1;
            trade2.items1 = new List<SerializableObjects.InventorySlot>();
            trade2.items2 = new List<SerializableObjects.InventorySlot>();

            int player1 = Server.FindPlayerByUsername(trade1.player1.username).id;
            int player2 = Server.FindPlayerByUsername(trade2.player1.username).id;
            NetworkManager.trades.Add(player1, trade1);
            NetworkManager.trades.Add(player2, trade2);
            ServerSend.PlayerTrade(trade1, Server.clients[player1].player.inventory);
            ServerSend.PlayerTrade(trade2, Server.clients[player2].player.inventory);
        }
    }

    public static void DeclineTrade(int from, Packet packet)
    {
        string link = packet.ReadString();
        if (NetworkManager.tradeLinks.ContainsKey(link))
        {
            PlayerTrade trade = NetworkManager.tradeLinks[link];
            NetworkManager.tradeLinks.Remove(link);
            int otherPlayer = Server.GetOtherPlayer(trade, from);
            
            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = $"Trade was declined!";
            ServerSend.ChatMessage(from, msg, otherPlayer);            
        }
    }

    public static void CancelPlayerTrade(int from, Packet packet) {
        if (NetworkManager.trades.ContainsKey(from)) {
            PlayerTrade trade = NetworkManager.trades[from];
            int otherPlayer = Server.FindPlayerByUsername(trade.player2.username).id;

            ServerSend.PlayerTradeCanceled(otherPlayer);        
            NetworkManager.trades.Remove(from);
            NetworkManager.trades.Remove(otherPlayer);
        }
    }

    public static void PlayerTradeAddItem(int from, Packet packet)
    {
        if (NetworkManager.trades.ContainsKey(from)) {
            SerializableObjects.Item item =  NetworkManager.ItemToSerializable(mysql.ReadItem(packet.ReadItem().item_id));
            int quantity = packet.ReadInt();

            PlayerTrade trade = NetworkManager.trades[from];
            bool found = false;
            foreach(SerializableObjects.InventorySlot slot in trade.items1)
            {
                if (slot.item.item_id == item.item_id && item.stackable)
                {
                    found = true;
                    slot.quantity += quantity;
                }
            }
            if(!found)            
            {
                SerializableObjects.InventorySlot slot = new SerializableObjects.InventorySlot();
                slot.item = item;
                slot.quantity = quantity;
                trade.items1.Add(slot);
            }
            int otherPlayer = Server.FindPlayerByUsername(trade.player2.username).id;

            PlayerTrade tradeOther = NetworkManager.trades[otherPlayer];
            tradeOther.items2 = trade.items1;

            ServerSend.PlayerTrade(tradeOther, Server.clients[otherPlayer].player.inventory);
        }
    }

    public static void PlayerTradeRemoveItem(int from, Packet packet)
    {
        if (NetworkManager.trades.ContainsKey(from))
        {
            SerializableObjects.Item item = NetworkManager.ItemToSerializable(mysql.ReadItem(packet.ReadItem().item_id));
            int quantity = packet.ReadInt();

            PlayerTrade trade = NetworkManager.trades[from];
            SerializableObjects.InventorySlot s=null;
            foreach (SerializableObjects.InventorySlot slot in trade.items1)
            {
                if (slot.item.item_id == item.item_id)
                {
                    s = slot;
                    if (item.stackable)
                    {
                        slot.quantity -= quantity;
                    }
                }
            }

            if (s != null) {
                if (item.stackable && s.quantity == 0)
                {
                    trade.items1.Remove(s);
                }
                else if (!item.stackable) {
                    trade.items1.Remove(s);
                }
            }

            int otherPlayer = Server.FindPlayerByUsername(trade.player2.username).id;

            PlayerTrade tradeOther = NetworkManager.trades[otherPlayer];
            tradeOther.items2 = trade.items1;

            ServerSend.PlayerTrade(tradeOther, Server.clients[otherPlayer].player.inventory);
        }
    }

    public static void PlayerTradeGoldChanged(int from, Packet packet) {
        if (NetworkManager.trades.ContainsKey(from))
        {
            int gold = packet.ReadInt();

            PlayerTrade trade = NetworkManager.trades[from];
            trade.gold1 = gold;            

            int otherPlayer = Server.FindPlayerByUsername(trade.player2.username).id;

            PlayerTrade tradeOther = NetworkManager.trades[otherPlayer];
            tradeOther.gold2 = gold;

            ServerSend.PlayerTrade(tradeOther, Server.clients[otherPlayer].player.inventory);
        }
    }

    public static void PlayerTradeAccept(int from, Packet packet) {
        if (NetworkManager.trades.ContainsKey(from))
        {
            PlayerTrade trade = NetworkManager.trades[from];
            trade.accepted = true;
            int otherPlayer = Server.FindPlayerByUsername(trade.player2.username).id;
            PlayerTrade tradeOther = NetworkManager.trades[otherPlayer];

            Message msg = new Message();
            msg.messageType = Message.MessageType.gameInfo;
            msg.text = $"Trade was accepted!";
            ServerSend.ChatMessage(from, msg, otherPlayer);

            if (trade.accepted && tradeOther.accepted) {
                bool result1 = PlayerTradeCheck(trade);
                bool result2 = PlayerTradeCheck(tradeOther);                

                Player player1 = Server.FindPlayerByUsername(trade.player1.username);
                Player player2 = Server.FindPlayerByUsername(tradeOther.player1.username);

                if (result1 && result2 && player1.data.gold >= trade.gold1 && player2.data.gold >= tradeOther.gold1)
                {
                    if (trade.gold1 > 0) {
                        player1.data.gold -= trade.gold1;
                        mysql.UpdatePlayerGold(player1.dbid, player1.data.gold);
                        player2.data.gold += trade.gold1;
                        mysql.UpdatePlayerGold(player2.dbid, player2.data.gold);
                    }

                    if (tradeOther.gold1 > 0)
                    {
                        player2.data.gold -= tradeOther.gold1;
                        mysql.UpdatePlayerGold(player2.dbid, player2.data.gold);
                        player1.data.gold += tradeOther.gold1;
                        mysql.UpdatePlayerGold(player1.dbid, player1.data.gold);
                    }

                    foreach (SerializableObjects.InventorySlot slot in trade.items1) {
                        mysql.InventoryAdd(player2, NetworkManager.SerializableToItem(slot.item), slot.quantity);
                        mysql.InventoryRemove(player1, slot.item.item_id, slot.quantity);                        
                    }

                    foreach (SerializableObjects.InventorySlot slot in tradeOther.items1)
                    {
                        mysql.InventoryAdd(player1, NetworkManager.SerializableToItem(slot.item), slot.quantity);
                        mysql.InventoryRemove(player2, slot.item.item_id, slot.quantity);
                    }

                    ServerSend.Inventory(player1.id, player1.inventory);
                    ServerSend.Inventory(player2.id, player2.inventory);
                    ServerSend.PlayerData(player1.id, player1.data);
                    ServerSend.PlayerData(player2.id, player2.data);
                    ServerSend.PlayerTradeClose(player1.id);
                    ServerSend.PlayerTradeClose(player2.id);
                    NetworkManager.trades.Remove(player1.id);
                    NetworkManager.trades.Remove(player2.id);
                }
                else {
                    msg = new Message();
                    msg.messageType = Message.MessageType.gameInfo;
                    msg.text = $"Trade is invalid!";
                    ServerSend.ChatMessage(0, msg, player1.id);
                    ServerSend.ChatMessage(0, msg, player2.id);
                }

                //povećati i smanjiti gold jednom i drugom playeru, provjeriti da li player ima dovoljno gold              
                //dodati u inventory od drugog player-a i izbrisati iz svog inventory-a               
            }
        }
    }

    public static void IsOnShip(int from, Packet packet) {
        int playerId = packet.ReadInt();
        Debug.Log("IsOnShip player=" + playerId);
        Player player = Server.clients[playerId].player;
        ServerSend.IsOnShip(from, playerId, player.data.is_on_ship);
    }
    struct PlayerDistance
    {
        public Player player;
        public  float distance;
    };

    public static void SwitchTarget(int from, Packet packet) {
        Player player = Server.clients[from].player;
        GameObject playerObject = Server.clients[from].player.playerInstance;
        List<PlayerDistance> players = new List<PlayerDistance>();
        
        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null && client.player.id!=from)
            {
                float distance = Vector3.Distance(playerObject.transform.position, client.player.playerInstance.transform.position);
                if (distance <= 10)
                {
                    players.Add(new PlayerDistance() { player = client.player, distance = distance });
                }
            }
        }
        
        players.Sort((x, y) => x.distance.CompareTo(y.distance));

        bool found = false;
        foreach (PlayerDistance pd in players) {
            if (!player.previousTargets.Contains(pd.player.id)) {
                player.previousTargets.Add(pd.player.id);
                ServerSend.TargetSelected(from, pd.player.id);
                found = true;
                break;
            }
        }

        if (!found && players.Count > 0)
        {
            player.previousTargets.Clear();
            player.previousTargets.Add(players[0].player.id);
            ServerSend.TargetSelected(from, players[0].player.id);
        }
    }

    public static bool PlayerTradeCheck(PlayerTrade trade) {
        Inventory inventory = Server.FindPlayerByUsername(trade.player1.username).inventory;
        
        List<SerializableObjects.InventorySlot> slots = new List<SerializableObjects.InventorySlot>();
        foreach (SerializableObjects.InventorySlot slot in trade.items1) {
            SerializableObjects.InventorySlot s = slots.Where(x=>x.item.item_id==slot.item.item_id).ToList().FirstOrDefault();
            if (s != null)
            {
                s.quantity += slot.quantity;
            }
            else {
                slots.Add(new SerializableObjects.InventorySlot() { item = slot.item, quantity = slot.quantity });
            }
        }


        foreach (SerializableObjects.InventorySlot slot in slots) {
            if (InventoryQuantity(inventory, slot.item) < slot.quantity) {
                return false;
            }
        }
        return true;
    }

    public static void PlayerCharacterPosition(int from, Packet packet) {        
        Player player = Server.clients[from].player;

        if (player.playerInstance != null && !player.playerCharacter.data.dead)
        {
            player.playerInstance.transform.position = packet.ReadVector3();
            player.playerInstance.transform.rotation = packet.ReadQuaternion();

            ServerSend.PlayerCharacterPosition(from, player.playerInstance.transform.position, player.playerInstance.transform.rotation);
        }        
    }

    public static void ShipPosition(int from, Packet packet)
    {
        Player player = Server.clients[from].player;

        if (!player.data.sunk)
        {
            player.transform.position = packet.ReadVector3();
            player.transform.rotation = packet.ReadQuaternion();

            ServerSend.ShipPosition(from, player.transform.position, player.transform.rotation);
        }
    }

    public static void Jump(int from, Packet packet) {        
        ServerSend.Jump(from);
        Player player = Server.clients[from].player;

        PlayerMovement playerMovement = player.playerInstance.GetComponent<PlayerMovement>();
        playerMovement.jump = true;
    }

    public static int InventoryQuantity(Inventory inventory,SerializableObjects.Item item) {
        int quantity = 0;
        foreach (InventorySlot slot in inventory.items) {
            if (slot.item!=null && slot.item.item_id == item.item_id) {
                quantity += slot.quantity;
            }
        }
        return quantity;
    }  
}
