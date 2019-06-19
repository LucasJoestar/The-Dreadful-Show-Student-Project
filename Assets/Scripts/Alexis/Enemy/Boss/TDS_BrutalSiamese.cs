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
    protected override Vector3 GetAttackingPosition()
    {
        Vector3 _offset = Vector3.zero;
        int _coeff = playerTarget.transform.position.x > transform.position.x ? -1 : 1;
        _offset.z = Random.Range(-agent.Radius, agent.Radius);
        _offset.x = agent.Radius / 2;
        _offset.x *= _coeff;
        return playerTarget.transform.position + _offset;
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
