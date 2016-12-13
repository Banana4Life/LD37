using UnityEngine;

public class MoveTable : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var table = animator.gameObject.GetComponent<Table>();
        table.animationFrame += Time.deltaTime;

        var delayTime = animator.GetBool("isPush") ? table.tables.delayTime : table.tables.delayTimePull;
        var moveTime = animator.GetBool("isPush") ? table.tables.moveTime : table.tables.moveTimePull;

        if (table.moving)
        {
            Orchestrator.play_tablemove_during();

            //Debug.Log("TableMove: " + table.animationFrame);
            if (!MovementUtil.doAnimation(table.animationStart, animator.gameObject.transform,
                table.getTargetTile().transform.position.x,
                table.getTargetTile().transform.position.z, table.animationFrame - delayTime, moveTime))
            {
                Orchestrator.stop_tablemove_during();

                table.moving = false;
                animator.SetBool("isPush", false);
                animator.SetBool("isPull", false);

                if (table.tables.IsTableAt(table.pos) && table.tables.GetTable(table.pos).gameObject == animator.gameObject)
                {
                    table.tables.RemoveTableAt(table.pos);
                }
                //Debug.Log("Table move done " + table.pos + "->" + table.targetPos);
                table.pos = table.targetPos;
                table.tables.AddTable(table);

                //Objs.Get(Objs.PLAYER).GetComponent<Animator>().SetBool("isPushing", false);
                //Objs.Get(Objs.PLAYER).GetComponent<Animator>().SetBool("isPulling", false);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
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