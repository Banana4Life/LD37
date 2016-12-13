using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnBehaviour : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	    var lookTo = animator.GetComponent<Player>().lookTo;
	    if (animator.GetBool("isTurningAround"))
	    {
	        if (lookTo == Vector2.up)
	        {
	            lookTo = Vector2.left;
	        }
	        else if (lookTo == Vector2.right)
	        {
	            lookTo = Vector2.up;
	        }
	        else if (lookTo == Vector2.down)
	        {
	            lookTo = Vector2.right;
	        }
	        else if (lookTo == Vector2.left)
	        {
	            lookTo = Vector2.down;
	        }
	    }
	    if (animator.GetBool("isPulling"))
	    {
	        lookTo *= -1;
	    }
	    var rotation = Quaternion.LookRotation(new Vector3(lookTo.x, 0, lookTo.y));

	    animator.GetComponent<Player>().gameObject.transform.rotation = rotation;
        animator.SetBool("isTurningAround", false);
        animator.SetBool("isTurningLeft", false);
        animator.SetBool("isTurningRight", false);
	    //Debug.Log("Turn to complete" + lookTo);
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
