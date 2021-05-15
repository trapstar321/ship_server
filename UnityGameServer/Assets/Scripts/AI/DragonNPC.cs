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
    public Transform enemy;
    private DragonAnimController anim;
    public float combatRadiusMin = 3f;
    public float combatRadiusMax = 4f;  

    enum State
    {
        PATROL,
        CHILL,
        CHASE,
        RETURN,
        COMBAT,
        JUMP
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
    public Dictionary<DragonNPCAbility, AbilityDone> abilityDone = new Dictionary<DragonNPCAbility, AbilityDone>();

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
    //attack
    public float attackElapsed = 0f;
    public bool attacking = false;
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
            speed = 1.5f,
            endTime = 0.8f,
            returning = false,
            originalPosition = biteTarget.transform.localPosition,
            target = biteTarget,
            npc = this
        });

        abilityDone.Add(DragonNPCAbility.FIRE, new AbilityDone(this, DragonNPCAbility.FIRE) { endTime = fireEndTime });
        abilityDone.Add(DragonNPCAbility.STOMP, new AbilityDone(this, DragonNPCAbility.STOMP) { endTime = stompEndTime });
        abilityDone.Add(DragonNPCAbility.FLY_FIRE, new AbilityDone(this, DragonNPCAbility.FLY_FIRE) { endTime = flyFireEndTime });
        abilityDone.Add(DragonNPCAbility.TAIL_ATTACK, new AbilityDone(this, DragonNPCAbility.TAIL_ATTACK) { endTime = tailAttackEndTime });        
    }

    private void FixedUpdate()
    {
        if (attacking && ability == DragonNPCAbility.BITE)
        {
            //Bite();
            inverseKinematics[DragonNPCAbility.BITE].Update();
        }

        if (attacking)
            attackElapsed += Time.fixedDeltaTime;

        /*if(enemy)
            Debug.Log(Vector3.Distance(enemy.position, transform.position));*/
        
        //ServerSend.NPCPosition(id, transform.position, transform.rotation);
    }    

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            state = State.JUMP;
        }

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
            case State.JUMP:
                Jump();
                break;
        }

        if (onCooldown && Time.time - cooldownStart > cooldown)
            onCooldown = false;        
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
                /*enemy = GetClosestEnemy();
                if (enemy != null) {
                    SwitchState(State.CHASE);
                    coroutineRunning = false;
                    yield break;
                }*/
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
        
        if (Vector3.Distance(transform.position, enemy.position) <= chaseRange)
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
            bool ok = NavMesh.CalculatePath(transform.position, enemy.position, NavMesh.AllAreas, path);
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
            agent.destination = enemy.position;
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

    class AttackParam {
        public float speed;
        public float range;
        public float endTime;
        public float cooldown;
    }

    public class AbilityDone {
        public float endTime;
        public DragonNPC npc;
        public DragonNPCAbility ability;

        public AbilityDone(DragonNPC npc, DragonNPCAbility ability) {
            this.npc = npc;
            this.ability = ability;
        }

        public void Update() {
            if (npc.attacking && npc.ability == ability)
            {
                if (npc.attackElapsed >= endTime)
                {
                    npc.attackElapsed = 0;
                    npc.attacking = false;
                    npc.ability = DragonNPCAbility.NONE;
                    npc.StartCooldown();
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

        if (!attacking)
        {
            if (!onCooldown)
            {
                int random = Random.Range(0, 6);
                if (random == 1 && Vector3.Distance(transform.position, enemy.position) >= fireRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.FIRE;
                    attacking = true;
                    anim.anim.SetTrigger("FireAttack");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.FIRE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                //ovjde provjeriti attack range za bite
                else if (random == 2 && Vector3.Distance(transform.position, enemy.position) <= biteRange)
                {
                    animationRig.weight = 1;
                    ability = DragonNPCAbility.BITE;
                    attacking = true;
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.BITE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 3 && Vector3.Distance(transform.position, enemy.position) >= stompRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.STOMP;
                    attacking = true;
                    anim.anim.SetTrigger("Stomp");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.STOMP, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 4 && Vector3.Distance(transform.position, enemy.position) >= fireRange) {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.FLY_FIRE;
                    attacking = true;
                    anim.anim.SetTrigger("FlyFire");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.FLY_FIRE, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
                else if (random == 5 && Vector3.Distance(transform.position, enemy.position) >= tailAttackRange)
                {
                    animationRig.weight = 0;
                    ability = DragonNPCAbility.TAIL_ATTACK;
                    attacking = true;
                    anim.anim.SetTrigger("TailAttack");
                    ServerSend.NPCDoAbility(id, (int)DragonNPCAbility.TAIL_ATTACK, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
                }
            }
        }

        foreach (AbilityDone ability in abilityDone.Values) {
            ability.Update();
        }

        //face target
        if (attacking && (ability == DragonNPCAbility.FIRE
            || ability == DragonNPCAbility.BITE
            || ability == DragonNPCAbility.FLY_FIRE
            || ability == DragonNPCAbility.TAIL_ATTACK))
        {
            FaceTarget(enemy.position);
        }

        if (Vector3.Distance(enemy.position, transform.position) > combatRadiusMax) {
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
        if (!jumping)
        {
            jumping = true;
            Vector3 target;
            JumpRandomPoint(transform.position, range, out target);
            transform.LookAt(target);
            jumpTarget.transform.position = target;            
            agent.enabled = false;
            parabolaController.FollowParabola();
            ability = DragonNPCAbility.JUMP;
            anim.anim.SetTrigger("JumpStart");
        }

        if (jumping && parabolaController.Animation)
        {            
            agent.enabled = true;
        }
        else if (jumping && !parabolaController.Animation) {
            agent.enabled = true;
            jumping = false;            
            SwitchState(State.PATROL);
        }
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
        else if (state == State.JUMP) {
            anim.ResetBools();
        }

        ServerSend.NPCSwitchState(id, (int)state, (int)GameObjectType.dragon, transform.position, NetworkManager.visibilityRadius);
    }

    Transform GetClosestEnemy()
    {
        Transform tMin = null;
        float minDist = aggro_range;
        Vector3 currentPos = transform.position;

        foreach (Client client in Server.clients.Values)
        {
            if (client.player)
            {
                PlayerCharacter p = client.player.playerCharacter;                
                dist = Vector3.Distance(p.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = p.transform;
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
        cooldown = Random.Range(1, 2.5f);
        cooldownStart = Time.time;
    }
}
