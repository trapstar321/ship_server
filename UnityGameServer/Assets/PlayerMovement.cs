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
    public NavMeshAgent agent;
    public float turnSpeed = 4f;

    NavMeshPath path;
    float elapsed;

    private void Awake()
    {
        animationController = GetComponentInChildren<CharacterAnimationController>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
        elapsed = 0.0f;
        agent.speed = walkSpeed;
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

    public float checkEvery = 1;
    float time;

    NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;

    private void LateUpdate()
    {
        if (player != null)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 0.2f)
            {
                elapsed -= 0.2f;
                bool ok = NavMesh.CalculatePath(sender.playerInstance.transform.position, player.playerInstance.transform.position, NavMesh.AllAreas, path);
                pathStatus = path.status;

                if (pathStatus == NavMeshPathStatus.PathComplete)
                {
                    agent.enabled = true;
                    agent.SetPath(path);
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

            /*if (agent.pathStatus == NavMeshPathStatus.PathComplete && !agent.hasPath)
            {
                agent.enabled = false;
                agent.enabled = true;
                agent.SetDestination(player.playerInstance.transform.position);
            }*/

            if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.hasPath)
            {
                Vector3 lookPos;
                Quaternion targetRot;

                agent.updatePosition = false;
                agent.updateRotation = false;

                lookPos = player.playerInstance.transform.position - this.transform.position;
                lookPos.y = 0;
                targetRot = Quaternion.LookRotation(lookPos);
                this.transform.rotation = targetRot;//Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
                this.transform.position = agent.nextPosition;
                //controller.Move(desVelocity.normalized * walkSpeed * Time.deltaTime);

                //agent.velocity = controller.velocity;

                if (Vector3.Distance(transform.position, player.playerInstance.transform.position) > 1)
                {
                    ServerSend.PlayerCharacterPosition(sender.id, sender.playerInstance.transform.position, 
                        sender.playerInstance.transform.rotation,
                        sender.playerCharacter.pirate.transform.rotation,
                        false);
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
        agent.enabled = true;
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
