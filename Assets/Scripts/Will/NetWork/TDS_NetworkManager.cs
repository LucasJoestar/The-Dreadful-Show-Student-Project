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
    #region Conection settings
    
    #endregion
    #region Photon
    [SerializeField, Header("Photon settings")]
    new PhotonView  photonView ;
    [SerializeField]
    string connectionVersion = "1.1";
    bool canLeave = false;
    #endregion
    #region Player     
    //Player type in local
    PlayerType localPlayer = PlayerType.BeardLady;
    //Player name in local
    //[SerializeField]
    //string playerName = "Player";
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Connect the player to Photon
    /// </summary>
    void InitConnection()
    {
        PhotonNetwork.ConnectUsingSettings(connectionVersion);
    }
    void JoinRoom()
    {
        RoomOptions _options = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 4,
        };
        PhotonNetwork.JoinOrCreateRoom("TDS_EPIIC",_options,null);
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!Instance) Instance = this;
    }
    private void OnGUI()
    {
        GUILayout.Box(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Box(PhotonNetwork.isMasterClient.ToString());
    }
    void Start ()
    {
        InitConnection();
        photonView = GetComponent<PhotonView>();
    }	
	void Update ()
    {
        
	}    
    #endregion
    #endregion
}