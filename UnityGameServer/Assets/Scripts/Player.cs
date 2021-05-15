using SerializableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Threading.Tasks;

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
    //public PlayerEquipment player_equipment;

    public List<ShipBaseStat> stats;
    public List<Experience> exp;
    public PlayerData data;
    public List<PlayerSkillLevel> skills;

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
    public Group group;
    public Group ownedGroup;
    
    private bool isOnDock;
    public GameObject playerPrefab;
    public GameObject playerInstance;
    public GameObject dock;
    private Mysql mysql;
    
    public List<int> previousTargets = new List<int>();
    public PlayerCharacter playerCharacter;
    public CannonShot cannonShot;
    public CannonController cannonController;
    public PlayerMovement playerMovement;
    public SpawnManager spawnManager;

    void Awake() {
        //mBody = GetComponent<Rigidbody>();        
        visibilityRadius = NetworkManager.visibilityRadius;
        //Instantiate(inventory);
        inventory = GetComponent<Inventory>();
        ship_equipment = GetComponent<ShipEquipment>();
        //player_equipment = GetComponent<PlayerEquipment>();

        movement = GetComponent<BoatMovement>();
        playerEnterCollider = GetComponentInChildren<SphereCollider>();
        playerEnterCollider.radius = NetworkManager.visibilityRadius / 2;
        mysql = FindObjectOfType<Mysql>();
        cannonShot = GetComponent<CannonShot>();
        cannonController = GetComponent<CannonController>();
        spawnManager = FindObjectOfType<SpawnManager>();
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

        ResetGroupReferences();
        TransferGroupOwner();
        SetGroupOwner();
    }

    public void Load()
    {
        Mysql mysql = FindObjectOfType<Mysql>();

        Action ac = () =>
        {
            try { 
                List<ShipBaseStat> stats = mysql.ReadShipBaseStatsTable();
                List<Experience> exp = mysql.ReadExperienceTable();
                PlayerData data = mysql.ReadPlayerData(dbid);
                List<PlayerSkillLevel> skills = mysql.ReadPlayerSkills(dbid);

                this.stats = stats;
                this.exp = exp;
                this.data = data;
                this.skills = skills;

                if (!NetworkManager.traders.ContainsKey(dbid))
                {
                    NetworkManager.traders.Add(dbid, mysql.ReadTraders());
                }
                List<SerializableObjects.Item> shipEquipment = LoadShipEquipment(mysql);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    LoadBaseStats();

                    //if (!data.is_on_ship) {
                    playerInstance = Instantiate(playerPrefab, new Vector3(data.X_PLAYER, data.Y_PLAYER, data.Z_PLAYER), Quaternion.identity);
                    playerCharacter = playerInstance.GetComponent<PlayerCharacter>();
                    playerMovement = playerInstance.GetComponent<PlayerMovement>();
                    playerCharacter.id = id;
                    playerCharacter.data = data;
                    playerInstance.transform.Find("PlayerSphere").GetComponent<SphereCollider>().radius = NetworkManager.visibilityRadius / 2;
                    playerInstance.transform.eulerAngles = new Vector3(0, data.Y_ROT_PLAYER, 0);
                    playerCharacter.pirate.transform.rotation = Quaternion.Euler(0, data.Y_ROT_PLAYER_CHILD, 0);

                    if (data.is_on_ship)
                        playerInstance.SetActive(false);                                       

                    ServerSend.OnGameStart(id, stats, playerCharacter.stats, exp, data);
                    playerCharacter.Load();

                    ShipEquipment equipment = ship_equipment;

                    foreach (SerializableObjects.Item item in shipEquipment)
                    {
                        equipment.Add(NetworkManager.SerializableToItem(item));
                    }

                    // Send the new player to all players
                    foreach (Client _client in Server.clients.Values)
                    {
                        if (_client.player != null)
                        {
                            if (_client.player.id != id)
                                ServerSend.SpawnShip(_client.id, this);
                        }
                    }

                    // Send all players to the new player
                    foreach (Client _client in Server.clients.Values)
                    {
                        if (_client.player != null)
                        {
                            if (_client.id != id)
                            {
                                ServerSend.SpawnShip(id, _client.player);
                            }
                        }
                    }

                    // Send the new player to all players except himself        
                    foreach (Client _client in Server.clients.Values)
                    {
                        if (_client.player != null)
                        {
                            if (_client.player.id != id)
                            {
                                //ServerSend.SpawnPlayer(_client.id, player);                        
                                ServerSend.InstantiatePlayerCharacter(_client.id, id,
                                    this.playerInstance.transform.position,
                                    this.playerInstance.transform.eulerAngles.y);
                            }
                        }
                    }

                    // Send all players to the new player
                    foreach (Client _client in Server.clients.Values)
                    {
                        if (_client.player != null)
                        {
                            if (_client.id != id)
                            {
                                /*if(!_client.player.data.is_on_ship)
                                    ServerSend.SpawnPlayer(id, _client.player);*/
                                ServerSend.InstantiatePlayerCharacter(id, _client.player.id,
                                        _client.player.playerInstance.transform.position,
                                        _client.player.playerInstance.transform.eulerAngles.y);
                            }
                        }
                    }

                    spawnManager.SendAllGameObjects(id);
                    ServerSend.Recipes(id);

                    //send stats after all is loaded
                    ServerSend.Stats(id);
                });
                
                //LoadPlayerEquipment();              
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        };        

        Task task = new Task(ac);
        task.Start();  

        LoadInventory();        
    }

    public void Update()
    {
        if (data.sunk)
        {
            if (Time.time - respawnUpdateTime < respawnTime)
                return;

            respawnUpdateTime = Time.time;
            Debug.Log("Respawn");
            Respawn();
        }
        else
        {
            respawnUpdateTime = Time.time;
        }
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
        if (data.sunk)
            return;

        bool crit = false;
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < player.crit_chance / 100)
        {
            crit = true;
            damage = player.attack * 2 - defence;
            health -= damage;
        }

        else
        {
            damage = player.attack - defence;
            health -= damage;
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }

        ServerSend.TakeDamage(id, transform.position, damage, "ship", crit);
        if(group!=null)
            ServerSend.GroupMembers(group.groupId);
    }

    private void TakeDamage(ShipNPC npc)
    {
        if (data.sunk)
            return;

        bool crit = false;
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < npc.crit_chance / 100)
        {
            crit = true;
            damage = npc.attack * 2 - defence;
            health -= damage;
        }

        else
        {
            damage = npc.attack - defence;
            health -= damage;
        }
        ServerSend.TakeDamage(id, transform.position, damage, "ship", crit);
        if (group != null)
            ServerSend.GroupMembers(group.groupId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("CannonBall"))
        {
            Player player = other.gameObject.GetComponent<CannonBall>().player;
            ShipNPC npc = other.gameObject.GetComponent<CannonBall>().npc;

            Vector3 tempPos = other.transform.position - new Vector3(0f, 0.5f, 0f);

            if (player == null)
                TakeDamage(npc);
            else
                TakeDamage(player);

            Debug.Log("Hit by " + other.name);
            other.gameObject.SetActive(false);
        }
        else if (other.name.Equals("Sphere"))
        {
            int otherPlayerId = other.GetComponentInParent<Player>().id;

            if (otherPlayerId != id)
            {
                ServerSend.Stats(otherPlayerId, id);
                ServerSend.Buffs(otherPlayerId, id);

                CannonController cannonController = other.GetComponentInParent<CannonController>();
                Quaternion leftRotation = cannonController.L_Cannon_1.transform.localRotation;
                Quaternion rightRotation = cannonController.R_Cannon_1.transform.localRotation;
                ServerSend.CannonRotate(otherPlayerId, id, leftRotation, "Left");
                ServerSend.CannonRotate(otherPlayerId, id, rightRotation, "Right");

                if(data.is_on_ship)
                    ServerSend.ActivateShip(id, otherPlayerId);
                /*bool isOnShip = Server.clients[otherPlayerId].player.data.is_on_ship;
                if (isOnShip)
                    ServerSend.DestroyPlayerCharacter(id, otherPlayerId);*/
            }
        }
        else if (other.name.Equals("PlayerSphere"))
        {
            int otherPlayerId = other.GetComponentInParent<PlayerCharacter>().id;

            if (otherPlayerId != id)
            {
                /*GameObject playerCharacter = Server.clients[otherPlayerId].player.playerInstance;
                
                Vector3 position = playerCharacter.transform.position;*/

                /*bool isOnShip = Server.clients[otherPlayerId].player.data.is_on_ship;
                if (!isOnShip)
                {*/
                //ServerSend.InstantiatePlayerCharacter(id, otherPlayerId, position, playerCharacter.transform.eulerAngles.y);
                //}*/                
                if (data.is_on_ship)
                {
                    ServerSend.ActivatePlayerCharacter(id, otherPlayerId);
                    Player player = Server.clients[otherPlayerId].player;
                    if (player.playerMovement.agent.enabled)
                    {
                        ServerSend.DeactivatePlayerMovement(otherPlayerId, player.playerInstance.transform.position);
                    }
                    else {
                        ServerSend.ActivatePlayerMovement(otherPlayerId, player.playerInstance.transform.position);
                    }
                }
                ServerSend.Stats(otherPlayerId, id);
                ServerSend.Buffs(otherPlayerId, id);
            }
        }
        else if (other.name.Equals("NPCSphere"))
        {
            if (data.is_on_ship)
            {
                int npcId = other.GetComponentInParent<NPC>().id;
                ServerSend.NPCStats(npcId, id);
                ServerSend.ActivateNPC(id, npcId);
            }
        }
        else if (other.tag.Equals("Dock"))
        {
            dock = other.gameObject;
            isOnDock = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Dock"))
        {
            isOnDock = false;
            dock = null;
        }
        else if (other.name.Equals("PlayerSphere"))
        {
            int otherPlayerId = other.GetComponentInParent<PlayerCharacter>().id;

            if (otherPlayerId != id)
            {
                if (data.is_on_ship)
                    ServerSend.DeactivatePlayerCharacter(id, otherPlayerId);
            }
        }
        else if (other.name.Equals("Sphere"))
        {
            int otherPlayerId = other.GetComponentInParent<Player>().id;

            if (otherPlayerId != id)
            {
                if (data.is_on_ship)
                    ServerSend.DeactivateShip(id, otherPlayerId);
            }
        }
        else if (other.name.Equals("NPCSphere"))
        {
            if (data.is_on_ship)
            {
                int npcId = other.GetComponentInParent<NPC>().id;
                ServerSend.DeactivateNPC(id, npcId);
            }
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

        foreach (ShipBaseStat stat in stats) {
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

    public List<SerializableObjects.Item> LoadShipEquipment(Mysql mysql) {        
        Player player = Server.clients[id].player;
        return mysql.ReadShipEquipment(player.dbid);
        /*ShipEquipment equipment = player.ship_equipment;

        foreach (SerializableObjects.Item item in items)
        {            
            equipment.Add(NetworkManager.SerializableToItem(item));
        }*/
    }

    /*public void LoadPlayerEquipment() {
        Mysql mysql = FindObjectOfType<Mysql>();
        Player player = Server.clients[id].player;
        List<Item> items = mysql.ReadPlayerEquipment(player.dbid);
        PlayerEquipment equipment = player.player_equipment;

        foreach (Item item in items)
        {
            equipment.Add(item);
        }
    }*/

    public void Move(BoatMovement.MovementOrder newPos)
    {
        movement.buffer.Add(newPos);
    }

    public void ResetGroupReferences() {
        foreach (Group group in NetworkManager.groups.Values) {
            if (group.owner == dbid)
                ownedGroup = group;

            if (group.players.Contains(dbid))
                this.group = group;
        }
    }

    public void RemoveGroupIfEmpty()
    {
        if (ownedGroup != null && NetworkManager.groups.ContainsKey(ownedGroup.groupId))
        {
            Group group = NetworkManager.groups[ownedGroup.groupId];
            if (group.players.Count == 1)
            {
                NetworkManager.groups.Remove(group.groupId);
            }            
        }
    }

    public void TransferGroupOwner() {
        if (group != null)
        {
            Group group = this.group;

            foreach (int dbid in group.players) {
                if (dbid != this.dbid) {
                    Player player = Server.FindPlayerByDBid(dbid);

                    if (player != null)
                    {
                        group.owner = dbid;
                        player.ownedGroup = group;
                        break;
                    }
                }
            }
        }
    }

    public void SetGroupOwner()
    {
        if (group != null)
        {
            Group group = this.group;

            bool aloneOnline = true;
            foreach (int dbid in group.players)
            {
                if (dbid != this.dbid && Server.FindPlayerByDBid(dbid) != null)
                {
                    aloneOnline = false;
                }
            }

            if (aloneOnline) {
                group.owner = this.dbid;
                ownedGroup = group;
            }
        }
    }

    public void LeaveEnterShip() {
        if (isOnDock && (dock == playerCharacter.dock || playerCharacter.dock == null)) {
            Vector3 spawnPosition = dock.transform.Find("SpawnPosition").transform.position;

            if (data.is_on_ship)
            {
                /*playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                playerInstance.transform.Find("PlayerSphere").GetComponent<SphereCollider>().radius = NetworkManager.visibilityRadius / 2;
                playerInstance.GetComponent<PlayerCharacter>().id = id;*/
                playerInstance.SetActive(true);
                playerInstance.transform.position = spawnPosition;
                data.is_on_ship = false;
                ServerSend.LeaveShip(id, playerInstance.transform.position, playerInstance.transform.eulerAngles.y);
            }
            else if (playerInstance.GetComponent<PlayerCharacter>().isOnDock)
            {                
                ServerSend.EnterShip(id, transform.position);
                //Destroy(playerInstance);
                playerInstance.SetActive(false);
                data.is_on_ship = true;                
            }
            mysql.UpdatePlayerIsOnShip(dbid, data.is_on_ship);
            ServerSend.PlayerData(id, data);
        }
    }

    public PlayerSkillLevel FindSkill(SkillType skillType) {
        foreach (PlayerSkillLevel skill in skills) {
            if (skill.skill_id == (int)skillType) {
                return skill;
            }
        }
        return null;
    }

    public void ExperienceGained(SkillType skill_type, int experienceGained, Player player) {
        PlayerSkillLevel pSkillLevel = FindSkill(skill_type);
        pSkillLevel.experience += experienceGained;
        SkillLevel skillLevel = NetworkManager.FindSkill(skill_type, pSkillLevel.level);
        SkillLevel nextSkillLevel = NetworkManager.FindSkill(skill_type, pSkillLevel.level + 1);

        if (skillLevel.experienceEnd < pSkillLevel.experience && nextSkillLevel!=null) {
            //level up
            mysql.DeletePlayerSkillLevel(player.dbid, (int)skill_type, pSkillLevel.level);
            mysql.InsertPlayerSkillLevel(player.dbid, nextSkillLevel.skill_level_id, pSkillLevel.experience);            
        }
    }

    public PlayerSkillLevel FindSkillRequirement(int skillId, int lvl) {
        foreach (PlayerSkillLevel level in skills)
        {
            if (level.skill_id == skillId && level.level>=lvl)
                return level;
        }
        return null;
    }

    public bool HasSkillRequirement(int skillId, int lvl) {
        foreach (PlayerSkillLevel level in skills) {
            if (level.skill_id == skillId && level.level>=lvl)
                return true;
        }
        return false;
    }

    public void Die()
    {
        Debug.Log("Die");
        data.sunk = true;
        mysql.SinkShip(Server.clients[id].player.dbid);
        //gameObject.SetActive(false);        
        ServerSend.DieShip(id, data);
    }

    public void Respawn()
    {
        data.sunk = false;
        gameObject.transform.position = NetworkManager.instance.respawnPointShip.transform.position;
        mysql.RespawnShip(Server.clients[id].player.dbid);
        health = maxHealth;
        ServerSend.RespawnShip(id, data);
        ServerSend.Stats(id);
    }

    public float respawnUpdateTime;
    public float respawnTime = 10;    
}
