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

    #region Events

    #endregion

    #region Fields / Properties
    #region Lobby
    bool canLeave = true;
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    void MyPersonalStart()
    {
        //Application.wantsToQuit += LeaveGame;
    }
    #endregion

    #region Lobby Methods   
    void CreateRoom()
    {
        if (roomName == string.Empty) roomName = "RoomTest";
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("room name : " + roomName);
    }

    void InitDisconect()
    {
        InitMulti();
        Application.wantsToQuit -= LeaveGame;
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

    bool LeaveGame()
    {
        if (canLeave)
            return true;
        PhotonNetwork.Destroy(TDS_UIManager.Instance.gameObject);
        TDS_SceneManager.Instance.PrepareSceneLoading(0);
        canLeave = true; 
        return false;        
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
        canLeave = true; 
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
        PlayerPrefs.SetString(PlayerNamePrefKey, _nickname);
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
        InitDisconect();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");

        TDS_UIManager.Instance?.SetButtonsInterractables(true);

        PlayerCount();

        canLeave = false; 
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
        Application.wantsToQuit += LeaveGame;
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