using UnityEngine;
using UnityEngine.UI;
using Photon;

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
       
    #region Fields / Properties
    #region Lobby
    [Space]
    [SerializeField, Range(1, 4)]
    int minimumPlayerToLaunch = 1;
    string roomName = string.Empty;


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
            if(value != string.Empty)
            {
                playerNamePrefKey = value;

                PhotonNetwork.playerName = value;
                PhotonNetwork.player.NickName = value;
                AuthenticationValues _authenticationValues = new AuthenticationValues(value);
                PhotonNetwork.AuthValues = _authenticationValues;
                PlayerPrefs.SetString(PlayerNamePrefKey, value);
            }
        }
    }
    #endregion

    #endregion

    #region Methods
    #region Original Methods    
    #region Loutre
    public void ConnectAtLaunch()
    {
        int _tempID = 1;//Random.Range(0,9999);
        PhotonNetwork.offlineMode = false;
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(_tempID.ToString());
            TDS_GameManager.IsOnline = true; 
        }
    }

    public void DemoTest(string _iD)
    {
        PhotonNetwork.ConnectUsingSettings(_iD);
    }

    public void EnableOfflineMode()
    {
        if (PhotonNetwork.connected) PhotonNetwork.Disconnect(); 
        PhotonNetwork.offlineMode = true;
        string _randomID = "LocalRoom" + Random.Range(1, 999); 
        PhotonNetwork.JoinOrCreateRoom(_randomID, new RoomOptions() { MaxPlayers = 1 }, null);
    }
    //♥
    #endregion

    #region Lobby Methods    

    public void InitDisconect()
    {
        PhotonNetwork.Disconnect(); 
    }

    /// <summary>
    /// Called when the player leave the room in the Main Menu
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }

    public void LockRoom()
    {
        if (PhotonNetwork.connected && !PhotonNetwork.offlineMode)
            PhotonNetwork.room.IsOpen = false;
    }

    void PlayerCount()
    {
        bool _canLaunch = PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch && PhotonNetwork.isMasterClient ? true : false;
        TDS_UIManager.Instance.UpdatePlayerCount(PhotonNetwork.room.PlayerCount, _canLaunch, PhotonNetwork.playerList); 
    }

    public void SelectRoom(Button _btn)
    {
        if (!PhotonNetwork.connected) return; 
        RoomId _roomId;

        _roomId = _btn.name == "FirstRoomButton" ? RoomId.FirstRoom :
                  _btn.name == "SecondRoomButton" ? RoomId.SecondRoom :
                  _btn.name == "ThirdRoomButton" ? RoomId.ThirdRoom :
                  _btn.name == "FourthRoomButton" ? RoomId.FourthRoom :
                  _btn.name == "FifthRoomButton" ? RoomId.FifthRoom :
                  RoomId.WaitForIt;

        roomName = _btn.name;

        RoomInfo[] _rooms = PhotonNetwork.GetRoomList();
        RoomInfo _roomInfo;
        for (int _i = 0; _i < _rooms.Length; _i++)
        {
            _roomInfo = _rooms[_i];
            if (_roomInfo.Name == roomName && (!_roomInfo.IsOpen) || _roomInfo.PlayerCount == _roomInfo.MaxPlayers)
            {
                TDS_UIManager.Instance?.ActivateErrorBox("This room is already full or in game!\nPlease select another room to enjoy the game!");
                return;
            }
        }

        if (_roomId == RoomId.WaitForIt)
        {
            Debug.LogError("Can't connect to the room");
            return;
        }

        TDS_UIManager.Instance.RoomSelectionManager.SetRoomsInterractable(false); 

        if (roomName == string.Empty) roomName = "RoomTest";
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        PhotonNetwork.LeaveLobby(); 
        Debug.Log("room name : " + roomName);

    }
    #endregion

    /// <summary>
    /// Called in UI, when the player wants to leave the game
    /// Disconnect him a get him to the main menu
    /// </summary>
    public void LeaveGame()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "ForceLeave", new object[] { });
        }
        else
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "RemovePlayer", new object[] { PhotonNetwork.player.ID });
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "RemovePlayerLifeBar", new object[] { (int)TDS_GameManager.LocalPlayer });
        }

        PhotonNetwork.SendOutgoingCommands();
        ForceLeave();
    }

    private void ForceLeave()
    {
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        PhotonNetwork.Disconnect();

        TDS_SceneManager.Instance.PrepareSceneLoading(0, (int)UIState.InMainMenu);
    }
    #endregion

    #region PhotonMethods
    /// <summary>
    /// When the player create a room, he's the host of the game
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        isHost = true;
    }
    public override void OnDisconnectedFromPhoton()
    {
        InitDisconect();
        TDS_GameManager.IsOnline = false; 
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");
        if (PhotonNetwork.offlineMode) return; 
        TDS_UIManager.Instance?.ActivateMenu((int)UIState.InCharacterSelection); 

        PlayerCount();
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.offlineMode) return;
        //PlayerCount();
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.offlineMode) return;
        //PlayerCount();
    }
    #endregion

    #region Unity Methods    
    private void Awake()
    {
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return; 
        }
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        return;
        GUILayout.Box(PhotonNetwork.GetPing().ToString()); 
        GUILayout.Box(PhotonNetwork.connectionStateDetailed.ToString());
    }
#endif
    #endregion
    #endregion
}