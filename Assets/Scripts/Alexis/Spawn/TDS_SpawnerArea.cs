using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class TDS_SpawnerArea : PunBehaviour
{
    /* TDS_SpawnerArea :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	 - Contains all spawn points of the zone.
     *	 - Call every waves when the previous one is cleared by the players
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11/02/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the SpawnerArea Class]
     *	    - Création de l'event OnNextWave
     *	    - Création des variables photonView, isLooping, waveIndex, wavesLength, deadEnemies, spawnedEnemies et spawnPoints
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// This action is called when a wave has to be started
    /// </summary>
    public Action OnNextWave;
    #endregion

    #region Fields / Properties

    #region Components and references
    /// <summary>
    /// Photon view of the area
    /// </summary>
    [SerializeField] protected PhotonView photonView;
    #endregion

    #region Variables
    /// <summary>
    /// Is the area call the first wave when the last wave is over  
    /// </summary>
    [SerializeField] protected bool isLooping = false;

    /// <summary>
    /// Current index of the area
    /// Increase this value when  
    /// </summary>
    private int waveIndex = 0;

    /// <summary>
    /// Number of waves called by this area
    /// </summary>
    [SerializeField] protected int wavesLength = 1;

    /// <summary>
    /// List of dead enemies enemies belonging to this area
    /// </summary>
    private List<TDS_Enemy> deadEnemies = new List<TDS_Enemy>();

    /// <summary>
    /// List of enemies belonging to this area
    /// </summary>
    private List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();

    /// <summary>
    /// List of all spawn points called by this area
    /// </summary>
    [SerializeField] protected List<TDS_SpawnPoint> spawnPoints = new List<TDS_SpawnPoint>();
    public List<TDS_SpawnPoint> SpawnPoints { get { return spawnPoints; } }
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    #endregion

    #region EditorMethod
    /// <summary>
    /// Add a point to the list of spawnPoints
    /// </summary>
    public void AddSpawnPoint() => spawnPoints.Add(new TDS_SpawnPoint()); 
    /// <summary>
    /// Remove a point from the list of spawnPoints at the index
    /// </summary>
    /// <param name="_index">ndex</param>
    public void RemovePoint(int _index) => spawnPoints.RemoveAt(_index); 
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {

    }

	// Use this for initialization
    private void Start()
    {
		
    }
	
	// Update is called once per frame
	private void Update()
    {
        
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .2f); 
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Gizmos.DrawLine(transform.position, spawnPoints[i].SpawnPosition);
            Gizmos.DrawSphere(spawnPoints[i].SpawnPosition, .1f); 
        }
    }
    #endregion

    #endregion
}
