using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int id;
    public PlayerEquipment equipment;
    public PlayerData data;
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

            if (otherPlayerId != id) {
                bool isOnShip = Server.clients[otherPlayerId].player.data.is_on_ship;

                if (isOnShip) {
                    ServerSend.DestroyPlayerCharacter(id, otherPlayerId);
                }
                ServerSend.Stats(otherPlayerId, id);
            }
        }
        else if (other.name.Equals("PlayerSphere"))
        {
            int otherPlayerId = other.GetComponentInParent<PlayerCharacter>().id;

            if (otherPlayerId != id)
            {
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
    }
}
