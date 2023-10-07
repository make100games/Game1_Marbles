using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateInAirInBoost : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter marble state InAirInBoost");
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.marble.CurrentState = this;
    }

    public void OnLanded()
    {
        Debug.Log("InAirInBoost -> Set Trigger: Landed");
        this.animator.SetTrigger(Triggers.Landed);
    }

    public void OnBoosted(int boostForce)
    {
        // No op
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        // No op
    }
}
