﻿using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{
    public void OnChatMessage(int from, Message message) {
        if (message.messageType == Message.MessageType.playerMessage)
        {
            //broadcast message
            ServerSend.ChatMessage(from, message);            
        }
        else if (message.messageType == Message.MessageType.privateMessage) {
            string to = message.to;

            bool found = false;
            foreach (Client client in Server.clients.Values) {
                if (client.player != null && client.player.data.username.Equals(to)) {
                    ServerSend.ChatMessage(from, message, client.player.id);
                    found = true;
                    break;
                }
            }

            if (!found) {
                Message msg = new Message();
                msg.messageType = Message.MessageType.gameInfo;
                msg.text = "Player not found or offline!";
                msg.to = message.from;
                ServerSend.OnGameMessage(from, msg);
            }
        }
    }
}