using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateOnGroundAfterBoost : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;
    private Rigidbody rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter marble state OnGroundAfterBoost");
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.rb = marble.GetComponent<Rigidbody>();
        this.marble.CurrentState = this;
        this.marble.StartAccelerating();
        this.marble.ApplyLandingBoost();
    }

    public void OnBoosted(int boostForce)
    {
        this.animator.SetTrigger(Triggers.HitBoost);
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        this.marble.Jump(jumpForce, gravityMultiplier);
        this.animator.SetTrigger(Triggers.Jumped);
    }

    public void OnLanded()
    {
        // No op
    }
}
