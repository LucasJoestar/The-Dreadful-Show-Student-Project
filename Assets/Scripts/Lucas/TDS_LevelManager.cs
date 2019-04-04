﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_LevelManager : MonoBehaviour 
{
    /* TDS_LevelManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the behaviour of a level.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	... Mhmm...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[01 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LevelManager class.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called when a new checkpoint is set.
    /// </summary>
    public Action<TDS_Checkpoint> OnCheckpointActivated = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Last activated checkpoint.
    /// </summary>
    [SerializeField] private TDS_Checkpoint checkpoint = null;

    /// <summary>Public accessor for <see cref="checkpoint"/>.</summary>
    public TDS_Checkpoint Checkpoint { get { return checkpoint; } }

    /// <summary>
    /// Local player, the one who play on this machine.
    /// </summary>
    [SerializeField] private TDS_Player localPlayer = null;

    /// <summary>
    /// Online players, the ones that play with the one playing on this machine.
    /// </summary>
    [SerializeField] private List<TDS_Player> onlinePlayers = new List<TDS_Player>();

    /// <summary>
    /// Get all players of the game, local and online ones.
    /// </summary>
    public TDS_Player[] AllPlayers
    {
        get { return onlinePlayers.Append(localPlayer).ToArray(); }
    }

    /// <summary>
    /// Points where to spawn at game start.
    /// </summary>
    public Vector3[] StartSpawnPoints = new Vector3[] { };
    #endregion

    #region Proto
    [SerializeField] private GameObject player = null;
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static TDS_LevelManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Make local player spawn.
    /// </summary>
    public void Spawn()
    {
        // Test things
        localPlayer = Instantiate(player, StartSpawnPoints[0], Quaternion.identity).GetComponentInChildren<TDS_Player>();
        TDS_Camera.Instance.Target = localPlayer.transform;

        localPlayer.OnDie += Respawn;

        // Instantiate the player
        PlayerType _type = player.GetComponentInChildren<TDS_Player>().PlayerType;
        //TDS_NetworkManager.Instance.InstantiatePlayer(_type);

    }

    /// <summary>
    /// Make Player which a particulary type spawn and set it as the camera target
    /// </summary>
    /// <param name="_playerType"></param>
    public void Spawn(PlayerType _playerType)
    {
        localPlayer = TDS_NetworkManager.Instance.InstantiatePlayer(_playerType, StartSpawnPoints[0]).GetComponent<TDS_Player>();
        TDS_Camera.Instance.Target = localPlayer.transform;
        TDS_UIManager.Instance?.SetPlayerLifeBar(localPlayer);
    }

    /// <summary>
    /// Make all dead players respawn.
    /// </summary>
    private void Respawn()
    {
        if (checkpoint) checkpoint.Respawn();
        else
        {
            // Test use
            TDS_Player[] _deadPlayers = AllPlayers.Where(p => p.IsDead).ToArray();

            if (_deadPlayers.Length == 0) return;

            TDS_Player _player = null;

            for (int _i = 0; _i < _deadPlayers.Length; _i++)
            {
                _player = _deadPlayers[_i];

                _player.transform.position = StartSpawnPoints[_i];
                _player.HealthCurrent = _player.HealthMax;
                _player.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Set new checkpoint where to respawn.
    /// </summary>
    /// <param name="_checkpoint">Checkpoint to set as new one.</param>
    public void SetCheckpoint(TDS_Checkpoint _checkpoint)
    {
        if (_checkpoint == null) return;

        checkpoint = _checkpoint;
        OnCheckpointActivated?.Invoke(_checkpoint);

        // Make dead players respawn
        Respawn();
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;
        else Destroy(this);
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        // Nullify the singleton instance if needed
        if (Instance == this) Instance = null;
    }

    // Use this for initialization
    void Start()
    {
        TDS_NetworkManager.Instance.InitConnection();

        // Spawn local player.
        //Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #endregion
}
