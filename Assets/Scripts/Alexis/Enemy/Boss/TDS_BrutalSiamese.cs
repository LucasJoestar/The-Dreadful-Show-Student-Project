using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; 

public class TDS_BrutalSiamese : TDS_Enemy 
{
    /* TDS_BrutalSiamese :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	[Behaviour of the brutal siamese]
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
     *	Date :			[24/05/2019]
     *	Author :		[THIEBAUT Alexis]
     *
     *	Changes :
     *
     *	[Initialisation of the class]
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
        canThrow = false;
        //StartCoroutine(Behaviour());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
