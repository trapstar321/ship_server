using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttack : MonoBehaviour
{
    public static void OnNPCAttack(DamageColliderInfo info, NPC attacker, PlayerCharacter player) {
        if (!attacker.DisableMultipleCollision(info, player))
        {
            DoDamage(attacker, player, info);
        }
    }

    private static void DoDamage(NPC attacker, PlayerCharacter receiver, DamageColliderInfo info)
    {
        if (receiver.data.dead)
            return;

        bool crit;
        float damage;
        CalcDamage(receiver.defence, attacker, attacker.AbilityDamage(info), out crit, out damage);

        receiver.TakeDamage(damage, crit);
        attacker.DamageCollision(info, receiver);        
    }

    public static void CalcDamage(float defence, NPC attacker, float multiplier, out bool crit, out float damage)
    {
        crit = false;
        damage = 0f;
        float randValue = UnityEngine.Random.value;

        float randomPercentage = Random.Range(0.9f, 1.1f);

        if (randValue < attacker.crit_chance / 100)
        {
            crit = true;
            damage = (int)(attacker.attack * (100 / (100 + defence)) * multiplier * 2* randomPercentage);
        }
        else
        {
            damage = (int)(attacker.attack * (100 / (100 + defence)) * multiplier* randomPercentage);
        }
    }
}
