using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatMovement : MonoBehaviour
{
    public class MovementOrder {
        public PlayerInputs input;
        public int lastInputSequenceNumber;
        public Player player;
    }

    public Rigidbody rb;    
    float maxSpeed;
    public float maxRotation;
    float speed;
    float rotSpeed;
    Player player;
    public bool forward;
    public bool left;
    public bool right;

    public List<MovementOrder> buffer = new List<MovementOrder>();

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();        
    }    

    private void FixedUpdate()
    {
        if (buffer.Count == 0)
            return;        
        
        left = buffer[0].input.left;
        right = buffer[0].input.right;
        forward = buffer[0].input.forward;
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

        right = false;
        left = false;
        forward = false;

        ServerSend.PlayerPosition(buffer[0].input, buffer[0].lastInputSequenceNumber, buffer[0].player, NetworkManager.visibilityRadius);
        buffer.RemoveAt(0);
    }   

    void FloatForward()
    {
        //rb.AddForce(transform.forward * player.speed);
        rb.MovePosition(transform.position + transform.forward * player.speed * Time.fixedDeltaTime);

        /*speed = speed + 2f;
        if (speed <= player.speed)
        {   
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }
        else if (speed >= player.speed)
        {
            speed = player.speed;                        
            rb.AddForce(transform.forward * speed);// * Time.deltaTime);
        }*/
    }
}
