using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CharacterAnimationController : MonoBehaviour
{    
    public Animator anim;
    PlayerMovement movement;
    PlayerCharacter playerCharacter;
    public bool gathering = false;
    public bool crafting = false;
    private SpawnManager spawnManager;
    public GameObject Axe;
    public GameObject Pickaxe;
    public GameObject Frypan;
    public GameObject Pancake;
    public SkillType gatheringType;
    public SkillType craftingType;
    public CharacterController controller;

    public static bool inState = false;

    public float rollDistance = 7f;
    public float rollTime = 0.9f;
    public float DSA_Top_time = 1f;
    public float DSA_Long_time = 0.5f;

    public PlayerAbility currentAttack;
    public struct AnimationInputs
    {
        public bool w;
        public bool jump;
        public bool leftShift;
        public bool leftMouseDown;
        public float speed;
        public float horizontal;
        public string attackName;
        public string rollDirection;
    }

    public List<AnimationInputs> buffer = new List<AnimationInputs>();

    private void Awake()
    {
        playerCharacter = GetComponentInParent<PlayerCharacter>();
        movement = GetComponentInParent<PlayerMovement>();

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip c in clips)
        {
            if (c.name.Equals("PirateRig|DualSwordAttack_From_Top"))
            {
                AnimationEvent[] events = new AnimationEvent[1];
                events[0] = new AnimationEvent();
                events[0].functionName = "EnableWeapon";
                events[0].time = 0.6f;
                c.events = events;
            }
        }
    }

    bool w;
    bool leftShift;
    bool jump;
    bool leftMouseDown;
    bool rolling;
    float speed;
    float horizontal;
    string attackName = "";
    string rollDirection = "";

    public string[] animation_tags = new string[] {
        "DSA_Flip", "DSA_Top", "DSA_Long",
        "Stab", "RollLeft", "RollRight",
        "Jump", "RollForward"};

    public bool InState()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        foreach (string tag in animation_tags)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag(tag))
                return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        //TODO: ovo poništi jump, popraviti
        if (buffer.Count == 0)
        {
            w = false;
            leftShift = false;
            //jump = false;
            //leftMouseDown = false;
            speed = 0;
            horizontal = 0;
        }
        else
        {
            w = buffer[0].w;
            leftShift = buffer[0].leftShift;
            jump = buffer[0].jump;
            leftMouseDown = buffer[0].leftMouseDown;
            speed = buffer[0].speed;
            horizontal = buffer[0].horizontal;
            attackName = buffer[0].attackName;
            rollDirection = buffer[0].rollDirection;            

            buffer.RemoveAt(0);
        }

        if (playerCharacter.weaponEnabled && !inState)
        {
            DisableWeapon();
        }

        if (rolling && !inState)
        {
            controller.enabled = true;
            rolling = false;
        }

        if (w || jump)
        {
            anim.SetBool(GatherTypeToAnimation(gatheringType), false);
            anim.SetBool(CraftingTypeToAnimation(craftingType), false);
            HideTool();
        }        

        if (!InState())
        {
            if (!gathering && !crafting)
            {                
                if (attackName.Equals("DSA_Top") && HasEnergy("DSA_Top"))//Input.GetKeyDown(KeyCode.Alpha2))
                {
                    /*IEnumerator translateCoroutine = Translate(Vector3.forward, DSA_Top_time, 2f, 0f);
                    StartCoroutine(translateCoroutine);*/
                    anim.SetTrigger("DSA_Top");
                    attackName = "";
                    currentAttack = NetworkManager.playerAbilities["DSA_Top"].Clone();
                    playerCharacter.energy -= NetworkManager.playerAbilities["DSA_Top"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }
                else if (attackName.Equals("DSA_Long") && HasEnergy("DSA_Long"))//Input.GetKeyDown(KeyCode.Alpha3))
                {
                    /*IEnumerator translateCoroutine = Translate(Vector3.forward, DSA_Long_time, 0.5f, 0.5f);
                    StartCoroutine(translateCoroutine);*/
                    anim.SetTrigger("DSA_Long");
                    attackName = "";
                    currentAttack = NetworkManager.playerAbilities["DSA_Long"].Clone();
                    EnableWeapon();
                    playerCharacter.energy -= NetworkManager.playerAbilities["DSA_Long"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }
                else if (attackName.Equals("Stab") && HasEnergy("Stab"))
                {
                    anim.SetTrigger("Stab");
                    attackName = "";
                    currentAttack = NetworkManager.playerAbilities["Stab"].Clone();
                    EnableWeapon();
                    playerCharacter.energy -= NetworkManager.playerAbilities["Stab"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }
                else if (rollDirection.Equals("Left") && HasEnergy("RollLeft"))//Input.GetKeyDown(KeyCode.Alpha5))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.left, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollLeft");
                    rolling = true;
                    controller.enabled = false;
                    rollDirection = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollLeft"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }
                else if (rollDirection.Equals("Right") && HasEnergy("RollRight"))//Input.GetKeyDown(KeyCode.Alpha6))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.right, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollRight");
                    rolling = true;
                    controller.enabled = false;
                    rollDirection = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollRight"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }
                else if (rollDirection.Equals("Forward") && HasEnergy("RollForward"))//Input.GetKeyDown(KeyCode.Alpha6))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.right, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollForward");
                    rolling = true;
                    controller.enabled = false;
                    rollDirection = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollForward"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                }                
            }

            if (jump && movement.isGrounded)
            {
                anim.SetTrigger("Jump");
                anim.SetBool(GatherTypeToAnimation(gatheringType), false);
                anim.SetBool(CraftingTypeToAnimation(craftingType), false);
                HideTool();
                jump = false;
            }
        }

        /*anim.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        anim.SetFloat("horizontal", horizontal, 0.1f, Time.deltaTime);*/
    }

    public void Jump()
    {        
        anim.SetTrigger("Jump");
    }

    public string GatherTypeToAnimation(SkillType type)
    {
        switch (type)
        {
            case SkillType.MINING:
                return "Mining";
            case SkillType.WOODCUTTING:
                return "Choping";
        }
        return "";
    }

    public string CraftingTypeToAnimation(SkillType type)
    {
        switch (type)
        {
            case SkillType.COOKING:
                return "Cooking";
        }
        return "";
    }

    public void ResetAllStates()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
        anim.SetBool("Idle", true);
    }

    public void StartCrafting()
    {
        if (playerCharacter.craftingEnabled)
        {
            CraftingSpot craftingSpot = playerCharacter.craftingSpot.GetComponent<CraftingSpot>();
            crafting = !crafting;
            craftingType = craftingSpot.skillType;
            string animName = CraftingTypeToAnimation(craftingType);
            anim.SetBool(animName, crafting);
            SwitchTool(craftingType);
            return;
        }
    }

    public void StopCrafting()
    {
        crafting = false;
        HideTool();
    }

    public void SwitchTool(SkillType type)
    {
        switch (type)
        {
            case SkillType.WOODCUTTING:
                Axe.SetActive(true);
                Pickaxe.SetActive(false);
                Frypan.SetActive(false);
                break;
            case SkillType.MINING:
                Axe.SetActive(false);
                Pickaxe.SetActive(true);
                Frypan.SetActive(false);
                break;
            case SkillType.COOKING:
                Axe.SetActive(false);
                Pickaxe.SetActive(false);
                Frypan.SetActive(true);
                Pancake.SetActive(true);
                break;
        }
    }

    public void HideTool()
    {
        Axe.SetActive(false);
        Pickaxe.SetActive(false);
        Frypan.SetActive(false);
        Pancake.SetActive(false);
    }

    public void Gather()
    {
    }

    public void EnableWeapon() {        
        playerCharacter.weaponEnabled = true;
        playerCharacter.currentWeapon.weaponCollider.enabled = true;
    }

    public void DisableWeapon() {        
        playerCharacter.weaponEnabled = false;
        playerCharacter.currentWeapon.weaponCollider.enabled = false;
    }

    IEnumerator Translate(Vector3 direction, float time, float multiplier, float waitBefore)
    {
        yield return new WaitForSeconds(waitBefore);
        float start = Time.time;

        while (true)
        {
            transform.parent.Translate(direction * multiplier * Time.deltaTime);

            if (Time.time - start > time)
                yield break;
            yield return new WaitForSeconds(0.002f);
        }        
    }

    private bool HasEnergy(string abilityName) {
        return playerCharacter.energy >= NetworkManager.playerAbilities[abilityName].energy;
    }
}
