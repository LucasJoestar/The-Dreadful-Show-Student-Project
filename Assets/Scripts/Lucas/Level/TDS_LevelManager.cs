using Photon;
using System;
using System.Collections;
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
    /// Local player, the one who play on this machine.
    /// </summary>
    [SerializeField] protected TDS_Player localPlayer = null;

    /// <summary>Public accessor for <see cref="localPlayer"/>.</summary>
    public TDS_Player LocalPlayer { get { return localPlayer; } }

    /// <summary>
    /// Other players, that is the ones playing with you, guy. (Online and local)
    /// </summary>
    [SerializeField] protected List<TDS_Player> otherPlayers = new List<TDS_Player>();

    /// <summary>Public accessor for <see cref="otherPlayers"/>.</summary>
    public List<TDS_Player> OtherPlayers { get { return otherPlayers; } }

    /// <summary>
    /// Get all players of the game, local and online ones.
    /// </summary>
    public TDS_Player[] AllPlayers
    {
        get { return otherPlayers.Append(localPlayer).ToArray(); }
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
        if(!PhotonNetwork.offlineMode)
        {
            if (_playerType == PlayerType.Juggler)
                localPlayer = PhotonNetwork.Instantiate(_playerType.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponentInChildren<TDS_Player>();
            else localPlayer = PhotonNetwork.Instantiate(_playerType.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponent<TDS_Player>();
        }
        else
        {
            TDS_GameManager.LocalPlayer = _playerType; 
            localPlayer = (Instantiate(Resources.Load(_playerType.ToString()), StartSpawnPoints[0], Quaternion.identity) as GameObject).GetComponent<TDS_Player>();
        }
        TDS_Camera.Instance.Target = localPlayer.transform;

    }

    /// <summary>
    /// Make the player with the type contained in the GameManager spawn
    /// </summary>
    public void Spawn()
    {
        if (!PhotonNetwork.connected) return; 
        localPlayer = PhotonNetwork.Instantiate(TDS_GameManager.LocalPlayer.ToString(), StartSpawnPoints[0], Quaternion.identity, 0).GetComponent<TDS_Player>();
        TDS_Camera.Instance.Target = localPlayer.transform;
    }

    public void LocalSpawn(int _playerID, PlayerType _typeToSpawn)
    {
        TDS_Player _player = (Instantiate(Resources.Load(_typeToSpawn.ToString()), StartSpawnPoints[0], Quaternion.identity) as GameObject).GetComponent<TDS_Player>();
        if (_playerID == 0)
            TDS_Camera.Instance.Target = _player.transform;
        //Set the player ID to control it with the controller with the same id
    }

    /// <summary>
    /// Add the player in the list of online players
    /// </summary>
    /// <param name="_onlinePlayer">Player to add to the onlinePlayer List</param>
    public void InitOnlinePlayer(TDS_Player _onlinePlayer)
    {
        otherPlayers.Add(_onlinePlayer);
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
        if (otherPlayers.Contains(_player)) otherPlayers.Remove(_player);
    }

    /// <summary>
    /// Check the count of living players, if there is no player alive, reload the scene
    /// </summary>
    public IEnumerator CheckLivingPlayers()
    {
        if (!PhotonNetwork.offlineMode && localPlayer.IsDead)
        {
            yield return new WaitForSeconds(2f);

            //TDS_UIManager.Instance.ResetUIManager();
            if (OtherPlayers.All(p => p.IsDead)) TDS_UIManager.Instance.StartCoroutine(TDS_UIManager.Instance.ResetInGameUI());
            else if (OtherPlayers.Count > 0)
            {
                TDS_Camera.Instance.Target = OtherPlayers.Where(p => !p.IsDead).First().transform;
            }
            yield break; 
        }
        Debug.LogError("THIS FEATURE IS NOT IMPLEMENTED"); 
        // TO DO: Store the players in a list
        // Check if everybody is dead
        // If so, reset the Level
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
