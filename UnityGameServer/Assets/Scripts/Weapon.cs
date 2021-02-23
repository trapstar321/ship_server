using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject player;
    private PlayerCharacter playerCharacter;
    public MeshCollider weaponCollider;

    private void Awake()
    {
        playerCharacter = player.GetComponent<PlayerCharacter>();
        playerCharacter.currentWeapon = gameObject.GetComponent<Weapon>();
        weaponCollider = GetComponent<MeshCollider>();
        weaponCollider.enabled = false;
    }
}
