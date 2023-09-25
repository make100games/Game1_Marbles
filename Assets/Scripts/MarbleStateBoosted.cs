using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateBoosted : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter marble state Boosted");
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.marble.CurrentState = this;
        animator.SetTrigger(Triggers.TriggerBoostWoreOff);
    }

    public void OnBoosted(int boostForce)
    {
        // We ignore this so that we don't boost too much in a row
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        this.marble.Jump(jumpForce, gravityMultiplier);
        this.animator.SetTrigger(Triggers.TriggerJumped);
    }

    public void OnLanded()
    {
        // No op
    }
}
