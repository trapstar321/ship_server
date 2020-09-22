using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    //private Rigidbody mBody;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float throwForce = 600f;
    public float health;
    public float maxHealth = 100f;
    public int itemAmount = 0;
    public int maxItemAmount = 3;
    public float moveForce = 10f;

    private bool[] inputs;
    private float yVelocity = 0;
    private float joystickVertical = 0;
    private float joystickHorizontal = 0;

    private Vector3 newPosition;
    private Quaternion newRotation;

    private float visibilityRadius;

    public Inventory inventory;    

    void Awake() {
        //mBody = GetComponent<Rigidbody>();        
        visibilityRadius = NetworkManager.visibilityRadius;
        //Instantiate(inventory);
        inventory = FindObjectOfType<Inventory>();
    }

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];        
    }

    /// <summary>Processes player input and moves the player.</summary>
    /*public void FixedUpdate()
    {
        transform.position = newPosition;
        transform.rotation = newRotation;

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }*/

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        /*Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);*/        

        /*mBody.velocity = new Vector3(joystickHorizontal * moveForce,
                                     mBody.velocity.y,
                                     joystickVertical * moveForce);*/

        /*mBody.velocity = new Vector3(mBody.velocity.x,
                                     mBody.velocity.y,
                                     joystickVertical * moveForce);

        mBody.transform.Rotate(Vector3.up, joystickHorizontal * 90f * Time.deltaTime);*/

        /*ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);*/
    }

    public void SetPosition(Vector3 position, Quaternion rotation) {
        transform.position = position;
        transform.rotation = rotation;

        ServerSend.PlayerPosition(this, visibilityRadius);        
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void SetInput(float vertical, float horizontal)
    {
        joystickVertical = vertical;
        joystickHorizontal = horizontal;

        if (vertical != 0 || horizontal != 0) {
            Debug.Log($"Vertical={vertical}, Horizontal={horizontal}");
        }
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
            else if (_hit.collider.CompareTag("Enemy"))
            {
                _hit.collider.GetComponent<Enemy>().TakeDamage(50f);
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this, visibilityRadius);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }

    public void SearchChest() {
        RaycastHit hit;        
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 10))
        {
            string s = "";
        }
    }
}
