using System.Collections;
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
}