﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class TDS_Siamese : TDS_Boss 
{
    /* TDS_Siamese :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	[Behaviour of the Siamese boss]
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
 *	Date :			[22/05/2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *	[Initialisation of the class]
 *	    - Start with Interface but then remove it
 *	    - Using inheritance with TDS_Boss
 *	    - Initialise the method SplitSiamese
 *
 *	-----------------------------------
*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] private string[] splitingEnemiesNames = new string[] { };
    [SerializeField] private Vector3[] splitingPosition = new Vector3[] { }; 
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Instantiate the splitting enemies at their splitting position
    /// CALL IN ANIMATION
    /// </summary>
    private void SplitSiamese()
    {
        if (!PhotonNetwork.isMasterClient) return;
        TDS_Enemy _e = null; 
        for (int i = 0; i < splitingEnemiesNames.Length; i++)
        {
            _e = PhotonNetwork.Instantiate(splitingEnemiesNames[i], transform.position + splitingPosition[i], Quaternion.identity, 0).GetComponent<TDS_Enemy>();
            if (Area && _e != null) Area.AddEnemy(_e); 
        }
        PhotonNetwork.Destroy(this.gameObject);
        if (Area) Area.RemoveEnemy(this);
    }

    #region Overriden Methods
    #endregion

    #endregion

    #region Unity Methods
    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < splitingPosition.Length; i++)
        {
            Gizmos.DrawLine(transform.position, transform.position + splitingPosition[i]); 
            Gizmos.DrawSphere(transform.position + splitingPosition[i], .1f); 

        }
    }
    #endregion

    #endregion
}
