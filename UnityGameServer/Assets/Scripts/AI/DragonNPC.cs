using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.Animations.Rigging;

public class DragonNPC : NPC
{     
    private NavMeshAgent agent;
    private Vector3 destination;
    public float range = 10f;
    public float dist;    
    public PlayerCharacter enemy;
    private DragonAnimController anim;
    public float combatRadiusMin = 3f;
    public float combatRadiusMax = 4f;
    public Collider biteCollider;
    public Collider[] tailColliders;

    enum State
    {
        PATROL,
        CHILL,
        CHASE,
        RETURN,
        COMBAT,
        DEAD
    }

    private State state;
    private State lastState;
    private bool coroutineRunning;
    private NavMeshPath path;    
    public Rig animationRig;
    
    private ParabolaController parabolaController;
    public GameObject jumpTarget;
    public float jumpRange = 3f;
    public bool jumping = false;
    public Dictionary<DragonNPCAbility, InverseKinematics> inverseKinematics = new Dictionary<DragonNPCAbility, InverseKinematics>();
    public Dictionary<DragonNPCAbility, Ability> abilities = new Dictionary<DragonNPCAbility, Ability>();
    Dictionary<DragonNPCAbility, List<PlayerCharacter>> disableMultipleCollisition = new Dictionary<DragonNPCAbility, List<PlayerCharacter>>();

    //fire  
    public float fireRange = 4f;
    public float fireEndTime = 7f;
    public float flyFireEndTime = 6f;
    //bite
    public GameObject biteTarget;
    public float biteRange = 2.2f;
    //stomp
    public float stompRange = 2.2f;
    public float stompEndTime = 2f;
    //tail attack
    public float tailAttackRange=2.2f;
    public float tailAttackEndTime=3f;
    //jump
    public float jumpEndTime = 2f;
    //attack
    public float attackElapsed = 0f;
    public bool usingAbility = false;
    public DragonNPCAbility ability;       

    public float cooldown;
    public bool onCooldown;
    public float cooldownStart;

    public float chaseRange;

    private void Awake()
    {
        level = 1;
        Mysql mysql = FindObjectOfType<Mysql>();
        baseStats = mysql.ReadNPCBaseStatsTable(NPCType.DRAGON);
        base.Initialize();

        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);

        anim = GetComponentInChildren<DragonAnimController>();

        path = new NavMeshPath();                
        parabolaController = GetComponent<ParabolaController>();
        state = State.PATROL;
        SwitchState(state);
        
        inverseKinematics.Add(DragonNPCAbility.BITE, new InverseKinematics() { 
            speed = 3f,
            endTime = 0.8f,
            returning = false,
            originalPosition = biteTarget.transform.localPosition,
            target = biteTarget,
            npc = this
        });

        abilities.Add(DragonNPCAbility.FIRE, new Ability(this, DragonNPCAbility.FIRE) { endTime = fireEndTime, multiplier=0 });
        abilities.Add(DragonNPCAbility.STOMP, new Ability(this, DragonNPCAbility.STOMP) { endTime = stompEndTime, multiplier=0 });
        abilities.Add(DragonNPCAbility.FLY_FIRE, new Ability(this, DragonNPCAbility.FLY_FIRE) { endTime = flyFireEndTime, multiplier=0 });
        abilities.Add(DragonNPCAbility.TAIL_ATTACK, new Ability(this, DragonNPCAbility.TAIL_ATTACK) { endTime = tailAttackEndTime, multiplier=1.2f });
        abilities.Add(DragonNPCAbility.JUMP, new Ability(this, DragonNPCAbility.JUMP) { endTime = jumpEndTime, multiplier=0 });
        abilities.Add(DragonNPCAbility.BITE, new Ability(this, DragonNPCAbility.BITE) { multiplier = 1.5f, useUpdate=false });

        disableMultipleCollisition.Add(DragonNPCAbility.TAIL_ATTACK, new List<PlayerCharacter>());
    }

    private void FixedUpdate()
    {
        if (usingAbility && ability == DragonNPCAbility.BITE)
        {
            //Bite();
            inverseKinematics[DragonNPCAbility.BITE].Update();
        }

        if (usingAbility)
            attackElapsed += Time.fixedDeltaTime;

        /*if(enemy)
            Debug.Log(Vector3.Distance(enemy.position, transform.position));*/
        
        //ServerSend.NPCPosition(id, transform.position, transform.rotation);
    }    

    // Start is called before the first frame update
    new void Update()
    {
        base.Update();
        switch (state)
        {
            case State.PATROL:
                Patrol();
                break;
            case State.CHILL:
                Chill();
                break;
            case State.CHASE:
                Chase();
                break;
            case State.RETURN:
                Return();
                break;
            case State.COMBAT:
                Combat();
                break;
        }

        if (onCooldown && Time.time - cooldownStart > cooldown)
            onCooldown = false;

        foreach (Ability ability in abilities.Values)
        {
            ability.Update();
        }
    }

    float totalChilled = 0f;
    float chillTime = 0f;
    void Chill() {
        IEnumerator Chilling() {
            coroutineRunning = true;
            chillTime  = Random.Range(5, 10);
            totalChilled = 0;

            while (totalChilled <= chillTime)
            {
                enemy = GetClosestEnemy();                
                if (enemy != null) {
                    SwitchState(State.CHASE);
                    coroutineRunning = false;
                    yield break;
                }
                totalChilled += 1;
                yield return new WaitForSeconds(1f);
            }

            SwitchState(State.PATROL);
            coroutineRunning = false;
        }

        if(!coroutineRunning)
            StartCoroutine(Chilling());
    }

    void Patrol() {
        agent.speed = speed / 1.5f;
        if(destination==new Vector3(0,0,0))
            RandomPoint(patrolPoint, range, out destination);
              
        if (Vector3.Distance(transform.position, destination) <= 1)
        {
            RandomPoint(patrolPoint, range, out destination);
            SwitchState(State.CHILL);
            return;
        }

        enemy = GetClosestEnemy();
        if (enemy != null)
        {
            SwitchState(State.CHASE);
            return;
        }

        /*enemy = GetClosestEnemy();
        if (enemy != null)
        {
            bool ok = NavMesh.CalculatePath(transform.position, enemy.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                StopAgent();
                SwitchState(State.CHASE);
                return;
            }            
        }*/

        agent.destination = destination;
    }

    float elapsed = 0f;

    void Chase()
    {
        if (enemy == null)
        {
            SwitchState(State.RETURN);
            return;
        }

        ChaseTarget(chaseRange);
        
        if (Vector3.Distance(transform.position, enemy.transform.position) <= chaseRange)
        {
            StopAgent();
            SwitchState(State.COMBAT);
        }
    }

    void ChaseTarget(float stoppingDistance)
    {
        //agent.stoppingDistance = stoppingDistance;
        elapsed += Time.deltaTime;
        if (elapsed > 0.2f)
        {
            elapsed -= 0.2f;
            bool ok = NavMesh.CalculatePath(transform.position, enemy.transform.position, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                enemy = null;
                SwitchState(State.RETURN);
                return;
            }
        }

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.speed = speed / 1.2f;
            agent.destination = enemy.transform.position;
        }
    }

    void Return()
    {
        agent.stoppingDistance = 0f;
        agent.speed = speed / 1.5f;
        agent.destination = patrolPoint;

        if (Vector3.Distance(transform.position, patrolPoint) <= 1f)
        {
            SwitchState(State.PATROL);
        }
    }

    public enum DragonNPCAbility { 
        NONE,
        FIRE,
        BITE,
        JUMP,
        WING_ATTACK,
        STOMP,
        FLY_FIRE,
        TAIL_ATTACK
    }

    public class Ability {
        public float endTime;
        public DragonNPC npc;
        public DragonNPCAbility ability;
        public float multiplier;
        public bool useUpdate = true;

        public Ability(DragonNPC npc, DragonNPCAbility ability, float multiplier=0) {
            this.npc = npc;
            this.ability = ability;
            this.multiplier = multiplier;
        }

        public void Update() {
            if (!useUpdate)
                return;

            if (npc.usingAbility && npc.ability == ability)
            {
                if (npc.attackElapsed >= endTime)
                {
                    npc.attackElapsed = 0;
                    npc.usingAbility = false;
                    npc.ability = DragonNPCAbility.NONE;
                    npc.StartCooldown();
                    npc.AbilityEnd(ability);
                }
            }
        }
    }

    void Combat() {
        if (enemy == null)
        {
            SwitchState(State.RETURN);
            return;
        }

        if (enemy && enemy.data.dead) {
            SwitchState(State.RETURN);
            return;
        }
        
        if (!parabolaController.Animation)
        {
            //agent.enabled = true;
            jumping = false;                        
        }

        if (!usingAbility)
        {
            if (!onCooldown)
            {
                int random = 5;//Random.Range(0, 7);
                if (random == 1 && Vector3.Distance(transform.position, enemy.transform.position) >= fireRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.FIRE;
                    usingAbility = true;
                    anim.anim.SetTrigger("FireAttack");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.FIRE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                //ovjde provjeriti attack range za bite
                else if (random == 2 && Vector3.Distance(transform.position, enemy.transform.position) <= biteRange)
                {
                    biteCollider.enabled = true;
                    animationRig.weight = 1;
                    ability = DragonNPCAbility.BITE;
                    usingAbility = true;
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.BITE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 3 && Vector3.Distance(transform.position, enemy.transform.position) >= stompRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.STOMP;
                    usingAbility = true;
                    anim.anim.SetTrigger("Stomp");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.STOMP, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 4 && Vector3.Distance(transform.position, enemy.transform.position) >= fireRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.FLY_FIRE;
                    usingAbility = true;
                    anim.anim.SetTrigger("FlyFire");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.FLY_FIRE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 5 && Vector3.Distance(transform.position, enemy.transform.position) >= tailAttackRange)
                {
                    foreach (Collider collider in tailColliders)
                        collider.enabled = true;

                    animationRig.weight = 0;
                    ability = DragonNPCAbility.TAIL_ATTACK;
                    usingAbility = true;
                    anim.anim.SetTrigger("TailAttack");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.TAIL_ATTACK, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 6 && health < maxHealth * 0.2f) {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.JUMP;
                    usingAbility = true;
                    Jump();
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.JUMP, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
            }
        }        

        //face target
        if (usingAbility && (ability == DragonNPCAbility.FIRE
            || ability == DragonNPCAbility.BITE
            || ability == DragonNPCAbility.FLY_FIRE
            || ability == DragonNPCAbility.TAIL_ATTACK))
        {
            FaceTarget(enemy.transform.position);
        }

        if (Vector3.Distance(enemy.transform.position, transform.position) > combatRadiusMax) {
            state = State.CHASE;
            SwitchState(state);
        }

        //chase target, stop when reached combatRadiusMin
        /*if (Vector3.Distance(enemy.position, transform.position) > combatRadiusMax)
        {            
            anim.anim.SetBool("Run", true);
            ChaseTarget(combatRadiusMax);
        }
        else if (Vector3.Distance(enemy.position, transform.position) <= combatRadiusMin)
        {
            Debug.Log(Vector3.Distance(enemy.position, transform.position));
            StopAgent();
        }*/
    }    
    
    public void Jump() {
        Vector3 target;
        JumpRandomPoint(transform.position, range, out target);
        transform.LookAt(target);
        jumpTarget.transform.position = target;            
        //agent.enabled = false;
        parabolaController.FollowParabola();            
        anim.anim.SetTrigger("JumpStart");                
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                bool ok = NavMesh.CalculatePath(transform.position, randomPoint, NavMesh.AllAreas, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }        
        }
    }

    bool JumpRandomPoint(Vector3 center, float range, out Vector3 result)
    {
        while (true)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere.normalized * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                bool ok = NavMesh.CalculatePath(transform.position, randomPoint, NavMesh.AllAreas, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    return true;
                }
            }
        }
    }

    void SwitchState(State newState)
    {
        lastState = state;
        state = newState;

        if (state == State.PATROL || state == State.RETURN)
        {
            anim.ResetBools();
            anim.anim.SetBool("walk", true);
        }
        else if (state == State.CHILL)
        {
            anim.ResetBools();
            anim.anim.SetBool("Idle", true);
        }
        else if (state == State.CHASE)
        {
            chaseRange = Random.Range(1, 3) == 1 ? chaseRange = combatRadiusMin : chaseRange = combatRadiusMax;
            Debug.Log(chaseRange);
            anim.ResetBools();
            anim.anim.SetBool("Run", true);
        }
        else if (state == State.COMBAT)
        {
            anim.ResetBools();
        }

        ServerSend.NPCSwitchState(id, (int)state, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
    }

    PlayerCharacter GetClosestEnemy()
    {
        PlayerCharacter tMin = null;
        float minDist = aggro_range;
        Vector3 currentPos = transform.position;

        foreach (Client client in GameServer.clients.Values)
        {
            if (client.player && client.player.playerCharacter && !client.player.playerCharacter.data.dead)
            {
                PlayerCharacter p = client.player.playerCharacter;                
                dist = Vector3.Distance(p.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = p;
                    minDist = dist;
                    ServerSend.NPCTarget(id, p.id, (int)GameObjectType.dragon, transform.position);
                    break;
                }                
            }
        }

        return tMin;
    }

    void StopAgent() {
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void StartCooldown() {
        onCooldown = true;
        cooldown = Random.Range(0.5f, 1.5f);
        cooldownStart = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            PlayerAttack.OnPlayerAttack(this, other);
        }
    }

    public float ParticleDamage(string particleSystem) {
        if (particleSystem.Equals("DragonStompFX"))
        {
            return (int)(attack / 15);
        }
        else if (particleSystem.Equals("DragonFire")) {
            return (int)(attack / 10);
        }
        return 0;
    }

    public override float AbilityDamage(DamageColliderInfo info) {
        if (info.colliderName.Equals("Bite"))
            return abilities[DragonNPCAbility.BITE].multiplier;
        else if(info.colliderName.Equals("Tail"))
            return abilities[DragonNPCAbility.TAIL_ATTACK].multiplier;

        return 0;
    }

    public override void Die()
    {
        dead = true;
        SwitchState(State.DEAD);
        anim.anim.SetTrigger("Die");
        ServerSend.DieNPC(id);

        biteCollider.enabled = false;
        foreach (Collider collider in tailColliders)
            collider.enabled = false;
    }

    public override void Respawn()
    {
        dead = false;
        //SwitchState(State.RETURN);
        anim.anim.SetBool("Idle", true);

        IEnumerator GoToIdle() {            
            yield return new WaitForSeconds(2f);
            SwitchState(State.RETURN);            
        }

        health = maxHealth;
        ServerSend.NPCStats(id);
        ServerSend.RespawnNPC(id);
        StartCoroutine(GoToIdle());

        biteCollider.enabled = true;
        foreach (Collider collider in tailColliders)
            collider.enabled = true;
    }    

    public override bool DisableMultipleCollision(DamageColliderInfo info, PlayerCharacter receiver) {
        if (info.colliderName.Equals("Tail") && disableMultipleCollisition[DragonNPCAbility.TAIL_ATTACK].Contains(receiver)) {
            return true;
        } else if (info.colliderName.Equals("Tail") && !disableMultipleCollisition[DragonNPCAbility.TAIL_ATTACK].Contains(receiver)) {
            disableMultipleCollisition[DragonNPCAbility.TAIL_ATTACK].Add(receiver);
        }
        return false;
    }

    private void AbilityEnd(DragonNPCAbility ability) {
        if (ability == DragonNPCAbility.TAIL_ATTACK) {
            disableMultipleCollisition[DragonNPCAbility.TAIL_ATTACK].Clear();
        }
    }

    public override void DamageCollision(DamageColliderInfo info, PlayerCharacter receiver)
    {
        if (info.colliderName.Equals("Bite"))
            biteCollider.enabled = false;        
    }
}
