using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SerializableObjects;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Collections;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public int id;
    public NPCType npc_type;

    public int level;
    public float attack;
    public float health;
    public float defence;
    public float rotation;
    public float speed;
    public float visibility;
    public float cannon_reload_speed;
    public float crit_chance;
    public float cannon_force;
    public float maxHealth = 100f;

    public float aggro_range;

    protected List<NPCBaseStat> baseStats;

    protected SphereCollider playerEnterCollider;    
    protected Vector3 patrolPoint;
    public float rotationSpeed;

    public bool dead;
    public float respawnUpdateTime;
    public float respawnTime = 10;

    public Dictionary<int, float> playerDamage = new Dictionary<int, float>();

    public int max_loot_count = 0;

    public float leaveCombatMaxRange = 20f;

    public NavMeshPath path;

    public void Initialize(NPCType type)
    {
        level = 1;
        path = new NavMeshPath();
        Mysql mysql = FindObjectOfType<Mysql>();
        baseStats = mysql.ReadNPCBaseStatsTable(type);
        max_loot_count = mysql.GetNPCMaxLootCount(type);

        LoadBaseStats();        

        playerEnterCollider = GetComponentsInChildren<SphereCollider>().Where(x => x.name.Equals("NPCSphere")).FirstOrDefault();
        playerEnterCollider.radius = NetworkManager.visibilityRadius / 2;        
        patrolPoint = transform.position;

        StartCoroutine(RemovePlayerDamage());
    }

    protected void LoadBaseStats()
    {
        foreach (NPCBaseStat stat in baseStats)
        {
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

    protected void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    public bool InState(Animator anim, string[] animation_tags)
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        foreach (string tag in animation_tags)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag(tag))
                return true;
        }
        return false;
    }

    public virtual void Die()
    {
        foreach (KeyValuePair<int, float> val in playerDamage) {
            float percentage = val.Value / maxHealth * 100;

            RandomLoot randomLoot;
            if (GameServer.clients[val.Key].player.group == null)
            {
                randomLoot = new RandomLoot((int)npc_type, percentage, max_loot_count);
                randomLoot.GenerateLoot();                
            }
            else {
                float damage = GameServer.FindGroupDamage(this, GameServer.clients[val.Key].player.group);
                randomLoot = new RandomLoot((int)npc_type, percentage, max_loot_count);
                randomLoot.GenerateLoot();
            }

            StartCoroutine(GameServer.DespawnLoot(val.Key, randomLoot, NetworkManager.lootDespawnTime));            

            if (!GameServer.playerLoot.ContainsKey(val.Key))
                GameServer.playerLoot.Add(val.Key, new List<RandomLoot>());
            GameServer.playerLoot[val.Key].Add(randomLoot);

            randomLoot.position = transform.position;
            ServerSend.OnLootDropped(val.Key, randomLoot.id, transform.position);
        }
        playerDamage.Clear();
    }

    public virtual void TakeDamage(PlayerCharacter attacker, float damage, bool crit)
    {
        health -= damage;        

        if (!playerDamage.ContainsKey(attacker.id))
        {
            playerDamage.Add(attacker.id, damage);
        }
        else
        {
            playerDamage[attacker.id] += damage;
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }

        ServerSend.TakeDamage(id, transform.position, damage, "npc", crit);
    }

    public void Update()
    {
        if (dead)
        {
            if (Time.time - respawnUpdateTime < respawnTime)
                return;

            respawnUpdateTime = Time.time;
            Respawn();
        }
        else
        {
            respawnUpdateTime = Time.time;
        }
    }

    public virtual void Respawn()
    {
       
    }

    public virtual float AbilityDamage(DamageColliderInfo info)
    {
        return 0;
    }

    public virtual void DamageCollision(DamageColliderInfo info, PlayerCharacter receiver)
    {

    }

    public virtual bool DisableMultipleCollision(DamageColliderInfo info, PlayerCharacter receiver)
    {
        return false;
    }

    public virtual SerializableObjects.NPCStartParams GetStartParams() {
        return null;
    }

    public float PlayerDamage(int playerId) {
        return playerDamage[playerId];
    }    

    IEnumerator RemovePlayerDamage() {
        List<int> toRemove = new List<int>();

        while (true)
        {
            foreach (KeyValuePair<int, float> val in playerDamage)
            {
                PlayerCharacter player = GameServer.clients[val.Key].player.playerCharacter;

                if (player.data.dead || Vector3.Distance(patrolPoint, player.transform.position) >= leaveCombatMaxRange)
                {
                    toRemove.Add(val.Key);
                }

                bool ok = NavMesh.CalculatePath(transform.position, player.transform.position, NavMesh.AllAreas, path);
                if (path.status != NavMeshPathStatus.PathComplete)
                {
                    toRemove.Add(val.Key);
                }
            }            

            foreach (int key in toRemove)
                playerDamage.Remove(key);

            toRemove.Clear();
            yield return new WaitForSeconds(1);
        }
    }
}

