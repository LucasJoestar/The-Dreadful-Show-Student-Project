using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_AnimOutHit : StateMachineBehaviour
{
    private TDS_Enemy owner = null;
    [SerializeField] private float recoveryTime = 1f; 
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!PhotonNetwork.isMasterClient) return;
        if (owner == null)
            owner = animator.GetComponent<TDS_Enemy>();
        owner.StopAll(); 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        if (owner == null)
            owner = animator.GetComponent<TDS_Enemy>();

        owner.SetAnimationState((int)EnemyAnimationState.Idle);
        owner.StartCoroutine(owner.ApplyRecoveryTime(recoveryTime)); 
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
