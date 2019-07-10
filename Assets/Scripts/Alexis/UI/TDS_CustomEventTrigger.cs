using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[RequireComponent(typeof(Button))]
public class TDS_CustomEventTrigger : EventTrigger 
{
	/* TDS_CustomEventTrigger :
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

	#region Fields / Properties
    public List<Action> OnSelectCallBacks = new List<Action>();
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnSelectCallBacks.ForEach(a => a?.Invoke());
    }
    #endregion

    #endregion
}
