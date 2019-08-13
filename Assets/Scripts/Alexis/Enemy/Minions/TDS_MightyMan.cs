using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_MightyMan : TDS_Minion 
{
    /* TDS_MightyMan :
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
    public bool IsDashing { get; set; }
    #endregion

    #region Methods

    #region Original Methods
    protected override void ApplyDamagesBehaviour(int _damage, Vector3 _position)
    {
        base.ApplyDamagesBehaviour(_damage, _position);

        if (IsDashing)
        {
            StopAttack();
        }
    }
    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
