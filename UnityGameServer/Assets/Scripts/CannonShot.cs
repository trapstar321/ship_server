using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public GameObject CannonBall;
    Rigidbody cannonballRB;
    public Transform L_shotPos_1;
    public Transform L_shotPos_2;
    public Transform R_shotPos_1;
    public Transform R_shotPos_2;    
    public float firePower;
    public float DestroyDistance = 50;

    private Player player;
    

    private void Awake()
    {
        player = transform.gameObject.GetComponent<Player>();
    }

    public void Shoot(string position) {
        if (position.Equals("left"))
        {
            FireCannon(L_shotPos_1, L_shotPos_2);
            ServerSend.Shoot(player.id, position, player.transform.position);
        }
        else if(position.Equals("right")){
            FireCannon(R_shotPos_1, R_shotPos_2);
            ServerSend.Shoot(player.id, position, player.transform.position);
        }
    }

    private void Update()
    {
        var cannonBall = GameObject.Find("CB_Player_" + player.id);
        if (cannonBall != null)
        {
            var distance = Vector3.Distance(transform.position, cannonBall.transform.position);
            if (distance > DestroyDistance)
            {
                Destroy(cannonBall);
            }
        }
    }

    protected void FireCannon(Transform _cannon1, Transform _cannon2)
    {
        GameObject cannonBallCopy = Instantiate(CannonBall, _cannon1.position, _cannon1.rotation) as GameObject;
        cannonBallCopy.name = "CB_Player_" + player.id.ToString();
        cannonballRB = cannonBallCopy.GetComponent<Rigidbody>();
        cannonballRB.AddForce(_cannon1.forward * firePower);
        cannonBallCopy.GetComponent<CannonBall>().player = GetComponent<Player>();
        //Instantiate(explosion, shotPos.position, shotPos.rotation);

        GameObject cannonBallCopy2 = Instantiate(CannonBall, _cannon2.position, _cannon2.rotation) as GameObject;        
        cannonBallCopy2.name = "CB_Player_" + player.id.ToString();
        cannonballRB = cannonBallCopy2.GetComponent<Rigidbody>();
        cannonballRB.AddForce(_cannon2.forward * firePower);
        cannonBallCopy2.GetComponent<CannonBall>().player = GetComponent<Player>();
    }
}
