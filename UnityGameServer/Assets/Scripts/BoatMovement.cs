using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatMovement : MonoBehaviour
{
    public Rigidbody rb;    
    float maxSpeed;
    public float maxRotation;
    float speed;
    float rotSpeed;
    Player player;
    public bool forward;
    public bool left;
    public bool right;    

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();        
    }

    private void FixedUpdate()
    {
        if (right)        
        {
            rotSpeed = rotSpeed + 0.5f;
            if (rotSpeed <= player.rotation)
            {
                transform.Rotate(0f, rotSpeed, 0f);
            }
            else
            {
                rotSpeed = player.rotation;
                transform.Rotate(0f, rotSpeed, 0f);
            }

        }
        else
        {

        }
        if (left)        
        {
            rotSpeed = rotSpeed + 0.5f;
            if (rotSpeed <= player.rotation)
            {
                transform.Rotate(0f, -rotSpeed, 0f);
            }
            else
            {
                transform.Rotate(0f, -player.rotation, 0f);
            }
        }
        else
        {

        }

        if (forward)        
        {
            FloatForward();            
        }
    }   

    void FloatForward()
    {
        Debug.Log("FloatForward");
        speed = speed + 2f;
        if (speed <= player.speed)
        {   
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }
        else if (speed >= player.speed)
        {
            speed = player.speed;                        
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }
    }
}
