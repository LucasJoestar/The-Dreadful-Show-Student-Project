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
                PlayerPrefs.SetString(PlayerNamePrefKey, value);
            }
        }
    }
    #endregion

    #endregion

    #region Methods
    #region Original Methods    
    #region Loutre
    public void DemoTest(string _iD)
    {
        PhotonNetwork.ConnectUsingSettings(_iD);
    }

    public void ConnectAtLaunch()
    {
        int _tempID = Random.Range(0,9999);

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings(_tempID.ToString());
        }
    }
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
        if (PlayerPrefs.HasKey(PlayerNamePrefKey))
        {
            PlayerNamePrefKey = PlayerPrefs.GetString(PlayerNamePrefKey);
            TDS_UIManager.Instance.PlayerNameField.text = playerNamePrefKey; 
        }
        #endregion
    }

    /// <summary>
    /// Called when the player leave the room in the Main Menu
    /// </summary>
    public void LeaveRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LeaveRoom"), new object[] { });
        }

        TDS_UIManager.Instance?.SetButtonsInterractables(false);
        TDS_UIManager.Instance.LocalIsReady = false;
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        TDS_UIManager.Instance?.SelectCharacter((int)PlayerType.Unknown);
        PhotonNetwork.Disconnect();
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
        RoomId _roomId;

        _roomId = _btn.name == "FirstRoomButton" ? RoomId.FirstRoom :
                  _btn.name == "SecondRoomButton" ? RoomId.SecondRoom :
                  _btn.name == "ThirdRoomButton" ? RoomId.ThirdRoom :
                  _btn.name == "FourthRoomButton" ? RoomId.FourthRoom :
                  _btn.name == "FifthRoomButton" ? RoomId.FifthRoom :
                  RoomId.WaitForIt;

        roomName = _btn.name;

        if (PhotonNetwork.GetRoomList().Any(r => r.Name == roomName && (!r.IsOpen || r.PlayerCount == r.MaxPlayers))) return;

        if (_roomId == RoomId.WaitForIt)
        {
            Debug.LogError("Can't connect to the room");
            return;
        }

        int _getIndex = (int)_roomId;
        string _stringID = _getIndex.ToString();
        PhotonNetwork.gameVersion = _stringID;

        if (roomName == string.Empty) roomName = "RoomTest";
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
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
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        PhotonNetwork.Disconnect();

        TDS_SceneManager.Instance.PrepareSceneLoading(0);
    }

    #region Player   
    public void SetPlayerName(string _nickname)
    {
        PhotonNetwork.playerName = _nickname + " ";
        PlayerPrefs.SetString(PlayerNamePrefKey, _nickname);
    }
    #endregion
    #endregion

    #region PhotonMethods
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
        InitDisconect();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");

        TDS_UIManager.Instance?.ActivateMenu((int)UIState.InCharacterSelection); 
        TDS_UIManager.Instance?.SetButtonsInterractables(true);

        PlayerCount();
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", newPlayer, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, typeof(TDS_UIManager), "UpdateSelectionButtons"), new object[] { (int)TDS_GameManager.LocalPlayer });
        if(PhotonNetwork.isMasterClient && TDS_UIManager.Instance && !TDS_UIManager.Instance.PlayerListReady.ContainsKey(newPlayer))
        {
            TDS_UIManager.Instance.PlayerListReady.Add(newPlayer, false); 
        }
        PlayerCount();
    }
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.isMasterClient && TDS_UIManager.Instance && TDS_UIManager.Instance.PlayerListReady.ContainsKey(otherPlayer))
        {
            TDS_UIManager.Instance.PlayerListReady.Remove(otherPlayer);
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
    #if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.Box(PhotonNetwork.GetPing().ToString()); 
        GUILayout.Box(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Box(PhotonNetwork.isMasterClient.ToString());
        GUILayout.Box(TDS_GameManager.LocalPlayer.ToString()); 
    }
    #endif

    void Start ()
    {
        if(!photonView) photonView = GetComponent<PhotonView>();
        InitMulti();
    }
    #endregion
    #endregion
}