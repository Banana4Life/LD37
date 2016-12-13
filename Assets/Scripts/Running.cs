using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Running : StateMachineBehaviour
{
    public bool next;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isRerun", false);
        var move = animator.GetComponent<Player>();
        move.isMoving = true;
        next = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var move = animator.GetComponent<Player>();
        if (move.isMoving && !next)
        {
            move.AnimationFrame += Time.deltaTime; // time happens

            if (!MovementUtil.doAnimation(move.animationStartPosition, animator.transform, move.animationEndPosition.x,
                move.animationEndPosition.z, move.AnimationFrame, stateInfo.length))
            {
                // Debug.Log("Animation to " + move.targetPos + " done " + move.AnimationFrame + "(" + Time.deltaTime + ")/" + stateInfo.length + " to " + move.moveTo);
                move.isMoving = false;
                animator.SetBool("isPushing", false);
                animator.SetBool("isPulling", false);
                move.Transition();
                if (move.isMoving)
                {
                   next = true;
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("isRunning"))
        {
            animator.SetBool("isPushing", false);
            animator.SetBool("isPulling", false);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}