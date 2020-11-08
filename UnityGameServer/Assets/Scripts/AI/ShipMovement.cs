using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipMovement : MonoBehaviour
{
    public Rigidbody rb;    
    public float maxSpeed;
    public float maxRotation;
    float speed;
    float rotSpeed;

    public bool forward;
    public bool left;
    public bool right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {

        if (right)
        {
            rotSpeed = rotSpeed + 0.5f;
            if (rotSpeed <= maxRotation)
            {
                transform.Rotate(0f, rotSpeed, 0f);
            }
            else
            {
                rotSpeed = maxRotation;
                transform.Rotate(0f, rotSpeed, 0f);
            }

        }

        if (left)
        {
            rotSpeed = rotSpeed + 0.5f;
            if (rotSpeed <= maxRotation)
            {
                transform.Rotate(0f, -rotSpeed, 0f);
            }
            else
            {
                transform.Rotate(0f, -maxRotation, 0f);
            }
        }

        if (forward)
        {
            FloatForward();
        }


        //ClientSend.Position(transform.position, transform.rotation);
    }   

    void FloatForward()
    {
        speed = speed + 2f;
        if (speed <= maxSpeed)
        {
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }
        else if (speed >= maxSpeed)
        {
            speed = maxSpeed;            
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }
    }
}
