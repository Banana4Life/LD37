using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnRight : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var legio = animator.GetComponent<Legio>();
        var vec = animator.gameObject.transform.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        animator.gameObject.transform.eulerAngles = vec;
        if (legio.lookAt == Vector2.up)
        {
            legio.lookAt = Vector2.right;
        }
        else if (legio.lookAt == Vector2.right)
        {
            legio.lookAt = Vector2.down;
        }
        else if (legio.lookAt == Vector2.down)
        {
            legio.lookAt = Vector2.left;
        }
        else if (legio.lookAt == Vector2.left)
        {
            legio.lookAt = Vector2.up;
        }
        //Debug.Log("Turn Right complete: " + legio.lookAt);
        Legio.getMoveInfo(animator);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}