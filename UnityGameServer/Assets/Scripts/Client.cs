using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using static GameServer;

public class PlayerInputs 
{
    public int inputSequenceNumber;
    public bool left;
    public bool right;
    public bool forward;    
}

public class Client: MonoBehaviour
{
    public static int dataBufferSize = 4096;

    public int id;
    public Player player;
    public string ipPort;
    public int connectionId;    

    public List<PlayerInputs> inputBuffer = new List<PlayerInputs>();
    public int lastInputSequenceNumber;    

    public Client(int _clientId)
    {
        id = _clientId;
    }

    public void Connect(/*int connectionId)*/string ipPort)
    {
        this.ipPort = ipPort;
        //this.connectionId = connectionId;
        ServerSend.Hello(id);
    }    

    /// <summary>Sends the client into the game and informs other clients of the new player.</summary>
    /// <param name="_playerName">The username of the new player.</param>
    public void SendIntoGame(PlayerData data, string _playerName, int dbid)
    {        
        player = NetworkManager.instance.InstantiatePlayer(data.X_SHIP, data.Y_SHIP, data.Z_SHIP);
        player.transform.eulerAngles = new Vector3(0, data.Y_ROT_SHIP, 0);
        player.Initialize(id, dbid);

        ServerSend.SpawnShip(id, player);
        player.Load(); 
    }

    /// <summary>Disconnects the client and stops all network traffic.</summary>
    public void Disconnect()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            Destroy(player.playerInstance);
            lastInputSequenceNumber = 0;
            inputBuffer.Clear();
            player.TransferGroupOwner();
            player.RemoveGroupIfEmpty();            

            int groupId = 0;
            if (player.group != null)
                groupId = player.group.groupId;

            if (NetworkManager.trades.ContainsKey(player.id))
            {
                PlayerTrade trade = NetworkManager.trades[player.id];
                int otherPlayer = GameServer.FindPlayerByUsername(trade.player2.username).id;

                if (GameServer.clients.ContainsKey(otherPlayer))
                {
                    ServerSend.PlayerTradeCanceled(otherPlayer);
                }
                NetworkManager.trades.Remove(player.id);
                NetworkManager.trades.Remove(otherPlayer);
            }

            for (int i = 0; i < GameServer.clients.Count; i++) {
                if (GameServer.clients[i+1].player) {
                    Player p = GameServer.clients[i + 1].player;
                    if (GameServer.clients[i + 1].player.playerMovement.player && GameServer.clients[i+1].player.playerMovement.player.id == player.id) {
                        GameServer.clients[i+1].player.playerMovement.DisableAgent();
                    }
                }
            }

            player = null;

            if (groupId != 0) {
                ServerSend.GroupMembers(groupId);
            }           
        });

        NetworkManager.PlayerDisconnected(id);
        ServerSend.PlayerDisconnected(id);
        //TODO: disconnect client
        //session.Disconnect();
    }
}
