using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator anim;
    PlayerMovement charMovement;
    public bool choping = false;
    public GameObject Axe;

    public List<AnimationInputs> buffer = new List<AnimationInputs>();

    public struct AnimationInputs {
        public bool w;
        public bool jump;
        public bool leftShift;
        public bool leftMouseDown;
    }

    private void Awake()
    {
        charMovement = FindObjectOfType<PlayerMovement>();
    }

    bool w;
    bool leftShift;
    bool jump;
    bool leftMouseDown;

    void Update()
    {
        if (buffer.Count == 0) {
            w = false;
            leftShift = false;
            jump = false;
            leftMouseDown = false;
        }

        for (int i = buffer.Count - 1; i >= 0; i--)
        {
            w = buffer[i].w;
            leftShift = buffer[i].leftShift;
            jump = buffer[i].jump;
            leftMouseDown = buffer[i].leftMouseDown;

            buffer.RemoveAt(i);
        }

        if (jump && charMovement.isGrounded)
        {
            Axe.SetActive(false);
            anim.SetBool("Jump", true);
            anim.SetBool("Choping", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            choping = false;
        }
        else if (w && !leftShift)// && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            Axe.SetActive(false);
            anim.SetBool("Idle", false);
            anim.SetBool("Choping", false);
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Walk", true);
            choping = false;
        }
        else if (w && leftShift)// && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            Axe.SetActive(false);
            anim.SetBool("Run", true);
            anim.SetBool("Choping", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Jump", false);
            choping = false;
        }
        else if (!choping)
        {
            Axe.SetActive(false);
            anim.SetBool("Idle", true);
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Choping", false);
            choping = false;
        }

        if (leftMouseDown && charMovement.gatheringEnabled)
        {
            Axe.SetActive(true);
            choping = !choping;
            anim.SetBool("Idle", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Choping", true);
        }
    }
}
