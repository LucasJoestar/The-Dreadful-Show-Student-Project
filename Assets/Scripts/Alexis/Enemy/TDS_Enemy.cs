using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Enemy : TDS_Character 
{
	/* TDS_Enemy :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Class to inherit from for all kind of enemies ( Minion and Boss)
     *	
     *	- Contains the common parts between Minions and bosses
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
	 *	[Creation of the enemy class]
	 *
     *  - Adding a custom NavMesh agent, 
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
	// Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake(); 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); 
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
