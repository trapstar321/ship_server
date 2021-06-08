using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimController : MonoBehaviour
{

    public Animator anim;
    public string[] animation_tags = new string[] {
        "Idle", "walk", "WingAttackt", "FireAttack", "Run"};

    public string[] bools = new string[] { "Idle", "walk", "Run" };

    public GameObject fire;
    public ParticleSystem stompFX;

    private void Awake()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip c in clips)
        {
            if (c.name.Equals("rig|FireAttack"))
            {
                AnimationEvent[] events = new AnimationEvent[2];
                events[0] = new AnimationEvent();
                events[0].functionName = "EnableFire";
                events[0].time = 2.5f;

                events[1] = new AnimationEvent();
                events[1].functionName = "DisableFire";
                events[1].time = 6f;
                c.events = events;
            }

            if (c.name.Equals("rig|FlyFire"))
            {
                AnimationEvent[] events = new AnimationEvent[2];
                events[0] = new AnimationEvent();
                events[0].functionName = "EnableFire";
                events[0].time = 3f;

                events[1] = new AnimationEvent();
                events[1].functionName = "DisableFire";
                events[1].time = 6f;
                c.events = events;
            }

            if (c.name.Equals("rig|Stomp"))
            {
                AnimationEvent[] events = new AnimationEvent[1];
                events[0] = new AnimationEvent();
                events[0].functionName = "PlayStompFX";
                events[0].time = 1.5f;
                c.events = events;
            }
        }
    }

    public bool InState()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        foreach (string tag in animation_tags)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag(tag))
                return true;
        }
        return false;
    }

    public void ResetBools()
    {
        foreach(string b in bools)
        {   
            anim.SetBool(b, false);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!InState())
        {
            
        }
    }

    public void EnableFire()
    {
        fire.SetActive(true);
    }

    public void DisableFire()
    {
        fire.SetActive(false);
    }

    public void PlayStompFX()
    {
        stompFX.Play();
    }
}
