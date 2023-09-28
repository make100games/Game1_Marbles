using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateContinuousBoost : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;
    private Rigidbody rb;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter marble state ContinuousBoost");
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.rb = marble.GetComponent<Rigidbody>();
        this.marble.CurrentState = this;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Continuously apply a gentle boost
        this.marble.ApplySoftBoost();
    }

    
    public void OnBoosted(int boostForce)
    {
        // No op
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        
    }

    public void OnLanded()
    {
        // No op
    }

    public void OnStartedContinuousBoost()
    {
        // No op
    }

    public void OnStoppedContinuousBoost()
    {
        this.animator.SetTrigger(Triggers.TriggerStopContinuousBoost);
    }
}
