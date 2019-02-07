using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_RPCManager : MonoBehaviour 
{
    /* TDS_RPCManager :
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
    public static TDS_RPCManager Instance;
    public PhotonView RPCPhotonView;
    #endregion

    #region Methods
    #region Original Methods    

    #endregion

    #region Unity Methods
    void Awake()
    {      
        if (!Instance) Instance = this;        
    }
    void Start()
    {
        if (!RPCPhotonView)
            RPCPhotonView = GetComponent<PhotonView>();
    }
    #endregion
    #endregion
}
