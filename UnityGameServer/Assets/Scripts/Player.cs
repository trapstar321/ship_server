using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public int dbid;    
    public CharacterController controller;
    //private Rigidbody mBody;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float throwForce = 600f;    
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public float moveForce = 10f;

    private bool[] inputs;
    private float yVelocity = 0;
    private float joystickVertical = 0;
    private float joystickHorizontal = 0;

    private Vector3 newPosition;
    private Quaternion newRotation;

    private float visibilityRadius;

    public Inventory inventory;
    public ShipEquipment ship_equipment;
    public PlayerEquipment player_equipment;

    public List<BaseStat> stats;
    public List<Experience> exp;
    public PlayerData data;

    public float attack;
    public float health;
    public float defence;
    public float rotation;
    public float speed;
    public float visibility;
    public float cannon_reload_speed;
    public float crit_chance;
    public float cannon_force;


    BoatMovement movement;
    SphereCollider playerEnterCollider;

    public List<ItemDrop> lootCache;

    void Awake() {
        //mBody = GetComponent<Rigidbody>();        
        visibilityRadius = NetworkManager.visibilityRadius;
        //Instantiate(inventory);
        inventory = GetComponent<Inventory>();
        ship_equipment = GetComponent<ShipEquipment>();
        player_equipment = GetComponent<PlayerEquipment>();

        movement = GetComponent<BoatMovement>();
        playerEnterCollider = GetComponentInChildren<SphereCollider>();
        playerEnterCollider.radius = NetworkManager.visibilityRadius / 2;
    }

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, int _dbid)
    {
        id = _id;
        dbid = _dbid;        
        health = maxHealth;

        inputs = new bool[5];                     
    }

    public void Load()
    {
        Mysql mysql = FindObjectOfType<Mysql>();

        List<BaseStat> stats = mysql.ReadBaseStatsTable();
        List<Experience> exp = mysql.ReadExperienceTable();
        PlayerData data = mysql.ReadPlayerData(id);

        this.stats = stats;
        this.exp = exp;
        this.data = data;

        LoadBaseStats();

        ServerSend.OnGameStart(id, stats, exp, data);

        LoadInventory();
        LoadPlayerEquipment();
        LoadShipEquipment();

        //send stats after all is loaded
        ServerSend.Stats(id);
    }

    public void Update()
    {
        
    }

    /// <summary>Processes player input and moves the player.</summary>
    /*public void FixedUpdate()
    {
        transform.position = newPosition;
        transform.rotation = newRotation;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }*/

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        /*Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);*/        

        /*mBody.velocity = new Vector3(joystickHorizontal * moveForce,
                                     mBody.velocity.y,
                                     joystickVertical * moveForce);*/

        /*mBody.velocity = new Vector3(mBody.velocity.x,
                                     mBody.velocity.y,
                                     joystickVertical * moveForce);

        mBody.transform.Rotate(Vector3.up, joystickHorizontal * 90f * Time.deltaTime);*/

        /*ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);*/
    }

    /*public void SetPosition(Vector3 position, Quaternion rotation) {
        transform.position = position;
        transform.rotation = rotation;

        ServerSend.PlayerPosition(this, visibilityRadius);        
    }*/

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void SetInput(float vertical, float horizontal)
    {
        joystickVertical = vertical;
        joystickHorizontal = horizontal;

        if (vertical != 0 || horizontal != 0) {
            Debug.Log($"Vertical={vertical}, Horizontal={horizontal}");
        }
    }    

    private void TakeDamage(Player player)
    {
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < player.crit_chance / 100)
        {
            damage = player.attack * 2 - defence;
            health -= damage;
        }

        else
        {
            damage = player.attack - defence;
            health -= damage;
        }
        ServerSend.TakeDamage(id, transform.position, damage, "player");
    }

    private void TakeDamage(EnemyAI npc)
    {
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < npc.crit_chance / 100)
        {
            damage = npc.attack * 2 - defence;
            health -= damage;
        }

        else
        {
            damage = npc.attack - defence;
            health -= damage;
        }
        ServerSend.TakeDamage(id, transform.position, damage, "player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("CannonBall"))
        {
            Player player = other.gameObject.GetComponent<CannonBall>().player;
            EnemyAI npc = other.gameObject.GetComponent<CannonBall>().npc;

            Vector3 tempPos = other.transform.position - new Vector3(0f, 0.5f, 0f);
            
            if (player==null)
                TakeDamage(npc);
            else
                TakeDamage(player);

            Debug.Log("Hit by " + other.name);
            other.gameObject.SetActive(false);
        }
        else if (other.name.Equals("Sphere")) {
            int otherPlayerId = other.GetComponentInParent<Player>().id;            
            ServerSend.Stats(otherPlayerId, id);

            CannonController cannonController = other.GetComponentInParent<CannonController>();
            Quaternion leftRotation = cannonController.L_Cannon_1.transform.localRotation;
            Quaternion rightRotation = cannonController.R_Cannon_1.transform.localRotation;
            ServerSend.CannonRotate(otherPlayerId, id, leftRotation, "Left");
            ServerSend.CannonRotate(otherPlayerId, id, rightRotation, "Right");
        }
        else if (other.name.Equals("NPCSphere"))
        {
            int npcId = other.GetComponentInParent<EnemyAI>().id;
            ServerSend.NPCStats(npcId, id);
        }
    }

    public void SearchChest() {
        RaycastHit hit;        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 10))
        {
            string s = "";
        }
    }

    protected void LoadBaseStats() {
        int level = data.level;

        foreach (BaseStat stat in stats) {
            if (stat.level == level)
            {
                attack = stat.attack;
                health = stat.health;
                maxHealth = stat.health;
                defence = stat.defence;
                rotation = stat.rotation;
                speed = stat.speed;
                visibility = stat.visibility;
                cannon_reload_speed = stat.cannon_reload_speed;
                crit_chance = stat.crit_chance;
                cannon_force = stat.cannon_force;
            }
        }
    }

    public void AddEquipment(Item item) {
        attack += item.attack;
        health += item.health;
        maxHealth += item.health;
        defence += item.defence;
        rotation += item.rotation;
        speed += item.speed;
        visibility += item.visibility;
        cannon_reload_speed += item.cannon_reload_speed;
        crit_chance += item.crit_chance;
        cannon_force += item.cannon_force;

        ServerSend.Stats(id);
    }

    public void RemoveEquipment(Item item) {
        attack -= item.attack;
        health -= item.health;
        maxHealth -= item.health;
        defence -= item.defence;
        rotation -= item.rotation;
        speed -= item.speed;
        visibility -= item.visibility;
        cannon_reload_speed -= item.cannon_reload_speed;
        crit_chance -= item.crit_chance;
        cannon_force -= item.cannon_force;

        ServerSend.Stats(id);
    }

    public void LoadInventory() {
        Mysql mysql = FindObjectOfType<Mysql>();
        Player player = Server.clients[id].player;
        List<InventorySlot> slots = mysql.ReadInventory(player.dbid);
        Inventory inventory = player.inventory;

        foreach (InventorySlot s in slots)
        {
            inventory.Add(s);
        }
    }

    public void LoadShipEquipment() {
        Mysql mysql = FindObjectOfType<Mysql>();
        Player player = Server.clients[id].player;
        List<Item> items = mysql.ReadShipEquipment(player.dbid);
        ShipEquipment equipment = player.ship_equipment;

        foreach (Item item in items)
        {
            equipment.Add(item);
        }
    }

    public void LoadPlayerEquipment() {
        Mysql mysql = FindObjectOfType<Mysql>();
        Player player = Server.clients[id].player;
        List<Item> items = mysql.ReadPlayerEquipment(player.dbid);
        PlayerEquipment equipment = player.player_equipment;

        foreach (Item item in items)
        {
            equipment.Add(item);
        }
    }

    public void Move(Vector3 newPos)
    {
        movement.buffer.Add(newPos);
    }
}
