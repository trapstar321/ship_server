using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SerializableObjects;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class NPC:MonoBehaviour
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

    public void Initialize()
    {
        level = 1;
        LoadBaseStats();

        playerEnterCollider = GetComponentInChildren<SphereCollider>();
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed*Time.deltaTime);
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
}

