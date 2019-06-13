using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
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
    [SerializeField,Range(1,4)]
    int minimumPlayerToLaunch = 1;
    string roomName = string.Empty;
    [Space]
    [SerializeField]
    TMP_Text textPlayerCounter;
    #endregion

    #region Methods
    #region Lobby Methods   
    void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("room name : " + roomName);
    }

    void InitMulti()
    {
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
        bool _canLaunch = PhotonNetwork.room.PlayerCount >= minimumPlayerToLaunch && PhotonNetwork.isMasterClient ? true : false ;
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

    #region Photon Methods   
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to Master");
        CreateRoom();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("room created");
        PhotonNetwork.JoinLobby();
    }
    public override void OnDisconnectedFromPhoton()
    {
        InitMulti();        
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("connected to Lobby");
        PhotonNetwork.JoinRoom(roomName);        
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
    private void Start()
    {
        InitMulti();
    }
    #endregion
    #endregion
}