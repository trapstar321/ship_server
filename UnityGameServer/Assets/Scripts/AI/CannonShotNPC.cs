using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShotNPC : MonoBehaviour
{
    public GameObject CannonBall;    
    Rigidbody cannonballRB;
    public Transform L_shotPos_1;
    public Transform L_shotPos_2;
    public Transform R_shotPos_1;
    public Transform R_shotPos_2;
    public Transform L_Cannon_1;
    public Transform L_Cannon_2;
    public Transform R_Cannon_1;
    public Transform R_Cannon_2;

    public float DestroyDistance = 50;    
    float LastShotTime = -1;
    public float reloadSpeed;
    EnemyAI AIscript;

    [Header("Angle Calculation")]
    public float R; //Range from player
    public float A; //Angle of cannons
    public float V = 0; //12 for object with Mass=1 and AddForce=600
    float g = Mathf.Abs(Physics.gravity.y);    

    // Start is called before the first frame update
    void Start()
    {
        AIscript = gameObject.GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        var cannonBall = GameObject.Find("NPC_" + this.name);
        if (cannonBall != null)
        {
            var distance = Vector3.Distance(transform.position, cannonBall.transform.position);
            if (distance > DestroyDistance)
            {
                cannonBall.SetActive(false);
            }
        }
    }
    
    public void ShotSide(string sideToAttack)
    {
        CalculateAngle();
        if (!float.IsNaN(A))
        {
            if ((Time.time - LastShotTime) < reloadSpeed && LastShotTime != -1)
            {
                return;
            }
            else
            {
                if (sideToAttack == "LEFT")
                {
                    L_Cannon_1.transform.Rotate(new Vector3(0, 0, A));
                    L_Cannon_2.transform.Rotate(new Vector3(0, 0, A));
                    FireCannon(L_shotPos_1, L_shotPos_2);
                    ServerSend.NPCShoot(AIscript.id, "left", transform.position, new Vector3(0, 0, A));
                    L_Cannon_1.transform.Rotate(new Vector3(0, 0, -A));
                    L_Cannon_2.transform.Rotate(new Vector3(0, 0, -A));                    
                }
                else
                {
                    R_Cannon_1.transform.Rotate(new Vector3(0, 0, A));
                    R_Cannon_2.transform.Rotate(new Vector3(0, 0, A));
                    FireCannon(R_shotPos_1, R_shotPos_2);
                    ServerSend.NPCShoot(AIscript.id, "right", transform.position, new Vector3(0, 0, A));
                    R_Cannon_1.transform.Rotate(new Vector3(0, 0, -A));
                    R_Cannon_2.transform.Rotate(new Vector3(0, 0, -A));                    
                }
                LastShotTime = Time.time;
            }
        }
    }

    void CalculateAngle()
    {
        R = AIscript.dist;
        A = (Mathf.Asin((g * R) / (AIscript.cannonVelocity * AIscript.cannonVelocity)) / 2) * Mathf.Rad2Deg;
        /*if (R > AIscript.maxShootingRange/1.75f)
        {
            A = 90 - (Mathf.Asin((g * R) / (AIscript.cannonVelocity * AIscript.cannonVelocity)) / 2) * Mathf.Rad2Deg;
        }
        else
        {
            A = (Mathf.Asin((g * R) / (AIscript.cannonVelocity * AIscript.cannonVelocity)) / 2) * Mathf.Rad2Deg;
        }
        */
        V = AIscript.cannonVelocity;
    }

    void FireCannon(Transform _cannon1, Transform _cannon2)
    {
        GameObject cannonBallCopy = ObjectPooler.SharedInstance.GetPooledObject("CannonBall");
        cannonballRB = cannonBallCopy.GetComponent<Rigidbody>();
        cannonBallCopy.GetComponent<CannonBall>().npc = AIscript;
        cannonballRB.velocity = Vector3.zero;
        cannonBallCopy.transform.position = _cannon1.position;
        cannonBallCopy.transform.rotation = _cannon1.rotation;
        cannonBallCopy.name = "NPC_" + this.name;
        cannonBallCopy.SetActive(true);
        cannonballRB.AddForce(_cannon1.forward * AIscript.cannon_force);

        //Instantiate(explosion, shotPos.position, shotPos.rotation);

        GameObject cannonBallCopy2 = ObjectPooler.SharedInstance.GetPooledObject("CannonBall");
        cannonBallCopy2.GetComponent<CannonBall>().npc = AIscript;
        cannonballRB = cannonBallCopy2.GetComponent<Rigidbody>();
        cannonballRB.velocity = Vector3.zero;
        cannonBallCopy2.transform.position = _cannon2.position;
        cannonBallCopy2.transform.rotation = _cannon2.rotation;
        cannonBallCopy2.name = "NPC_" + this.name;
        cannonBallCopy2.SetActive(true);
        cannonballRB.AddForce(_cannon2.forward * AIscript.cannon_force);
    }
}
