<<<<<<< HEAD
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    GameObject[] entryButtons;
    string connectionVersion = "2.1";
    #endregion

    #region Methods
    #region Original Methods   
    public void ConnectToLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Room Name", new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("Creating N joining lobby !!");
    }
    public void GetRoomList()
    {
        RoomInfo[] _allRooms = PhotonNetwork.GetRoomList();
        Debug.Log(_allRooms.Length);


        if (_allRooms.Length > 0)
        {
            for (int i = 0; i < _allRooms.Length; i++)
            {
                RoomInfo _room = _allRooms[i];
                GameObject _entry = entryButtons[i];
                Text _entryText = _entry.GetComponentInChildren<Text>();
                _entryText.text = _room.Name;
                Debug.Log("I'm in !!");
            }
        }
    }
    void InitLobbyConnection()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(connectionVersion);
        }
    }
    public void JoinRoom(Text _entryText)
    {
        PhotonNetwork.JoinRoom(_entryText.text);
    }
    #endregion

    #region Unity Methods

    public override void OnJoinedLobby()
    {
        Debug.Log("connected to lobby");
    }

    private void Start()
    {
        InitLobbyConnection();
    }
    #endregion
    #endregion
=======
﻿using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;
using UnityEngine.UI;


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
    string connectionVersion = "2.1";
    [SerializeField]
    GameObject[] entryButtons;
    [SerializeField]
    InputField nameInputField;
    static string playerNamePref = "PlayerName";
    #endregion

    #region Methods
    #region Original Methods   
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Room Name", new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("Creating N Joining room !!");
    }

    public void GetRoomList()
    {
        RoomInfo[] _allRoom = PhotonNetwork.GetRoomList();
        Debug.Log(_allRoom.Length);
        if (_allRoom.Length > 0)
        {
            for (int i = 0; i < _allRoom.Length; i++)
            {
                RoomInfo _room = _allRoom[i];
                GameObject _entry = entryButtons[i];
                Text _entryText = _entry.GetComponentInChildren<Text>();
                _entryText.text = _room.Name;
            }
        }
    }

    private void Init()
    {
       if(!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(connectionVersion);
        }
        string _defaultName = string.Empty;
        //InputField _nameInputField = GetComponent<InputField>();
        if (nameInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePref))
            {
                _defaultName = PlayerPrefs.GetString(playerNamePref);
                nameInputField.text = _defaultName;
            }
        }
    }

    public void JoinRoom(Text _entryText)
    {
        PhotonNetwork.JoinRoom(_entryText.text);
    }

    public void SetPlayerName(string _playerName)
    {
        PhotonNetwork.playerName = _playerName + " ";
        PlayerPrefs.SetString(playerNamePref,_playerName);
    }
    #endregion

    #region Photon Methods
    public override void OnConnectedToMaster()
    {
        Debug.Log("connect to master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("connect to Lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer _newPlayer)
    {
        Debug.Log(_newPlayer.NickName + "joined the room");
        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount == 4) PhotonNetwork.LoadLevel(1);
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        Init();
    }
    #endregion
    #endregion
>>>>>>> Master2018.3
}