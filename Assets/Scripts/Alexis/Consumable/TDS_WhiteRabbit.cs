using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(CustomNavMeshAgent))]
public class TDS_WhiteRabbit : TDS_Consumable 
{
    /* TDS_WhiteRabbit :
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
    [SerializeField, Range(1, 10)] private int healingValueMax;
    [SerializeField, Range(1,10)] private int healingValueMin;
    private int passingCountCurrent = 0;
    [SerializeField] int passingCountMax = 5;
    private float boundLeft;
    private float boundRight; 
    [SerializeField, Range(1,10)] private float speed;
    private CustomNavMeshAgent agent; 
    #endregion

    #region Methods

    #region Original Methods
    protected override void Use(TDS_Player _player)
    {
    }

    private void Run()
    {

    }
    #endregion

    #region Unity Methods
	// Use this for initialization
    private void Start()
    {
        agent = GetComponent<CustomNavMeshAgent>(); 
    }
	#endregion

	#endregion
}
