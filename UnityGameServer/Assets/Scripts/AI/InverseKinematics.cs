using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InverseKinematics
{
    //biteSpeed
    public float speed;
    //biteEndTime
    public float endTime;
    //biteReturn
    public bool returning;
    //biteTargetOriginal
    public Vector3 originalPosition;
    //biteTarget
    public GameObject target;

    public DragonNPC npc;

    public void Update() {
        float step = speed * Time.fixedDeltaTime;
        if (npc.attackElapsed >= endTime && !returning)
        {
            npc.attackElapsed = 0;
            returning = true;
        }
        else if (npc.attackElapsed >= endTime && returning)
        {
            npc.attackElapsed = 0;
            returning = false;
            npc.usingAbility = false;
            npc.ability = DragonNPC.DragonNPCAbility.NONE;
            npc.StartCooldown();
        }
        else if (npc.attackElapsed < endTime && !returning && npc.enemy)
        {
            Vector3 targetPosition = npc.enemy.transform.position + new Vector3(0, 0.5f, 0);
            target.transform.position = Vector3.MoveTowards(target.transform.position, targetPosition, step);
        }
        else if (npc.attackElapsed < endTime && returning)
        {
            target.transform.localPosition = Vector3.MoveTowards(target.transform.localPosition, originalPosition, step);
        }
    }
}

