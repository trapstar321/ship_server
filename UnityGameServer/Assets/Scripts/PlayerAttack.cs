using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack
{
    public static void OnPlayerAttack(PlayerCharacter receiver, Collider other) {
        PlayerCharacter attacker = other.GetComponent<Weapon>().player.GetComponent<PlayerCharacter>();
        if (attacker.id != receiver.id)
        {
            CharacterAnimationController animationController = attacker.GetComponentInChildren<CharacterAnimationController>();
            if (animationController.currentAttack != null && !animationController.currentAttack.done)
            {
                Debug.Log("Attack " + animationController.currentAttack.abilityName);
                animationController.currentAttack.done = true;
                DoDamage(attacker, receiver, animationController.currentAttack);
            }
        }
    }

    public static void OnPlayerAttack(NPC receiver, Collider other)
    {
        PlayerCharacter attacker = other.GetComponent<Weapon>().player.GetComponent<PlayerCharacter>();
        if (attacker.id != receiver.id)
        {
            CharacterAnimationController animationController = attacker.GetComponentInChildren<CharacterAnimationController>();
            if (animationController.currentAttack != null && !animationController.currentAttack.done)
            {
                Debug.Log("Attack " + animationController.currentAttack.abilityName);
                animationController.currentAttack.done = true;
                DoDamage(attacker, receiver, animationController.currentAttack);
            }
        }
    }

    private static void DoDamage(PlayerCharacter attacker, PlayerCharacter receiver, SerializableObjects.PlayerAbility attack) {        
        if (receiver.data.dead)
            return;

        bool crit;
        float damage;
        CalcDamage(receiver.defence, attacker, attack, out crit, out damage);

        receiver.TakeDamage(damage, crit);        
    }

    private static void DoDamage(PlayerCharacter attacker, NPC receiver, SerializableObjects.PlayerAbility attack)
    {
        if (receiver.dead)
            return;

        bool crit;
        float damage;
        CalcDamage(receiver.defence, attacker, attack, out crit, out damage);

        receiver.TakeDamage(damage, crit);       
    }

    public static void CalcDamage(float defence, PlayerCharacter attacker, SerializableObjects.PlayerAbility ability, out bool crit, out float damage) {
        crit = false;
        damage = 0f;
        float randValue = UnityEngine.Random.value;

        float randomPercentage = Random.Range(0.9f, 1.1f);
        if (randValue < attacker.crit_chance / 100)
        {
            crit = true;
            damage = (int)(attacker.attack * (100 / (100 + defence))*ability.multiplier*2*randomPercentage);    
        }
        else
        {
            damage = (int)(attacker.attack * (100 / (100 + defence)) * ability.multiplier*randomPercentage);
        }
    }
}
