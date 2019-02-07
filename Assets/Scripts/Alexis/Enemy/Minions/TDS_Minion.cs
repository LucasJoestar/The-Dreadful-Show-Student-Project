using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Minion : TDS_Enemy 
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
	#endregion

	#region Methods

	#region Original Methods

	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    protected override void Awake()
    {

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        
	}
	#endregion

	#endregion
}
