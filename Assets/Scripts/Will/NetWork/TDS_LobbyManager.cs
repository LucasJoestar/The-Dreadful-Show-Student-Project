using System.Collections;
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
}