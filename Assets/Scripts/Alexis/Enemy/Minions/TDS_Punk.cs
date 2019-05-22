﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random;

public class TDS_Punk : TDS_Enemy 
{
    /* TDS_Punk :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Specific Behaviour of the Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[13/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Setting the Punk class as a inherited class from TDS_Enemy]
     *	    - Implementing the classes Behaviour and StartAttack
     *	    - Creating a array of enemy attack to stock all the punk attacks
     *	    - Creating a Method to get the best attack to cast for a given distance
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Punk Class]
     *	    
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods
    #endregion

    #region Overriden Methods
    /// <summary>
    /// Set its animation state to its taunt -> It will call the behaviour method
    /// </summary>
    public override void ActivateEnemy()
    {
        SetAnimationState(11);
    }
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
