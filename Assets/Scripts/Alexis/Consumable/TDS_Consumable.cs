using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon; 

[RequireComponent(typeof(Animator))]
public abstract class TDS_Consumable : PunBehaviour 
{
    /* TDS_Consumable :
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
    [SerializeField] protected Animator animator;
    #endregion

    #region Methods

    #region Original Methods
    abstract protected void Use(TDS_Player _player); 
	#endregion

	#region Unity Methods
	#endregion

	#endregion
}
