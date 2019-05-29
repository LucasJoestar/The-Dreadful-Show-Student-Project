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
    [Space]   
    [SerializeField]
    GameObject roomsUIRoot;
    [Space]
    [SerializeField]
    GameObject launchButton;
    [Space]
    [SerializeField]
    GameObject leaveButton;
    [Space]
    [SerializeField, Range(1, 4)]
    int minimumPlayerToLaunch = 1;
    string roomName = string.Empty;
    [Space]
    [SerializeField]
    new PhotonView photonView;
    [Space]
    [SerializeField]
    TMP_Text textPlayerCounter;
    #endregion
    //
    public static TDS_NetworkManager Instance;
    #region Player     
    PlayerType localPlayer = PlayerType.BeardLady;
    bool isHost = false;
    public bool IsHost { get { return isHost; } }
    static string PlayerNamePrefKey = "PlayerName";
    [Space]
    [SerializeField]
    TMP_InputField nameInputField;
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
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("room name : " + roomName);
    }

    void InitMulti()
    {
        #region Player
        string _defaultName = " ";
        //TMP_InputField _nameInputField = GetComponent<TMP_InputField>();
        if(nameInputField != null)
        {
            if (PlayerPrefs.HasKey(PlayerNamePrefKey))
            {
                _defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
                nameInputField.text = _defaultName;
            }
        }
        PhotonNetwork.playerName = _defaultName;
        #endregion

        #region UI
        launchButton.SetActive(false);
        leaveButton.SetActive(false);
        roomsUIRoot.SetActive(true);
        textPlayerCounter.gameObject.SetActive(false);
        #endregion        
    }

    public void LaunchNLoadGame()
    {
        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch)
            PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
    }

    void PlayerCount()
    {
        if (!textPlayerCounter.gameObject.activeInHierarchy)
            textPlayerCounter.gameObject.SetActive(true);
        textPlayerCounter.text = $"Player : {PhotonNetwork.room.PlayerCount}/4";
        bool _canLaunch = PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch && PhotonNetwork.isMasterClient ? true : false;
        launchButton.SetActive(_canLaunch);
    }

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

        roomsUIRoot.SetActive(false);
    }
    #endregion

    #region Player
    /// <summary>
    /// Spawn player based on the Enum PlayerType
    /// </summary>
    /// <param name="_playerType"></param>
    /// <returns></returns>
    public PhotonView InstantiatePlayer(PlayerType _playerType, Vector3 _spawnPosition)
    {
        PhotonView _playerId = PhotonNetwork.Instantiate(_playerType.ToString(), _spawnPosition, Quaternion.identity, 0).GetComponent<PhotonView>();
        localPlayer = _playerType;

        return _playerId;
    }
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
    /// <summary>
    /// When the player joins the room, instantiate a prefab for the player and set its name with the player name
    /// </summary>
    public override void OnDisconnectedFromPhoton()
    {
        InitMulti();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("connected to Room there is : " + PhotonNetwork.room.PlayerCount + " player here !!");
        leaveButton.SetActive(true);
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
    }
    void Start ()
    {
        if(!photonView) photonView = GetComponent<PhotonView>();
        InitMulti();
    }
    #endregion
    #endregion
}