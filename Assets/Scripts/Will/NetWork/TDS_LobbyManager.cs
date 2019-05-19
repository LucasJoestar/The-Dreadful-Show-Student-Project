using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class TDS_LobbyManager : PunBehaviour
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
	 *	Date :			[]
	 *	Author :		[]
	 *
	 *	Changes :
	 *
	 *	
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField]
    string connectionVersion = "1.1";
    #endregion

    #region Methods
    #region Original Methods   
    private void Init()
    {
       if(!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(connectionVersion);
        }
    }
    #endregion

    #region Photon Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("connect to master");
        //PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("connect to Lobby");
    }

    #endregion

    #region Unity Methods
    private void Start()
    {
        Init();
    }
    #endregion
    #endregion
}
