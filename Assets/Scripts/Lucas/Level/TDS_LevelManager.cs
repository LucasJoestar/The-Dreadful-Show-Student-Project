using Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_LevelManager : PunBehaviour 
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
    public static Action<TDS_Checkpoint> OnCheckpointActivated = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Last activated checkpoint.
    /// </summary>
    [SerializeField] protected TDS_Checkpoint checkpoint = null;

    /// <summary>Public accessor for <see cref="checkpoint"/>.</summary>
    public TDS_Checkpoint Checkpoint { get { return checkpoint; } }

    /// <summary>
    /// Get the ID of this object photon view.
    /// </summary>
    public int phID { get { return photonView.viewID; } }

    /// <summary>
    /// Local player, the one who play on this machine.
    /// </summary>
    [SerializeField] protected TDS_Player localPlayer = null;

    /// <summary>Public accessor for <see cref="localPlayer"/>.</summary>
    public TDS_Player LocalPlayer { get { return localPlayer; } }

    /// <summary>
    /// Online players, the ones that play with the one playing on this machine.
    /// </summary>
    [SerializeField] protected List<TDS_Player> onlinePlayers = new List<TDS_Player>();

    /// <summary>Public accessor for <see cref="onlinePlayers"/>.</summary>
    public List<TDS_Player> OnlinePlayers { get { return onlinePlayers; } }

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

    #region Singleton
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static TDS_LevelManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Make Player which a particulary type spawn and set it as the camera target
    /// </summary>
    /// <param name="_playerType"></param>
    public void Spawn(PlayerType _playerType)
    {
        if(PhotonNetwork.connected)
        {
            if (_playerType == PlayerType.Juggler)
                localPlayer = PhotonNetwork.Instantiate(_playerType.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponentInChildren<TDS_Player>();
            else localPlayer = PhotonNetwork.Instantiate(_playerType.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponent<TDS_Player>();
        }
        else
        {
            TDS_GameManager.LocalPlayer = _playerType; 
            if (_playerType == PlayerType.Juggler)
                localPlayer = (Instantiate(Resources.Load(_playerType.ToString()), StartSpawnPoints[0], Quaternion.identity) as GameObject).GetComponentInChildren<TDS_Player>();
            else localPlayer = (Instantiate(Resources.Load(_playerType.ToString()), StartSpawnPoints[0], Quaternion.identity) as GameObject).GetComponent<TDS_Player>();
        }
        TDS_Camera.Instance.Target = localPlayer.transform;

    }

    /// <summary>
    /// Make the player with the type contained in the GameManager spawn
    /// </summary>
    public void Spawn()
    {
        if (!PhotonNetwork.connected) return; 
        if (TDS_GameManager.LocalPlayer == PlayerType.Juggler)
            localPlayer = PhotonNetwork.Instantiate(TDS_GameManager.LocalPlayer.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponentInChildren<TDS_Player>();
        else localPlayer = PhotonNetwork.Instantiate(TDS_GameManager.LocalPlayer.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponent<TDS_Player>();
        TDS_Camera.Instance.Target = localPlayer.transform;
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

    /// <summary>
    /// Add the player in the list of online players
    /// </summary>
    /// <param name="_onlinePlayer">Player to add to the onlinePlayer List</param>
    public void InitOnlinePlayer(TDS_Player _onlinePlayer)
    {
        onlinePlayers.Add(_onlinePlayer);
    }

    /// <summary>
    /// Remove the player with the selected id from the onlinePlayers list
    /// </summary>
    /// <param name="_playerId">Id of the removed player</param>
    public void RemoveOnlinePlayer(TDS_Player _player)
    {
        if (!_player)
        {
            Debug.LogError("Player Not Found"); 
            return;
        }
        TDS_UIManager.Instance.ClearUIRelatives(_player.PlayerType);
        if (onlinePlayers.Contains(_player)) onlinePlayers.Remove(_player);
    }

    /// <summary>
    /// Check the count of living players, if there is no player alive, reload the scene
    /// </summary>
    public void CheckLivingPlayers()
    {
        if (localPlayer.IsDead && !OnlinePlayers.Any(p => !p.IsDead))
            TDS_SceneManager.Instance?.PrepareSceneLoading(TDS_GameManager.CurrentSceneIndex); 
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }
 
    #endregion

    #endregion
}
