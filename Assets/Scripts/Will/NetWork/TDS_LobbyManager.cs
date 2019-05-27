using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using System;

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

    #region Fields / Properties

    #endregion

    #region Methods
    #region Original Methods   
    public void CreateRoom(Button _btn)
    {
        RoomId _roomId;

        _roomId = _btn.name == "FirstRoom" ? RoomId.FirstRoom :
                  _btn.name == "SecondRoom" ? RoomId.SecondRoom :
                  _btn.name == "ThirdRoom" ? RoomId.ThirdRoom :
                  _btn.name == "FourthRoom" ? RoomId.FourthRoom :
                  _btn.name == "FifthRoom" ? RoomId.FifthRoom : 
                  RoomId.WaitForIt;

        if (_roomId == RoomId.WaitForIt)
        {
            Debug.LogError("Can't connect to the room");
            return;
        }

        InitLobbyConnection(_roomId);
    }

    void InitLobbyConnection(RoomId _roomId)
    {
        int _getIndex = (int)_roomId;
        string _stringID = _getIndex.ToString();

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(_stringID);
        }
    }
    #endregion

    #region Photon Methods   

    #endregion

    #region Unity Methods
    
    #endregion
    #endregion
}