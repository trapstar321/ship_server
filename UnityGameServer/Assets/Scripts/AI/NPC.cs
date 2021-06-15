using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SerializableObjects;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class NPC : MonoBehaviour
{
    public int id;

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
    protected RandomLoot randomLoot;
    protected Vector3 patrolPoint;
    public float rotationSpeed;

    public bool dead;
    public float respawnUpdateTime;
    public float respawnTime = 10;

    public Dictionary<int, float> playerDamage = new Dictionary<int, float>();

    public void Initialize()
    {
        level = 1;
        LoadBaseStats();

        playerEnterCollider = GetComponentsInChildren<SphereCollider>().Where(x => x.name.Equals("NPCSphere")).FirstOrDefault();
        playerEnterCollider.radius = NetworkManager.visibilityRadius / 2;

        randomLoot = FindObjectOfType<RandomLoot>();
        patrolPoint = transform.position;
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
        playerDamage.Clear();
    }

    public virtual void TakeDamage(PlayerCharacter attacker, float damage, bool crit)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }
        else {
            if (!playerDamage.ContainsKey(attacker.id))
            {
                playerDamage.Add(attacker.id, damage);
            }
            else
            {
                playerDamage[attacker.id] += damage;
            }
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
}

