using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonParticleDamage : ParticleDamage
{
    public override float Damage(string particleSystemName, GameObject parent)
    {
        DragonNPC npc = parent.GetComponent<DragonNPC>();
        return npc.ParticleDamage(particleSystemName);
    }
}
