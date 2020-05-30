using UnityEngine;

public class TDS_AnimInMightyManRoar : StateMachineBehaviour
{
    private bool            isInitialized = false;
    private TDS_MightyMan   mightyMan =     null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isInitialized)
        {
            isInitialized = true;
            mightyMan = animator.GetComponent<TDS_MightyMan>();
        }

        // Play sound on GameObject
        if (mightyMan.PlayFirstTaunt)
        {
            AkSoundEngine.PostEvent("Play_mightyman_gros", mightyMan.gameObject);
        }
        else
            mightyMan.PlayFirstTaunt = true;
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
