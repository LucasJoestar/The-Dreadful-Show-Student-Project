using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_PunkBoss : TDS_Boss 
{
    /* TDS_PunkBoss :
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

    #region Events

    #endregion

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Overriden Methods
    /// <summary>
    /// Set its animation state to its taunt -> It will call the behaviour method
    /// </summary>
    public override void ActivateEnemy(bool _hasToTaunt = false)
    {
        if (_hasToTaunt)
            SetAnimationState((int)EnemyAnimationState.Taunt);
        else base.ActivateEnemy(_hasToTaunt);
    }
    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
