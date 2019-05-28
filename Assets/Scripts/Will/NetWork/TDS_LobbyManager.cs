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
    string roomName = string.Empty;
    #endregion

    #region Methods
    #region Original Methods   
    public void SelectRoom(Button _btn)
    {        
        RoomId _roomId;

        _roomId = _btn.name == "FirstRoom" ? RoomId.FirstRoom :
                  _btn.name == "SecondRoom" ? RoomId.SecondRoom :
                  _btn.name == "ThirdRoom" ? RoomId.ThirdRoom :
                  _btn.name == "FourthRoom" ? RoomId.FourthRoom :
                  _btn.name == "FifthRoom" ? RoomId.FifthRoom : 
                  RoomId.WaitForIt;

        roomName = _btn.name;

        if (_roomId == RoomId.WaitForIt)
        {            
            Debug.LogError("Can't connect to the room");
            return;
        }

        int _getIndex = (int)_roomId;
        string _stringID = _getIndex.ToString();

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(_stringID);
        }        
    }

    void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("room name : " + roomName);
    }
    #endregion

    #region Photon Methods   
    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        PhotonNetwork.JoinLobby();        
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to Master");
        CreateRoom();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("connected to Lobby");
        PhotonNetwork.JoinRoom(roomName);        
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room");
    }    
    #endregion

    #region Unity Methods

    #endregion
    #endregion
}