using System.Linq;
using System.Collections.Generic; 
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
       
    #region Fields / Properties
    #region Lobby
    [Space]
    [SerializeField, Range(1, 4)]
    int minimumPlayerToLaunch = 1;
    string roomName = string.Empty;

    [Space]
    [SerializeField]
    new PhotonView photonView;

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

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(_tempID.ToString());
        }
    }

    public void DemoTest(string _iD)
    {
        PhotonNetwork.ConnectUsingSettings(_iD);
    }

    public void EnableOfflineMode()
    {
        PhotonNetwork.offlineMode = true;
    }
    //♥
    #endregion

    #region Lobby Methods    

    void InitDisconect()
    {
        InitMulti();
        //Application.wantsToQuit -= LeaveGame;
    }

    void InitMulti()
    {
        #region Player
        /*
        if (PlayerPrefs.HasKey(PlayerNamePrefKey))
        {
            PlayerNamePrefKey = PlayerPrefs.GetString(PlayerNamePrefKey);
            TDS_UIManager.Instance.PlayerNameField.text = playerNamePrefKey; 
        }
        */
        #endregion
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
        PhotonNetwork.room.IsOpen = false;
    }

    void PlayerCount()
    {
        bool _canLaunch = PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch && PhotonNetwork.isMasterClient ? true : false;
        TDS_UIManager.Instance?.UpdatePlayerCount(PhotonNetwork.room.PlayerCount, _canLaunch, PhotonNetwork.playerList); 
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

        if (PhotonNetwork.GetRoomList().Any(r => r.Name == roomName && (!r.IsOpen || r.PlayerCount == r.MaxPlayers)))
        {
            TDS_UIManager.Instance?.ActivateErrorBox("This room is already full or in game!\nPlease select another room to enjoy the game!");
            return;
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
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LeaveGame"), new object[] { });
        }
        else
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "RemovePlayer"), new object[] { PhotonNetwork.player.ID });
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "RemovePlayerLifeBar"), new object[] { (int)TDS_GameManager.LocalPlayer }); 
        }
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        PhotonNetwork.Disconnect();

        TDS_SceneManager.Instance.PrepareSceneLoading("MainMenu");
        TDS_UIManager.Instance.ActivateMenu(UIState.InMainMenu); 
    }

    #region Player   
    /*
    public void SetPlayerName(string _nickname)
    {
        PhotonNetwork.playerName = _nickname + " ";
        PlayerPrefs.SetString(PlayerNamePrefKey, _nickname);
    }
    */
    #endregion
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
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");

        TDS_UIManager.Instance?.ActivateMenu((int)UIState.InCharacterSelection); 

        PlayerCount();
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if(PhotonNetwork.isMasterClient && TDS_UIManager.Instance && !TDS_GameManager.PlayerListReady.ContainsKey(newPlayer))
        {
            TDS_GameManager.PlayerListReady.Add(newPlayer, false); 
        }
        PlayerCount();
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.isMasterClient && TDS_UIManager.Instance && TDS_GameManager.PlayerListReady.ContainsKey(otherPlayer))
        {
            TDS_GameManager.PlayerListReady.Remove(otherPlayer);
        }
        PlayerCount();
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

    private void OnGUI()
    {
        GUILayout.Box(PhotonNetwork.GetPing().ToString()); 
        GUILayout.Box(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Box($"PLAYER USERNAME: {PhotonNetwork.playerName}\nPLAYER USER ID: {PhotonNetwork.player.UserId}\nID: {PhotonNetwork.player.ID}");
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