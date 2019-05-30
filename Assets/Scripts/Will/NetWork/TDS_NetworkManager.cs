﻿using System.Linq; 
using UnityEngine;
using UnityEngine.UI;
using Photon;
using TMPro;

#pragma warning disable 0414
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
    #region Lobby
    bool canLeave = false;
    [Space]
    [SerializeField, Range(1, 4)]
    int minimumPlayerToLaunch = 1;
    string roomName = string.Empty;

    [Space]
    [SerializeField]
    new PhotonView photonView;

    /*
    [Space]
    [SerializeField]
    TMP_Text textPlayerCounter;
    */

    #endregion
    public static TDS_NetworkManager Instance;

    #region Player     
    bool isHost = false;
    public bool IsHost { get { return isHost; } }
    private string playerNamePrefKey = "Local Player"; 
    public string PlayerNamePrefKey
    {
        get { return playerNamePrefKey; }
        set
        {
            playerNamePrefKey = value;
            PhotonNetwork.playerName = value; 
            PlayerPrefs.SetString(PlayerNamePrefKey, value);
        }
    }
    #endregion

    #endregion

    #region Methods
    public void DemoTest(string _iD)
    {
        PhotonNetwork.ConnectUsingSettings(_iD);
    }

    #region Original Methods


    #region Lobby Methods   
    void CreateRoom()
    {
        if (roomName == string.Empty) roomName = "RoomTest"; 
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("room name : " + roomName);
    }

    void InitMulti()
    {
        #region Player
        if (PlayerPrefs.HasKey(PlayerNamePrefKey))
        {
            PlayerNamePrefKey = PlayerPrefs.GetString(PlayerNamePrefKey);
            TDS_MainMenu.Instance.PlayerNameField.text = playerNamePrefKey; 
        }
        #endregion
    }

    public void LeaveRoom()
    {
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    void PlayerCount()
    {
        bool _canLaunch = PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch && PhotonNetwork.isMasterClient ? true : false;
        TDS_MainMenu.Instance?.UpdatePlayerCount(PhotonNetwork.room.PlayerCount, _canLaunch, PhotonNetwork.playerList); 
    }

    public void SelectRoom(Button _btn)
    {
        RoomId _roomId;

        _roomId = _btn.name == "FirstRoomButton" ? RoomId.FirstRoom :
                  _btn.name == "SecondRoomButton" ? RoomId.SecondRoom :
                  _btn.name == "ThirdRoomButton" ? RoomId.ThirdRoom :
                  _btn.name == "FourthRoomButton" ? RoomId.FourthRoom :
                  _btn.name == "FifthRoomButton" ? RoomId.FifthRoom :
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
    #endregion

    #region Player   
    public void SetPlayerName(string _nickname)
    {
        PhotonNetwork.playerName = _nickname + " ";
        PlayerPrefs.SetString(PlayerNamePrefKey,_nickname);
    }
    #endregion

    #endregion

    #region PhotonMethods
    /// <summary>
    /// When the player is connected to master, he joins the room
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to Master");
        CreateRoom();
    }
    /// <summary>
    /// When the player create a room, he's the host of the game
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        PhotonNetwork.JoinLobby();
        isHost = true;
    }
    public override void OnDisconnectedFromPhoton()
    {
        InitMulti();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");
        //leaveButton.SetActive(true);
        PlayerCount();
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        PlayerCount();
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PlayerCount();
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
        GUILayout.Box(TDS_GameManager.LocalPlayer.ToString()); 
    }
    void Start ()
    {
        if(!photonView) photonView = GetComponent<PhotonView>();
        InitMulti();
    }
    #endregion
    #endregion
}