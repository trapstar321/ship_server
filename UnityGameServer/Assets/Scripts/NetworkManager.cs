using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class NetworkManager : MonoBehaviour
{
    public static ServerSend send;
    public static NetworkManager instance;
    public static Waves wavesScript;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject projectilePrefab;
    public static float visibilityRadius = 50;

    float lastPositionUpdateTime = -1;
    float positionUpdateDifference = 5;

    Mysql mysql;

    public static Dictionary<int, Group> groups = new Dictionary<int, Group>();
    public static Dictionary<string, int> invitationLinks = new Dictionary<string, int>();

    public class PacketData {
        public int type;
        public Packet packet;
    }

    public static Dictionary<int, List<PacketData>> buffer = new Dictionary<int, List<PacketData>>();

    private void Awake()
    {
        send = FindObjectOfType<ServerSend>();
        StartCoroutine(Tick());
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

    private void Update()
    {
        ServerSend.Time(Time.deltaTime);
        UpdatePlayerPosition();
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer(float x, float y, float z)
    {
        return Instantiate(playerPrefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<Player>();
    }

    public static void SendStats(int from)
    {
        //send player stats to all
        ServerSend.Stats(from);

        //send all player stats to player
        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null && client.id != from)
                ServerSend.Stats(client.id, from);
        }
    }

    public static void SendNPCBaseStats(int to) {
        Mysql mysql = FindObjectOfType<Mysql>();
        List<BaseStat> stats = mysql.ReadNPCBaseStatsTable();

        ServerSend.BaseStats(to, stats, "npc");
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
                    ProcessMovementPackets(client);
                }
            }
            yield return new WaitForSeconds(1 / 50);
        }
    }

    void UpdatePlayerPosition()
    {
        if (Time.time - lastPositionUpdateTime < positionUpdateDifference && lastPositionUpdateTime != -1)
            return;

        lastPositionUpdateTime = Time.time;

        foreach (Client client in Server.clients.Values)
        {
            Player player = client.player;

            if (player != null)
            {               
                mysql.UpdateShipPosition(player.dbid, player.transform.position.x, player.transform.position.y, player.transform.position.z, player.transform.eulerAngles.y);

                if (!player.data.is_on_ship) {
                    Transform transform = player.playerInstance.transform;
                    mysql.UpdatePlayerPosition(player.dbid, transform.position.x, transform.position.y, transform.position.z, transform.eulerAngles.y);
                }
            }
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
                    case (int)ClientPackets.addShipEquipment:
                        ServerHandle.AddShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.removeShipEquipment:
                        ServerHandle.RemoveShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.getShipEquipment:
                        ServerHandle.GetShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.replaceShipEquipment:
                        ServerHandle.ReplaceShipEquipment(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.addItemToInventory:
                        ServerHandle.AddItemToInventory(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.removeItemFromInventory:
                        ServerHandle.RemoveItemFromInventory(client.id, packet.packet);
                        break;
                    case (int)ClientPackets.removePlayerEquipment:
                        ServerHandle.RemovePlayerEquipment(client.id, packet.packet);
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
                }
            }
            catch (Exception ex) {
                Debug.LogError("NetworkManager.cs ProcessBuffer(): "+ex.Message+" "+ex.StackTrace);
            }
        }

        buffer[client.id].RemoveRange(0, end);
    }

    void ProcessMovementPackets(Client client) {
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
            client.player.Move(new Vector3(input.left ? 1 : 0, input.right ? 1 : 0, input.forward ? 1 : 0));
            //Debug.Log("SN " + input.inputSequenceNumber + " moveCount = " + moveCount + $" position={client.player.transform.position} move {input.left},{input.right},{input.forward}");
            moveCount += 1;
            //client.inputBuffer.RemoveAt(i);                        
        }
        if (end != 0)
            client.inputBuffer.RemoveRange(0, end);

        if (lastInputSequenceNumber != 0)
        {
            client.lastInputSequenceNumber = lastInputSequenceNumber;
            send.PlayerPosition(lastInput, client.lastInputSequenceNumber, client.player, visibilityRadius);
            //Debug.Log("SN " + client.lastInputSequenceNumber + ", position=" + client.player.transform.position);
        }
        //Debug.Log("LSN:" + client.lastInputSequenceNumber);
    }

    public static void PlayerDisconnected(int id) {
        buffer.Remove(id);
    }
}