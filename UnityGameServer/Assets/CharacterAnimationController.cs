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

    public bool inState = false;

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
        public string currentAbility;
    }

    public List<AnimationInputs> buffer = new List<AnimationInputs>();

    private void Awake()
    {
        playerCharacter = GetComponentInParent<PlayerCharacter>();
        movement = GetComponentInParent<PlayerMovement>();

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip c in clips)
        {
            if (animation_length.ContainsKey(c.name) 
                && !c.name.Contains("PirateRig|DualSwordAttack_From_Top")
                && !c.name.Contains("PirateRig|DualSwordAttack_From_Top"))
            {
                AnimationEvent[] events = new AnimationEvent[1];
                events[0] = new AnimationEvent();
                events[0].functionName = "AnimationEnd";
                events[0].time = animation_length[c.name];
                c.events = events;
            }

            if (c.name.Equals("PirateRig|DualSwordAttack_CW_Flip"))
            {
                AnimationEvent[] events = new AnimationEvent[2];
                events[0] = new AnimationEvent();
                events[0].functionName = "EnableWeapon";
                events[0].time = 0.8f;

                events[1] = new AnimationEvent();
                events[1].functionName = "AnimationEnd";
                events[1].time = animation_length[c.name];
                c.events = events;
            }

            if (c.name.Equals("PirateRig|DualSwordAttack_From_Top"))
            {
                AnimationEvent[] events = new AnimationEvent[2];
                events[0] = new AnimationEvent();
                events[0].functionName = "EnableWeapon";
                events[0].time = 0.6f;

                events[1] = new AnimationEvent();
                events[1].functionName = "AnimationEnd";
                events[1].time = animation_length[c.name];                
                c.events = events;
            }
        }
    }

    bool w;
    bool leftShift;
    bool jump;
    bool leftMouseDown;    
    float speed;
    float horizontal;
    string currentAbility = "";    

    public Dictionary<string, float> animation_length = new Dictionary<string, float>() {
        { "PirateRig|DualSwordAttack_CW_Flip", 1},{ "PirateRig|DualSwordAttack_From_Top", 1},{ "PirateRig|TwoHandLongAttack", 1},
        { "PirateRig|RightSwordStab_01", 1},{ "PirateRig|RollLeft", 1f},{ "PirateRig|RollRight", 1f},
        { "PirateJump", 1f},{ "PirateRig|RollForward", 1f},
    };

    public string[] animation_tags = new string[] {
        "DSA_Flip", "DSA_Top", "DSA_Long",
        "Stab", "RollLeft", "RollRight",
        "Jump", "RollForward"};

    public bool InState()
    {
        return inState;
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
            currentAbility = buffer[0].currentAbility;                       

            buffer.RemoveAt(0);
        }

        if (playerCharacter.weaponEnabled && !inState)
        {
            DisableWeapon();
        }

        if (playerCharacter.rolling && !inState)
        {
            playerCharacter.rolling = false;
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
                if (currentAbility.Equals("DSA_Flip") && HasEnergy("DSA_Flip"))//Input.GetKeyDown(KeyCode.Alpha2))
                {
                    anim.SetTrigger("DSA_Flip");
                    currentAbility = "";
                    currentAttack = NetworkManager.playerAbilities["DSA_Flip"].Clone();
                    playerCharacter.energy -= NetworkManager.playerAbilities["DSA_Flip"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                if (currentAbility.Equals("DSA_Top") && HasEnergy("DSA_Top"))//Input.GetKeyDown(KeyCode.Alpha2))
                {
                    /*IEnumerator translateCoroutine = Translate(transform.forward, DSA_Top_time, 2f, 0f);
                    StartCoroutine(translateCoroutine);*/                   

                    anim.SetTrigger("DSA_Top");
                    currentAbility = "";
                    currentAttack = NetworkManager.playerAbilities["DSA_Top"].Clone();
                    playerCharacter.energy -= NetworkManager.playerAbilities["DSA_Top"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                else if (currentAbility.Equals("DSA_Long") && HasEnergy("DSA_Long"))//Input.GetKeyDown(KeyCode.Alpha3))
                {
                    /*IEnumerator translateCoroutine = Translate(Vector3.forward, DSA_Long_time, 0.5f, 0.5f);
                    StartCoroutine(translateCoroutine);*/
                    anim.SetTrigger("DSA_Long");
                    currentAbility = "";
                    currentAttack = NetworkManager.playerAbilities["DSA_Long"].Clone();
                    EnableWeapon();
                    playerCharacter.energy -= NetworkManager.playerAbilities["DSA_Long"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                else if (currentAbility.Equals("Stab") && HasEnergy("Stab"))
                {
                    anim.SetTrigger("Stab");
                    currentAbility = "";
                    currentAttack = NetworkManager.playerAbilities["Stab"].Clone();
                    EnableWeapon();
                    playerCharacter.energy -= NetworkManager.playerAbilities["Stab"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                else if (currentAbility.Equals("RollLeft") && HasEnergy("RollLeft"))//Input.GetKeyDown(KeyCode.Alpha5))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.left, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollLeft");
                    playerCharacter.rolling = true;
                    currentAbility = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollLeft"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                else if (currentAbility.Equals("RollRight") && HasEnergy("RollRight"))//Input.GetKeyDown(KeyCode.Alpha6))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.right, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollRight");
                    playerCharacter.rolling = true;
                    currentAbility = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollRight"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
                }
                else if (currentAbility.Equals("RollForward") && HasEnergy("RollForward"))//Input.GetKeyDown(KeyCode.Alpha6))
                {
                    /*IEnumerator rollCoroutine = Translate(Vector3.right, rollTime, 2f, 0f);
                    StartCoroutine(rollCoroutine);*/
                    anim.SetTrigger("RollForward");
                    playerCharacter.rolling = true;
                    currentAbility = "";
                    playerCharacter.energy -= NetworkManager.playerAbilities["RollForward"].energy;
                    playerCharacter.energyUpdateStart = Time.time;
                    inState = true;
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
        if (!inState)
        {
            anim.SetBool("Jump_", true);
            StartCoroutine(JumpEnd());
        }
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
    int i = 0;

    IEnumerator Translate(Vector3 direction, float time, float multiplier, float waitBefore)
    {
        yield return new WaitForSeconds(waitBefore);
        float start = Time.time;

        while (true)
        {
            i++;
            Debug.Log(i);
            //transform.parent.Translate(direction * multiplier * Time.deltaTime, Space.Self);
            //transform.parent.position += direction * multiplier* Time.deltaTime;
            controller.Move(direction * multiplier * Time.fixedDeltaTime);

            if (Time.time - start > time)
                yield break;
            yield return new WaitForSeconds(0.02f);
        }        
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

    public void Simulate(string currentAbility)
    {        
        Vector3 direction = Vector3.zero;
        float multiplier = 0; ;

        if (currentAbility.Equals("DSA_Top")){
            direction = transform.forward;
            multiplier = 2;
        }else if (currentAbility.Equals("DSA_Long"))
        {
            direction = transform.forward;
            multiplier = 0.5f;
        }else if (currentAbility.Equals("RollLeft"))
        {
            direction = -transform.right;
            multiplier = 2f;
        }
        else if (currentAbility.Equals("RollRight"))
        {
            direction = transform.right;
            multiplier = 2f;
        }
        else if (currentAbility.Equals("RollForward"))
        {
            direction = transform.forward;
            multiplier = 2f;
        }

        //Vector3 move = transform.forward;
        controller.Move(direction * multiplier * Time.fixedDeltaTime);           
        //controller.Move(move * multiplier/*2f / 1.5f*/ * Time.fixedDeltaTime);
    }    

    private bool HasEnergy(string abilityName) {
        return playerCharacter.energy >= NetworkManager.playerAbilities[abilityName].energy;
    }

    public void AnimationEnd()
    {
        inState = false;
    }

    public IEnumerator JumpEnd()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Jump_", false);
    }
}
