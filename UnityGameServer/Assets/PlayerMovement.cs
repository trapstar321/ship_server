using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool gatheringEnabled;

    Vector3 velocity;
    public bool isGrounded;

    public struct PlayerInputs {
        public bool w;
        public bool leftShift;
        public bool jump;
        public bool leftMouseDown;
        public Vector3 move;
    }

    public List<PlayerInputs> buffer = new List<PlayerInputs>();

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        for (int i = buffer.Count - 1; i >= 0; i--)
        {
            bool w = buffer[i].w;
            bool leftShift = buffer[i].leftShift;
            bool jump = buffer[i].jump;            

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            /*float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;*/
            Vector3 move = buffer[i].move;

            if (w && !leftShift)
            {
                controller.Move(move * walkSpeed * Time.deltaTime);
            }
            else if (w && leftShift)
            {
                controller.Move(move * runSpeed * Time.deltaTime);
            }

            if (jump && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            buffer.RemoveAt(i);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Resource"))
        {
            gatheringEnabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Resource"))
        {
            gatheringEnabled = false;
        }
    }
}
