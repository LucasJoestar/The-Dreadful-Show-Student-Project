using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_GameManager : MonoBehaviour 
{
    /* TDS_GameManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manage the whole game. Yep.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[26 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Added the spawn system.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[25 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_GameManager class.
     *	
     *	    Added base content of the class.
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
    /// Indicates if the game is currently in pause.
    /// </summary>
    [SerializeField] private bool isPaused = false;

    /// <summary>
    /// Indicates if players are currently in game, or in menu.
    /// </summary>
    [SerializeField] private bool isInGame = true;

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
    public static TDS_GameManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Leaves the game, and go back to the menu.
    /// </summary>
    public void LeaveGame()
    {

    }

    /// <summary>
    /// Make local player spawn.
    /// </summary>
    public void Spawn()
    {
        localPlayer = Instantiate(player, StartSpawnPoints[0], Quaternion.identity).GetComponentInChildren<TDS_Player>();
        TDS_Camera.Instance.Target = localPlayer.transform;

        localPlayer.OnDie += Respawn;
    }

    /// <summary>
    /// Make all dead players respawn.
    /// </summary>
    private void Respawn()
    {
        if (checkpoint) checkpoint.Respawn();
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
    /// Pause or resume the game.
    /// </summary>
    /// <param name="_doPause">Should the game be paused or resumed</param>
    public void SetPause(bool _doPause)
    {
        isPaused = _doPause;
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
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
    void Start ()
    {
        // Spawn local player.
        Spawn();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}
	#endregion

	#endregion
}
