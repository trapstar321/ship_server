﻿using System.Collections;
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
    public bool isOnDock=false;

    public bool gatheringEnabled;
    public bool tradingEnabled;
    public bool tradeBrokerEnabled;
    public CraftingSpot craftingSpot;
    public Trader trader;

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
        else if (other.tag.Equals("Dock"))
        {
            isOnDock = true;
        }
        else if (other.tag.Equals("CraftingSpot"))
        {
            craftingSpot = other.GetComponent<CraftingSpot>();
        }
        else if (other.tag == "Trader")
        {
            tradingEnabled = true;
            trader = other.gameObject.GetComponent<Trader>();
        }
        else if (other.tag == "TradeBroker") {
            tradeBrokerEnabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Resource"))
        {
            gatheringEnabled = false;
        }
        else if (other.tag.Equals("Dock")) {
            isOnDock = false;
        }
        else if (other.tag.Equals("CraftingSpot"))
        {
            craftingSpot = null;
        }
        else if (other.tag == "Trader")
        {
            tradingEnabled = false;
            trader = null;
        }
        else if (other.tag == "TradeBroker")
        {
            tradeBrokerEnabled = false;
        }
    }
}
