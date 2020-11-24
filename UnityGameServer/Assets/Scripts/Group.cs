using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group
{
    //!!!owner and players contain dbid not id of Player
    public int groupId;
    public string groupName;
    public int owner;
    public List<int> players = new List<int>();
    public const int maxPlayers = 6;

    private static int counter = 1;

    public Group(string groupName, Player owner) {
        groupId = counter;
        counter += 1;
        this.groupName = groupName;
        owner.ownedGroup = this;
        this.owner = owner.dbid;        
        NetworkManager.groups.Add(groupId, this);
        AddPlayer(owner);
    }

    public void AddPlayer(Player player) {
        if (players.Count == maxPlayers)
            return;

        if (!players.Contains(player.dbid))
            players.Add(player.dbid);

        player.group = this;

        ServerSend.GroupMembers(groupId);
    }

    public void RemovePlayer(Player player)
    {
        if (players.Contains(player.dbid))
            players.Remove(player.dbid);

        player.group = null;
        player.ownedGroup = null;

        ServerSend.GroupMembers(groupId);
    }

    public void Disband() {
        foreach (int dbid in players)
        {
            Player player = Server.FindPlayerByDBid(dbid);
            if (player != null)
                player.group = null;
        }

        Player owner = Server.FindPlayerByDBid(this.owner);
        if(owner!=null)
            owner.ownedGroup = null;
        players.Clear();
        NetworkManager.groups.Remove(groupId);
    }
}
