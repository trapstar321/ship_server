﻿using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class ShipNPC : NPC
{    
    enum State
    {
        SAIL,
        CHASE,
        ATTACKING,
        POSITIONING
    }

    enum Side
    {
        LEFT,
        RIGHT,
        STRAIGHT
    }

    private State state;
    private State lastState;
    private bool arrivedAtDestination = false;
    bool rotationStarted = false;
    private ShipMovement shipMovement;
    public GameObject currentDestination;
    public GameObject destination1;
    public GameObject destination2;    
    public float minShootingRange = 20f;
    public float maxShootingRange = 30f;
    public GameObject L_cannon1;
    public GameObject L_cannon2;
    public GameObject R_cannon1;
    public GameObject R_cannon2;
    private Transform enemy;
    private GameObject cannon1;
    private GameObject cannon2;
    NavMeshAgent agent;
    bool coroutineRunning = false;
    bool adjustingComplete = false;
    public float dist;
    public float cannonVelocity;
    public static ServerSend send;    

    

    Dictionary<int, float> PlayerDamage = new Dictionary<int, float>();    

    public void Awake() {        
        Mysql mysql = FindObjectOfType<Mysql>();
        baseStats = mysql.ReadNPCBaseStatsTable(NPCType.SHIP);
        base.Initialize(NPCType.SHIP);
    }    

    private void Start()
    {
        destination1 = GameObject.Find("AIDestination1");
        destination2 = GameObject.Find("AIDestination2");
        send = FindObjectOfType<ServerSend>();

        agent = GetComponent<NavMeshAgent>();
        SwitchState(State.SAIL);
        shipMovement = GetComponent<ShipMovement>();
        shipMovement.maxSpeed = speed;
        shipMovement.maxRotation = rotation;
        currentDestination = destination1;
        agent.SetDestination(currentDestination.transform.position);
        cannonVelocity = Time.fixedDeltaTime * cannon_force;        

        maxShootingRange = (2 * cannonVelocity * cannonVelocity * Mathf.Sin(45 * Mathf.Deg2Rad) * Mathf.Cos(45 * Mathf.Deg2Rad)) / Mathf.Abs(Physics.gravity.y);
    }

    private void FixedUpdate()
    {
        send.NPCPosition(this, NetworkManager.visibilityRadius);   
    }

    void EnemyLost() {
        if ((state == State.CHASE || state == State.POSITIONING) && enemy == null)
        {
            StopAgent();
            arrivedAtDestination = false;
            StopAllCoroutines();
            agent.SetDestination(currentDestination.transform.position);
            SwitchState(State.SAIL);
        }
    }

    void Update()
    {
        EnemyLost();

        switch (state)
        {
            case State.SAIL:
                Sail();
                break;
            case State.CHASE:
                Chase();
                break;
            case State.POSITIONING:
                Positioning();
                break;
            default:
                break;
        }
    }

    public void SetHealth(float health) {
        this.health = health;
        if (health <= 0 && !dead) {
            Die();            
        }
    }

    public override void Die() {
        base.Die();
        dead = true;        
    }

    private float TakeDamage(Player player)
    {
        bool crit = false;
        float damage = 0f;
        float randValue = UnityEngine.Random.value;
        if (randValue < player.crit_chance / 100)
        {
            crit = true;
            damage = player.attack * 2 - defence;
            health -= damage;
        }

        else
        {
            damage = player.attack - defence;
            health -= damage;
        }
        SetHealth(health);
        ServerSend.TakeDamage(id, transform.position, damage, "npc", crit);
        return damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("CannonBall"))
        {
            Player player = other.gameObject.GetComponent<CannonBall>().player;
            
            Vector3 tempPos = other.transform.position - new Vector3(0f, 0.5f, 0f);

            float damage = TakeDamage(player);

            if (!PlayerDamage.ContainsKey(player.id))
                PlayerDamage.Add(player.id, 0);

            PlayerDamage[player.id]+=damage;

            Debug.Log("Hit by " + other.name);
            other.gameObject.SetActive(false);
        }
    }

        void Sail()
        {
        /*if (arrivedAtDestination && !IsLookingAtObject(transform, currentDestination.transform, Vector3.forward))
        {
            FaceTarget(currentDestination.transform);
            return;
        }
        else if (arrivedAtDestination && IsLookingAtObject(transform, currentDestination.transform, Vector3.forward))
        {
            arrivedAtDestination = false;
            agent.SetDestination(currentDestination.transform.position);
        }*/
        if (arrivedAtDestination) {
            arrivedAtDestination = false;
            agent.SetDestination(currentDestination.transform.position);
        }

        if (Vector3.Distance(transform.position, currentDestination.transform.position) <= 10f)
        {
            if (currentDestination == destination1)
                currentDestination = destination2;
            else
                currentDestination = destination1;

            StopAgent();
            arrivedAtDestination = true;
        }

        Transform enemy = GetClosestEnemy();
        if (enemy != null)
        {
            Vector3 enemyPos = enemy.transform.position;
            arrivedAtDestination = false;
            this.enemy = enemy;
            if (Vector3.Distance(transform.position, enemyPos) > minShootingRange)
            {
                SwitchState(State.CHASE);
            }
            //SwitchState(State.ADJUST_FIRE);
        }
    }

    void Chase()
    {
        Vector3 enemyPos;
        if (enemy == null)
        {
            EnemyLost();
            return;
        }
        else {
            enemyPos = enemy.transform.position;
        }

        StopAgent();        
        agent.SetDestination(enemyPos);

        if (Vector3.Distance(transform.position, enemyPos) <= Random.Range(minShootingRange, maxShootingRange))
        {
            StopAgent();
            SwitchState(State.POSITIONING);
        }
    }

    void Positioning()
    {
        var enemyMoving = isEnemyTargetMoving();
        bool CannonPointing;
        string SideToAttack;
        IsCannonPointingToTarget(transform, enemy, out CannonPointing, out SideToAttack); 
        
        if (CannonPointing && SideToAttack != null)
        {
            dist = Vector3.Distance(transform.position, enemy.position);
            CannonShotNPC instance = gameObject.GetComponent<CannonShotNPC>();
            instance.ShotSide(SideToAttack);
        }


        var AdjustOrMove = Random.Range(1, 4);

        if (adjustingComplete && AdjustOrMove == 1)
            AdjustOrMove = 2;

        if (!coroutineRunning)
        {
            if (AdjustOrMove == 1)
            {
                StartCoroutine(AdjustingCannon());
            }
            else if (AdjustOrMove == 2)
            {
                var xc = RandomPointInAnnulus(new Vector2(enemy.transform.position.x, enemy.transform.position.z), minShootingRange, maxShootingRange);
                StartCoroutine(Move(xc));
            }
            else
            {
                StartCoroutine(RotateAround());
            }
        }

        IEnumerator Move(Vector3 rndPos)
        {            
            coroutineRunning = true;
            Vector3 targetDir = rndPos - transform.position;
            bool stillRotating = false;
            do {
                var oldEulerAngles = transform.rotation.eulerAngles.y;
                Quaternion rotDir = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotDir, Time.deltaTime * 20f);

                if (oldEulerAngles == transform.rotation.eulerAngles.y)
                    stillRotating = true;

                Debug.DrawLine(transform.position, rndPos);
                yield return null;
            } while (!IsLookingAtObject(transform.forward, targetDir) && !stillRotating);

            do {
                shipMovement.forward = true;
                Debug.DrawLine(transform.position, rndPos);
                yield return null;
            } while (isShipInsideSphere(transform.position, rndPos) && IsLookingAtObject(transform.forward, targetDir));

            shipMovement.forward = false;
            adjustingComplete = false;
            coroutineRunning = false;
        }

        IEnumerator RotateAround()
        {   
            coroutineRunning = true;
            Side side = LeftOrRight(enemy.gameObject);
            var countDown = 10f;
            if (side == Side.RIGHT)
            {
                for (int i = 0; i < 100; i++)
                {
                    do
                    {
                        Debug.Log(i++);
                        countDown -= Time.smoothDeltaTime;
                        transform.RotateAround(enemy.position, Vector3.up, 5 * Time.deltaTime);
                        Vector3 targetDir = enemy.position - transform.position;
                        Quaternion rotDir = Quaternion.LookRotation(targetDir);
                        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, rotDir.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
                        transform.rotation = Quaternion.Slerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * 1f);
                        yield return null;
                    } while (countDown >= 0);
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    do
                    {
                        //Debug.Log(i++);
                        countDown -= Time.smoothDeltaTime;
                        transform.RotateAround(enemy.position, Vector3.up, -5 * Time.deltaTime);
                        Vector3 targetDir = enemy.position - transform.position;
                        Quaternion rotDir = Quaternion.LookRotation(targetDir);
                        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, rotDir.eulerAngles.y + 90, transform.rotation.eulerAngles.z);
                        transform.rotation = Quaternion.Slerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * 1f);
                        yield return null;
                    } while (countDown >= 0);
                }
            }
            
            

            coroutineRunning = false;
            
        }

        IEnumerator AdjustingCannon()
        {
            coroutineRunning = true;
            var sideFacing = 0;
            Side side;// = LeftOrRight(enemy.gameObject);

            if (Random.Range(1,3) == 1)
            {
                sideFacing = 90;
                side = Side.LEFT;
                cannon1 = L_cannon1;
            }
            else
            {
                sideFacing = -90;
                side = Side.RIGHT;
                cannon1 = R_cannon1;
            }            
            if (side == Side.RIGHT)
            {
                do
                {
                    Vector3 targetDir = enemy.position - transform.position;
                    Quaternion rotDir = Quaternion.LookRotation(targetDir);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, rotDir.eulerAngles.y + sideFacing, transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * 1f);
                    yield return null;
                } while (!IsLookingAtObject(cannon1.transform, enemy, transform.right));
            }
            else
            {
                do
                {
                    Vector3 targetDir = enemy.position - transform.position;
                    Quaternion rotDir = Quaternion.LookRotation(targetDir);
                    Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, rotDir.eulerAngles.y + sideFacing, transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, LookAtRotationOnly_Y, Time.deltaTime * 1f);
                    yield return null;
                } while (!IsLookingAtObject(cannon1.transform, enemy, -transform.right));
            }

            adjustingComplete = true;
            coroutineRunning = false;
            //StartCoroutine(Attacking());                      
        }

        if (Vector3.Distance(transform.position, enemy.transform.position) > maxShootingRange)
        {
            StopAllCoroutines();
            coroutineRunning = false;
            SwitchState(State.CHASE);
        }

        Vector3 xyz = (enemy.transform.position - transform.position).normalized;        
        Vector3 newVec1 = (Quaternion.AngleAxis(45, transform.up) * xyz) * maxShootingRange;        
        Vector3 newVec2 = (Quaternion.AngleAxis(-45, transform.up) * xyz) * maxShootingRange;
        /*
        Debug.DrawRay(transform.position, xyz * 50, Color.green, 1);
        Debug.DrawRay(enemy.position, newVec1, Color.black, 1);
        Debug.DrawRay(enemy.position, newVec2, Color.black, 1);
        */
    }
    
    public Vector3 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
    {
        Vector2 point;
        float angleArea;
        Vector3 V1;
        Vector3 V2;
        Vector3 newPosDirection;
        Vector3 newPos;

        Vector3 vec;
        Vector3 distance;
        float v;
        do
        {
            var randomDirection = (Random.insideUnitCircle * origin).normalized;
            var randomDistance = Random.Range(minRadius, maxRadius);
            point = origin + randomDirection * randomDistance;
            newPos = new Vector3(point.x, 0f, point.y);
            
            newPosDirection = newPos - enemy.position;
            
            distance = enemy.transform.position - transform.position;

            v = Vector3.SignedAngle(distance, newPosDirection, Vector3.up);

        }
        while ((Vector3.SignedAngle(distance, newPosDirection, Vector3.up) < 45 && Vector3.SignedAngle(distance, newPosDirection, Vector3.up) > -45) 
                || (Vector3.SignedAngle(distance, newPosDirection, Vector3.up) > 135 || Vector3.SignedAngle(distance, newPosDirection, Vector3.up) < -135));// (Vector3.Angle(V1, newPosDirection) < angleArea && Vector3.Angle(V2, newPosDirection) < angleArea));

        v = Vector3.SignedAngle(distance, newPosDirection, Vector3.up);
        //Debug.DrawRay(new Vector3(0, 0, 0), newPosDirection * 50, Color.magenta, 5);
        //Debug.DrawRay(new Vector3(0,0,0), distance * 10, Color.cyan, 5);
        return newPos;
    }

    Side LeftOrRight(GameObject go)
    {
        var relativePoint = transform.InverseTransformPoint(go.transform.position);
        if (relativePoint.x < 0.0)
            return Side.LEFT;
        else if (relativePoint.x > 0.0)
            return Side.RIGHT;
        else
            return Side.STRAIGHT;
    }

    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 20f);
        //Debug.DrawRay(transform.position, direction * 50, Color.green, 1);
    }

    void StopAgent()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    bool IsLookingAtObject(Transform transform, Transform target, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == target.gameObject)
                return true;
        }
        //Debug.DrawRay(transform.position, direction*50, Color.magenta, 1);
        return false;
    }

    bool IsLookingAtObject(Vector3 from, Vector3 to)
    {        
        float angle = Vector3.Angle(from, to);

        if (angle < 2f || angle > 358f)
            return true;

        return false;
    }

    void IsCannonPointingToTarget(Transform transform, Transform target, out bool pointing, out string side)
    {
        Side checkSide = LeftOrRight(enemy.gameObject);
        side = checkSide.ToString();
        Vector3 direction;
        pointing = false;
        if (checkSide == Side.LEFT)
        {
            direction = -transform.right;
        }
        else
        {
            direction = transform.right;
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject == target.gameObject)
                 pointing = true;
        }
        Debug.DrawRay(transform.position, direction * 50, Color.cyan, 1);
        
    }

    bool isEnemyTargetMoving()
    {
        var threshold = .02f; // anything above this value will return true

        if (enemy.GetComponent<Rigidbody>().velocity.sqrMagnitude >= threshold)
        {
            return true; // enemy is moving
        }
        else
        {
            return false;
        }
    }

    bool isShipInsideSphere(Vector3 pos, Vector3 center)
    {
        float radius = 5;
        float x = Vector3.Distance(pos, center);
        if (x < radius)
            return false;
        
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggro_range);
        Gizmos.color = Color.green;
        if (enemy != null)
            Gizmos.DrawWireSphere(enemy.position, maxShootingRange);

    }

    Transform GetClosestEnemy()
    {
        Transform tMin = null;
        float minDist = aggro_range;
        Vector3 currentPos = transform.position;

        foreach (Client client in GameServer.clients.Values)
        {
            Player p = client.player;
            if (p != null)
            {
                dist = Vector3.Distance(p.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = p.transform;
                    minDist = dist;
                    break;
                }
            }
        }
        

        return tMin;
    }

    void SwitchState(State newState)
    {
        lastState = state;
        state = newState;
    }    
}
