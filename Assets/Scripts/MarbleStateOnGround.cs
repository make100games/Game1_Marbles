using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleStateOnGround : StateMachineBehaviour, MarbleState
{
    private Marble marble;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.marble = animator.gameObject.GetComponent<Marble>();
        this.animator = animator;
        this.marble.CurrentState = this;
    }

    public void OnJumpTriggered(int jumpForce, int gravityMultiplier)
    {
        var rb = marble.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * marble.JumpForce * gravityMultiplier, ForceMode.Force);
        this.animator.SetTrigger(Triggers.TriggerJumped);
    }

    public void OnLanded()
    {
        // No-op
    }
}
