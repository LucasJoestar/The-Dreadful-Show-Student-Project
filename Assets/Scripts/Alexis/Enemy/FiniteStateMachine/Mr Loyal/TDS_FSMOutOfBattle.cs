using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_FSMOutOfBattle : StateMachineBehaviour 
{
    /* TDS_FSMOutOfBattle :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    private TDS_MrLoyal owner = null;
    private Coroutine outOfBattleCoroutine = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!owner) owner = animator.GetComponent<TDS_MrLoyal>();
        if (!owner) return;
        owner.IsInvulnerable = true; 
        owner.StopAll(); 
        outOfBattleCoroutine = owner.StartCoroutine(owner.GetOutOfBattle()); 
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex); 
        if (outOfBattleCoroutine != null)
        {
            owner.StopCoroutine(outOfBattleCoroutine);
            outOfBattleCoroutine = null;
        }
    }
}
