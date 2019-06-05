using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_PlayerSpriteHolder : MonoBehaviour 
{
    /* TDS_PlayerSpriteHolder :
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
    public TDS_Player Owner { get; set; }
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    private void OnBecameInvisible()
    {
        if (!Owner) return; 
        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, true);
    }

    private void OnBecameVisible()
    {
        if (!Owner) return;
        TDS_UIManager.Instance?.DisplayHiddenPlayerPosition(Owner, false);
    }
    #endregion

    #endregion
}
