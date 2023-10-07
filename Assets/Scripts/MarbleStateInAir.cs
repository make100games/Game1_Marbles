using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateInAir : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter marble state InAir");
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.marble.CurrentState = this;
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        // Slam back down to the ground
        this.marble.SlamDown();
    }

    public void OnLanded()
    {
        this.animator.SetTrigger(Triggers.Landed);
    }

    public void OnBoosted(int boostForce)
    {
        this.animator.SetTrigger(Triggers.HitBoost);
    }
}
