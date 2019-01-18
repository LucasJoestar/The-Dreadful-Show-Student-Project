using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon;


[RequireComponent(typeof(PhotonView))]
public class TDS_NetworkManager : PunBehaviour
{
    /* NetworkManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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
    public static TDS_NetworkManager Instance;
    [SerializeField]
    PhotonView photonView;
    private bool canLeave = false;
    #endregion

    #region Methods
    #region Original Methods

    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // Use this for initialization
    void Start ()
    {
		
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}    
    #endregion
    #endregion
}
