using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    #region Events

    #endregion

    #region Fields / Properties
    public static TDS_NetworkManager Instance;
    #region Conection settings
    
    #endregion
    #region Photon
    [SerializeField, Header("Photon settings")]
    new PhotonView  photonView ;
    [SerializeField]
    string connectionVersion = "1.1";
    [SerializeField]
    string roomName = "TDS_EPIIC";
    bool canLeave = false;
    #endregion
    #region Player     
    //Player type in local
    PlayerType localPlayer = PlayerType.BeardLady;
    bool isHost = false;
    public bool IsHost { get { return isHost; } }

    //Player name in local
    //[SerializeField]
    //string playerName = "Player";
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Connect the player to Photon
    /// </summary>
    public void InitConnection()
    {
        PhotonNetwork.ConnectUsingSettings(connectionVersion);
    }
    /// <summary>
    /// Spawn player based on the Enum PlayerType
    /// </summary>
    /// <param name="_playerType"></param>
    /// <returns></returns>
    public TDS_Player InstantiatePlayer (PlayerType _playerType, Vector3 _spawnPosition)
    {
        PhotonView _playerId = PhotonNetwork.Instantiate(_playerType.ToString(), _spawnPosition, Quaternion.identity, 0).GetComponent<PhotonView>();
        localPlayer = _playerType;

        return null;
    }
    /// <summary> 
    /// Join the room if this room already exist else create it with somme parameters 
    /// </summary> 
    void JoinRoom()
    {
        RoomOptions _options = new RoomOptions()
        {
            IsVisible = true,
            MaxPlayers = 4,
        };
        PhotonNetwork.JoinOrCreateRoom(roomName,_options,null);
        Debug.Log(roomName);
    }
    #region PhotonMethods
    /// <summary>
    /// When the player is connected to master, he joins the room
    /// </summary>
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        JoinRoom();
    }
    /// <summary>
    /// When the player create a room, he's the host of the game
    /// </summary>
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        isHost = true;
    }

    public void OnleaveRoom()
    {

    }
    
    /// <summary>
    /// When the player joins the room, instantiate a prefab for the player and set its name with the player name
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (!PhotonNetwork.isMasterClient)
        {
           //SendInGamePlayers
        }
    }
    #endregion
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
        //InitConnection(); //Deplacé dans le GameManager
        if(!photonView)
        photonView = GetComponent<PhotonView>();
    }	
	void Update ()
    {
        
	}    
    #endregion
    #endregion
}