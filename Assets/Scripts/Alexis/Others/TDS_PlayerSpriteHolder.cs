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
     *	Script used on a player's sprite to send events when the sprite stop rendering. 
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
	 *	Date :			[05/06/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the PlayerSpriteHolder class]
	 *  - Implementing the variable Owner
     *  - Implementing the Methods OnBecameInvisible and OnBecameVisible
     *  
	 *	-----------------------------------
	*/

    #region Fields / Properties
    public TDS_Player Owner { get; set; }
    #endregion

    #region Methods

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
