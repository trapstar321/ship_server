using SerializableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

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
    public Player player;
    public Player sender;
    private NavMeshAgent agent;
    private NavMeshPath path;

    public struct PlayerInputs
    {
        public bool w;
        public bool leftShift;
        public bool jump;
        public bool leftMouseDown;
        public Vector3 move;
    }

    public List<PlayerInputs> buffer = new List<PlayerInputs>();
    private CharacterAnimationController animationController;
    public float turnSpeed = 4f;

    float elapsed;

    private void Awake()
    {
        animationController = GetComponentInChildren<CharacterAnimationController>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        elapsed = 0.0f;
        path = new NavMeshPath();
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

    NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;
    private void Update()
    {

        if (player != null)
        {
            Debug.Log(player.playerInstance.transform.position);
            elapsed += Time.deltaTime;
            if (elapsed > 1.0f)
            {
                elapsed -= 1.0f;
                bool ok = NavMesh.CalculatePath(sender.playerInstance.transform.position, player.playerInstance.transform.position, NavMesh.AllAreas, path);
                pathStatus = path.status;

                if (pathStatus == NavMeshPathStatus.PathComplete)
                {
                    agent.enabled = true;
                    agent.SetDestination(player.playerInstance.transform.position);
                    ServerSend.DeactivatePlayerMovement(sender.playerCharacter.id, sender.playerInstance.transform.position);
                }
                else
                {
                    pathStatus = NavMeshPathStatus.PathInvalid;
                    ServerSend.ActivatePlayerMovement(sender.playerCharacter.id, sender.playerInstance.transform.position);
                    player = null;
                    sender = null;
                    agent.enabled = false;
                }
            }

            if (pathStatus == NavMeshPathStatus.PathComplete)
            {
                if (Vector3.Distance(transform.position, player.playerInstance.transform.position) > 1)
                {
                    ServerSend.PlayerCharacterPosition(sender.id, sender.playerInstance.transform.position, sender.playerInstance.transform.rotation, false);
                }

                if (Vector3.Distance(transform.position, player.playerInstance.transform.position) < 1)
                {
                    pathStatus = NavMeshPathStatus.PathInvalid;
                    ServerSend.SendTradeRequest(player, sender);
                    ServerSend.ActivatePlayerMovement(sender.playerCharacter.id, sender.playerInstance.transform.position);
                    player = null;
                    sender = null;
                    agent.enabled = false;
                }
            }
        }
    }

    public void SetDestination(Player sender, Player player)
    {
        this.sender = sender;
        this.player = player;
        pathStatus = NavMeshPathStatus.PathInvalid;
    }

    public void DisableAgent()
    {
        if (sender != null)
        {
            pathStatus = NavMeshPathStatus.PathInvalid;
            ServerSend.ActivatePlayerMovement(sender.playerCharacter.id, sender.playerInstance.transform.position);
            player = null;
            sender = null;
            agent.enabled = false;
        }
    }
}