using UnityEngine;

public class TDS_AnimTornadoSiamese : StateMachineBehaviour
{
    private bool        isInitialized = false;
    private GameObject  gameObject =    null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isInitialized)
        {
            isInitialized = true;
            gameObject = animator.gameObject;
        }

        // Play sound on GameObject
        AkSoundEngine.SetRTPCValue("ennemies_attack", .4f, gameObject);
        AkSoundEngine.PostEvent("SIAMESE", gameObject);

        Debug.LogError("Start Tornado");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop playing sound on GameObject
        AkSoundEngine.PostEvent("Stop_SIAMESE_TONRADO", gameObject);

        Debug.LogError("Stop Tornado");
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
