﻿using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int id;
    public PlayerEquipment equipment;
    public PlayerData data;
    public Weapon currentWeapon;
    public bool weaponEnabled;
    // Start is called before the first frame update

    public float maxHealth = 100f;
    public float attack;
    public float health;
    public float defence;    
    public float speed;
    public float crit_chance;
    public float energy;

    public List<PlayerBaseStat> stats;

    private Mysql mysql;

    public bool isOnDock = false;

    public bool gatheringEnabled;
    public bool craftingEnabled;
    public GameObject currentResource;
    public bool tradingEnabled;
    public bool tradeBrokerEnabled;
    public CraftingSpot craftingSpot;
    public Trader trader;    

    private void Awake()
    {
        equipment = GetComponent<PlayerEquipment>();
        mysql = FindObjectOfType<Mysql>();        
    }

    public void Load()
    {
        this.stats = mysql.ReadPlayerBaseStatsTable();
        LoadBaseStats();
        LoadPlayerEquipment();
    }

    public void LoadPlayerEquipment()
    {
        Mysql mysql = FindObjectOfType<Mysql>();
        Player player = Server.clients[id].player;
        List<Item> items = mysql.ReadPlayerEquipment(player.dbid);        

        foreach (Item item in items)
        {
            equipment.Add(item);
        }
    }

    protected void LoadBaseStats()
    {
        int level = data.level;

        foreach (PlayerBaseStat stat in stats)
        {
            if (stat.level == level)
            {
                attack = stat.attack;
                health = stat.health;
                maxHealth = stat.health;
                defence = stat.defence;                
                speed = stat.speed;
                crit_chance = stat.crit_chance;
                energy = stat.energy;
            }
        }
    }

    public void AddEquipment(Item item)
    {
        attack += item.attack;
        health += item.health;
        maxHealth += item.health;
        defence += item.defence;
        speed += item.speed;
        crit_chance += item.crit_chance;        

        ServerSend.Stats(id);
    }

    public void RemoveEquipment(Item item)
    {
        attack -= item.attack;
        health -= item.health;
        maxHealth -= item.health;
        defence -= item.defence;        
        speed -= item.speed;
        crit_chance -= item.crit_chance;
        
        ServerSend.Stats(id);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Equals("Sphere"))
        {
            int otherPlayerId = other.GetComponentInParent<Player>().id;

            if (otherPlayerId != id)
            {
                bool isOnShip = Server.clients[otherPlayerId].player.data.is_on_ship;

                /*if (isOnShip)
                {
                    ServerSend.DestroyPlayerCharacter(id, otherPlayerId);
                }*/
                ServerSend.ActivateShip(id, otherPlayerId);
                ServerSend.Stats(otherPlayerId, id);
            }
        }
        else if (other.name.Equals("PlayerSphere"))
        {
            int otherPlayerId = other.GetComponentInParent<PlayerCharacter>().id;

            if (otherPlayerId != id)
            {
                ServerSend.ActivatePlayerCharacter(id, otherPlayerId);
                ServerSend.Stats(otherPlayerId, id);
            }
        }
        else if (other.tag.Equals("Resource"))
        {
            gatheringEnabled = true;
            currentResource = other.gameObject;
        }
        else if (other.tag.Equals("Dock"))
        {
            isOnDock = true;
        }
        else if (other.tag.Equals("CraftingSpot"))
        {
            craftingEnabled = true;
            craftingSpot = other.GetComponent<CraftingSpot>();
        }
        else if (other.tag == "Trader")
        {
            tradingEnabled = true;
            trader = other.gameObject.GetComponent<Trader>();
        }
        else if (other.tag == "TradeBroker")
        {
            tradeBrokerEnabled = true;
        }
        else if (other.tag == "Weapon") {
            PlayerCharacter otherPlayer = other.GetComponent<Weapon>().player.GetComponent<PlayerCharacter>();
            if (otherPlayer.id != id) {
                CharacterAnimationController animationController = otherPlayer.GetComponentInChildren<CharacterAnimationController>();
                if (animationController.currentAttack != null && !animationController.currentAttack.done)
                {
                    Debug.Log("Attack "+animationController.currentAttack.attackName);
                    animationController.currentAttack.done = true;
                    OnPlayerAttack(otherPlayer, animationController.currentAttack);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Resource"))
        {
            currentResource = null;
            gatheringEnabled = false;
        }
        else if (other.tag.Equals("Dock"))
        {
            isOnDock = false;
        }
        else if (other.tag.Equals("CraftingSpot"))
        {
            craftingEnabled = false;
            craftingSpot = null;
        }
        else if (other.tag == "Trader")
        {
            tradingEnabled = false;
            trader = null;
        }
        else if (other.tag == "TradeBroker")
        {
            tradeBrokerEnabled = false;
        }
        else if (other.name.Equals("PlayerSphere"))
        {
            int otherPlayerId = other.GetComponentInParent<PlayerCharacter>().id;

            if (otherPlayerId != id)
            {
                ServerSend.DeactivatePlayerCharacter(id, otherPlayerId);                
            }
        }
        else if (other.name.Equals("Sphere"))
        {
            int otherPlayerId = other.GetComponentInParent<Player>().id;

            if (otherPlayerId != id)
            {                
                ServerSend.DeactivateShip(id, otherPlayerId);
            }
        }
    }

    private void OnPlayerAttack(PlayerCharacter player, PlayerAttack attack) {
        if (data.dead)
            return;

        bool crit = false;
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < player.crit_chance / 100)
        {
            crit = true;
            damage = (player.attack * 2 - defence)*attack.multiplier;
            health -= damage;
        }
        else
        {
            damage = (player.attack - defence)*attack.multiplier;
            health -= damage;
        }

        if (health <= 0) {
            health = 0;
            Die();
        }

        ServerSend.TakeDamage(id, transform.position, damage, "character", crit);
    }

    public void Die() {
        Debug.Log("Die");
        data.dead = true;
        mysql.DiePlayerCharacter(Server.clients[id].player.dbid);
        //gameObject.SetActive(false);        
        ServerSend.DiePlayerCharacter(id, data);
    }

    public void Respawn() {
        data.dead = false;
        gameObject.transform.position = NetworkManager.instance.respawnPointCharacter.transform.position;
        mysql.RespawnPlayerCharacter(Server.clients[id].player.dbid);
        health = maxHealth;        
        ServerSend.RespawnPlayerCharacter(id, data);
        ServerSend.Stats(id);
    }

    public float respawnUpdateTime;
    public float respawnTime = 10;

    public void Update()
    {
        if (data.dead)
        {
            if (Time.time - respawnUpdateTime < respawnTime)
                return;

            respawnUpdateTime = Time.time;
            Debug.Log("Respawn");
            Respawn();
        }
        else {
            respawnUpdateTime = Time.time;
        }
    }
}
