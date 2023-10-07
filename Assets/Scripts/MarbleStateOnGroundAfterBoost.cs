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

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void OnBoosted(int boostForce)
    {
        // No op
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
