using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMicrowave : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    var microwave = animator.gameObject.GetComponent<Microwave>();
	    microwave.animationFrame += Time.deltaTime;

	    var delayTime = animator.GetBool("isPush") ? microwave.tables.delayTime : microwave.tables.delayTimePull;
	    var moveTime = animator.GetBool("isPush") ? microwave.tables.moveTime : microwave.tables.moveTimePull;

	    if (microwave.moving)
	    {
	        Orchestrator.play_tablemove_during();
	        //Debug.Log("TableMove: " + table.animationFrame);
	        if (!MovementUtil.doAnimation(microwave.animationStart, animator.gameObject.transform,
	            microwave.getTargetTile().transform.position.x,
	            microwave.getTargetTile().transform.position.z, microwave.animationFrame - delayTime, moveTime))
	        {
	            Orchestrator.stop_tablemove_during();
	            microwave.moving = false;
	            animator.SetBool("isPush", false);
	            animator.SetBool("isPull", false);

	            microwave.pos = microwave.targetPos;
	            microwave.gameObject.transform.parent = microwave.getTargetTile().transform;

	            //Objs.Get(Objs.PLAYER).GetComponent<Animator>().SetBool("isPushing", false);
	            //Objs.Get(Objs.PLAYER).GetComponent<Animator>().SetBool("isPulling", false);
	        }
	    }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
