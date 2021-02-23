using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 4f;
    public float runSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;    

    Vector3 velocity;
    public bool isGrounded;

    public bool jump;

    public struct PlayerInputs {
        public bool w;
        public bool leftShift;
        public bool jump;
        public bool leftMouseDown;
        public Vector3 move;
    }

    public List<PlayerInputs> buffer = new List<PlayerInputs>();
    private CharacterAnimationController animationController;

    private void Awake()
    {
        animationController = GetComponentInChildren<CharacterAnimationController>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        /*for (int i = buffer.Count - 1; i >= 0; i--)
        {
            w = buffer[i].w;
            leftShift = buffer[i].leftShift;
            jump = buffer[i].jump;
            Vector3 move = buffer[i].move;            

            /*float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            move = transform.right * x + transform.forward * z;*/

        /*if (w && !leftShift)
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
        }
        else if (w && leftShift)
        {
            controller.Move(move * runSpeed * Time.deltaTime);
        }*/

        /*if (jump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }            

        buffer.RemoveAt(i);
    }*/

        if (jump && isGrounded)
        {            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animationController.Jump();
            jump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }    
}
