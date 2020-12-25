using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float maxHp;
    public float currentHp;
    public int resourceCount;
    public int itemId;
    public SkillType skill_type;
    int experienceLeft;
    int lastExpCalc = 0;
    public float experience;

    public float respawnTime;
    public bool respawning;
    public int gatheredTime=0;
    
    float resourceDropHP;    
    float dmg;
    float totalDamage = 0;

    int nResourcesDropped = 0;
    
    public Item item;
    private GameObject resourceObject;

    public System.DateTime epochStart; //treba nam isto referentno vrijeme za server i cliente, jer Time.time je drugacije vrijeme za svakog playera
    public int currTimeInSec = 0;

    public void Awake()
    {
        resourceObject = gameObject.transform.Find("Resource").gameObject;             
        respawning = false;
        epochStart = new System.DateTime(2020, 1, 1, 0, 0, 0, System.DateTimeKind.Utc); //ovo nam daje sekunde od 01.01.2020 - 00:00:00
    }

    private void Update()
    {   
        currTimeInSec = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        if (respawning && /*Time.time -*/ currTimeInSec - gatheredTime > respawnTime) {
            Respawn();
        }
    }

    public void GatherResource(float damagePercent, out int resources, out int experience)
    {
        resources = 0;
        experience = 0;
        dmg = damagePercent / 100 * maxHp;

        if (currentHp <= 0)
            return;

        int resourcesDropped = 0;
        currentHp -= dmg;

        experienceLeft = (int)(((float)currentHp / maxHp) * this.experience);
        if (experienceLeft <= 0)
        {
            experience = (int)this.experience - lastExpCalc;
            lastExpCalc = 0;
        }
        else
        {
            experience = (int)this.experience - experienceLeft - lastExpCalc;
            lastExpCalc = (int)this.experience - experienceLeft;
        }

        if (currentHp <= 0)
        {
            resourcesDropped = resourceCount - nResourcesDropped;
            nResourcesDropped += resourcesDropped;
            resources = resourcesDropped;
            return;
        }

        totalDamage += dmg / resourceDropHP;

        int lastResourcesDropped = (int)(totalDamage);
        resourcesDropped = lastResourcesDropped - nResourcesDropped;

        nResourcesDropped += resourcesDropped;
        resources = resourcesDropped;
    }

    public bool Empty() {
        return nResourcesDropped == resourceCount;
    }

    public void Gathered() {
        
        resourceObject.SetActive(false);
        respawning = true;
        gatheredTime = currTimeInSec;// Time.time;
    }

    private void Respawn() {
        respawning = false;
        resourceObject.SetActive(true);
        nResourcesDropped = 0;
        totalDamage = 0;
        currentHp = maxHp;
    }

    public void Initialize()
    {
        resourceDropHP = maxHp / resourceCount;
        currentHp = maxHp;
    }
}
