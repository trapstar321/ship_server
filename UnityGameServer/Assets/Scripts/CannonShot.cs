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
                cannonBall.SetActive(false);
            }
        }
    }

    protected void FireCannon(Transform _cannon1, Transform _cannon2)
    {
        GameObject cannonBallCopy = ObjectPooler.SharedInstance.GetPooledObject("CannonBall");
        cannonBallCopy.GetComponent<CannonBall>().player = GetComponent<Player>();
        cannonballRB = cannonBallCopy.GetComponent<Rigidbody>();
        cannonballRB.velocity = Vector3.zero;
        cannonBallCopy.transform.position = _cannon1.position;
        cannonBallCopy.transform.rotation = _cannon1.rotation;        
        cannonBallCopy.name = "CB_Player_" + player.id.ToString();
        cannonBallCopy.SetActive(true);        
        cannonballRB.AddForce(_cannon1.forward * firePower);

        GameObject cannonBallCopy2 = ObjectPooler.SharedInstance.GetPooledObject("CannonBall");
        cannonBallCopy2.GetComponent<CannonBall>().player = GetComponent<Player>();
        cannonballRB = cannonBallCopy2.GetComponent<Rigidbody>();
        cannonballRB.velocity = Vector3.zero;
        cannonBallCopy2.transform.position = _cannon2.position;
        cannonBallCopy2.transform.rotation = _cannon2.rotation;        
        cannonBallCopy2.name = "CB_Player_" + player.id.ToString();
        cannonBallCopy2.SetActive(true);        
        cannonballRB.AddForce(_cannon2.forward * firePower);
    }
}
