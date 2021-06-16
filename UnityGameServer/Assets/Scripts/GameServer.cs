using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Linq;
using WatsonTcp;

public class GameServer
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    /*public static Dictionary<int, PacketHandler> packetHandlers;*/
    public static Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();

    public static WatsonTcpServer server;
    //public static AsyncTCPServer.AsyncTCPServer server;

    /// <summary>Starts the server.</summary>
    /// <param name="_maxPlayers">The maximum players that can be connected simultaneously.</param>
    /// <param name="_port">The port to start the server on.</param>
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        InitializeServerData();
        server = new WatsonTcpServer("0.0.0.0", _port);
        server.Events.ExceptionEncountered += Events_ExceptionEncountered;
        server.Events.ClientConnected += ClientConnected;
        server.Events.ClientDisconnected += ClientDisconnected;
        server.Events.MessageReceived += MessageReceived;
        server.Start();

        /*server = new AsyncTCPServer.AsyncTCPServer(IPAddress.Any, _port);
        server.OnClientConnected += Server_OnClientConnected;
        server.OnClientDisconnected += Server_OnClientDisconnected;
        server.OnReceived += Server_OnReceived;        
        server.Start();*/

        Debug.Log($"Server started on port {Port}");
    }

    /*private static void Server_OnLog(object sender, string message)
    {
        Debug.Log(message);
    }

    private static void Server_OnReceived(object sender, AsyncTCPServer.AsyncTCPServer.ReceivedEventArgs e)
    {
        foreach (AsyncTCPServer.Message message in e.Messages){
            ThreadManager.ExecuteOnMainThread(() =>
            {
                byte[] packetData = message.data;
                Packet _packet = new Packet(packetData);
                int _packetId = _packet.ReadInt();
                Client client = FindClientByConnectionId(e.ClientID);
                NetworkManager.AddPacket(client.id, _packetId, _packet);
            });
        }
    }

    private static void Server_OnClientDisconnected(object sender, AsyncTCPServer.AsyncTCPServer.ClientArgs e)
    {
        Debug.Log($"Client {e.ClientID} disconnected!");
        Client client = FindClientByConnectionId(e.ClientID);
        client.Disconnect();
    }

    private static void Server_OnClientConnected(object sender, AsyncTCPServer.AsyncTCPServer.ClientArgs e)
    {
        Debug.Log($"Client {e.ClientID} connected!");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (!clients[i].player)
            {
                Client client = clients[i];
                clients[i].Connect(e.ClientID);
                return;
            }
        }

        //disconnect player if no room
        //TODO
    }*/

    private static void Events_ExceptionEncountered(object sender, ExceptionEventArgs e)
    {
        Debug.LogError(e.Exception);
        Debug.LogError(e.Json);
    }

    private static void MessageReceived(object sender, MessageReceivedEventArgs e)
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            byte[] packetData = e.Data;
            Packet _packet = new Packet(packetData);
            int _packetId = _packet.ReadInt();
            Client client = FindClientByIpPort(e.IpPort);            
            NetworkManager.AddPacket(client.id, _packetId, _packet);
        });
    }

    private static void ClientDisconnected(object sender, DisconnectionEventArgs e)
    {
        Debug.Log($"Client from {e.IpPort} disconnected!");
        Client client = FindClientByIpPort(e.IpPort);        
        client.Disconnect();
    }

    private static void ClientConnected(object sender, ConnectionEventArgs e)
    {
        Debug.Log($"Client from {e.IpPort} connected!");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (!clients[i].player)
            {
                Client client = clients[i];
                clients[i].Connect(e.IpPort);
                return;
            }
        }

        //disconnect player if no room
        server.DisconnectClient(e.IpPort);        
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {            
            clients.Add(i, new Client(i));
        }
    }

    public static void StopServer()
    {        
        foreach (Client client in clients.Values) {
            client.Disconnect();            
        }
        server.Stop();
    }

    public static Client FindClientByIpPort(string ipPort) {
        foreach (Client client in GameServer.clients.Values)
        {
            if (client.ipPort.Equals(ipPort))
                return client;
        }
        return null;
    }

    public static Client FindClientByConnectionId(int connectionId)
    {
        foreach (Client client in GameServer.clients.Values)
        {
            if (client.connectionId == connectionId)
                return client;
        }
        return null;
    }

    public static Player FindPlayerByDBid(int dbid) {
        foreach (Client client in clients.Values) {
            if (client.player != null && client.player.dbid == dbid)
                return client.player;
        }
        return null;
    }

    public static Player FindPlayerByUsername(string username)
    {
        foreach (Client client in clients.Values)
        {
            if (client.player != null && client.player.data.username.Equals(username))
                return client.player;
        }
        return null;
    }

    public static int GetOtherPlayer(PlayerTrade trade, int from) {
        int player1 = FindPlayerByUsername(trade.player1.username).id;
        int player2 = FindPlayerByUsername(trade.player2.username).id;
        int otherPlayer = 0;
        if (player1 == from)
        {
            otherPlayer = player2;
        }
        else
        {
            otherPlayer = player1;
        }
        return otherPlayer;
    }    
}
