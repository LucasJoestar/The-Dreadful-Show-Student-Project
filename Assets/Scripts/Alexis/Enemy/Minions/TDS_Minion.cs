using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public abstract class TDS_Minion : TDS_Enemy
{
    /* TDS_Minion :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Class that implement the global behaviour of a minion.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :          [22/05/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Refactoring of the class and its inheritances]
     *	    - Removing all methods that can be virtual in the TDS_EnemyClass
     *	    - Removing the Interface

     *	    
     *	-----------------------------------
     * 
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Creating the Minion class]
     *	    - Change Unity Methods into override Methods.
     *	    - Adding a bool hasEvolved.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] protected bool hasEvolved = false;

    protected int ragingCount = 0;
    [SerializeField] protected int ragingThreshold = 3;

    [SerializeField] protected float resetRageDelay = 1; 

    protected Coroutine resetRagingThreshold = null;
    #endregion

    #region Methods

    #region Original Methods

    /// <summary>
    /// Set the boolean has Evolved to true
    /// </summary>
    protected virtual void Evolve()
    {
        hasEvolved = true; 
    }

    /// <summary>
    /// Override the Die Method
    /// Remove the enemy from the Area
    /// </summary>
    protected override void Die()
    {
        base.Die();
        if (Area) Area.RemoveEnemy(this);
    }

    /// <summary>
    /// Override the ApplyDamagesBehaviour Method
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_position"></param>
    protected override void ApplyDamagesBehaviour(int _damage, Vector3 _position)
    {
        if(ragingCount > ragingThreshold)
        {
            SetAnimationState((int)EnemyAnimationState.LightHit);
            return; 
        }
        else
        {
            base.ApplyDamagesBehaviour(_damage, _position);
            ragingCount++;
            if (resetRagingThreshold != null)
            {
                StopCoroutine(resetRagingThreshold);
                resetRagingThreshold = null;
            }
            resetRagingThreshold = StartCoroutine(ResetRage());
        }
    }

    public override IEnumerator CastAttack()
    {
        yield return base.CastAttack();
        if (resetRagingThreshold != null)
        {
            StopCoroutine(resetRagingThreshold);
            resetRagingThreshold = null;
        }
        ragingCount = 0;
    }

    protected IEnumerator ResetRage()
    {
        yield return new WaitForSeconds(resetRageDelay);
        resetRagingThreshold = null; 
        ragingCount = 0; 
    }
    #endregion

    #region Overridden Methods
    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
