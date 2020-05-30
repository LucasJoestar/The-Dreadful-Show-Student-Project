using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Fakir : TDS_Minion 
{
	/* TDS_Fakir :
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

	#region Unity Methods
    public void PlayDrawSwordSound()
    {
        AkSoundEngine.SetRTPCValue("ennemies_attack", .4f, gameObject);
        AkSoundEngine.PostEvent("FAKIR", gameObject);
    }
	#endregion

	#endregion
}
