using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_AnimInSetMiniGameActive : StateMachineBehaviour
{
    /// <summary>
    /// New state of the Fire Eater mini game.
    /// </summary>
    [SerializeField] private bool isInMiniGameActive = true;

    /// <summary>
    /// Fire Eater associated with this script.
    /// </summary>
    private TDS_FireEater fireEater = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!fireEater) fireEater = animator.GetComponent<TDS_FireEater>();
        fireEater.IsInMiniGame = isInMiniGameActive;
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
}
