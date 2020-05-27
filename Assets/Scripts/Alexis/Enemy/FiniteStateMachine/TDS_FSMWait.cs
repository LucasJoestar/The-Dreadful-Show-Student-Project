using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FSMWait : StateMachineBehaviour
{
    private TDS_Enemy owner = null;
    private Coroutine waitingCoroutine = null; 

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!PhotonNetwork.isMasterClient) return;
        if (!owner) owner = animator.GetComponent<TDS_Enemy>();
        waitingCoroutine = owner.StartCoroutine(owner.Waiting()); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(waitingCoroutine != null)
        {
            owner.StopCoroutine(waitingCoroutine);
            waitingCoroutine = null; 
        }
    }

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
}
