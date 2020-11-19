using SerializableObjects;
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

            foreach (Client client in Server.clients.Values) {
                if (client.player != null && client.player.data.username.Equals(to)) {
                    ServerSend.ChatMessage(from, message, client.player.id);
                }
            }
        }
    }
}
