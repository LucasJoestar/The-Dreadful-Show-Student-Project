using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(BoxCollider))]
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
     * 	Date :			[14/02/2019]
	 *	Author :		[Thiebaut Alexiss]
	 *
	 *	Changes :
	 *
	 *	Remplacement de la list de spawn points par une liste de vagues qui sera parcourrue pour avancer dans les vagues
	 *
	 *	-----------------------------------
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
    /// List of dead enemies enemies belonging to this area
    /// </summary>
    private List<TDS_Enemy> deadEnemies = new List<TDS_Enemy>();

    /// <summary>
    /// List of enemies belonging to this area
    /// </summary>
    private List<TDS_Enemy> spawnedEnemies = new List<TDS_Enemy>();

    [SerializeField] List<TDS_Wave> waves = new List<TDS_Wave>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods

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

    private void OnTriggerEnter(Collider _coll)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .2f); 
        for (int i = 0; i < waves.Count; i++)
        {
            Gizmos.color = waves[i].DebugColor;
            for (int j = 0; j < waves[i].SpawnPoints.Count; j++)
            {
                Gizmos.DrawLine(transform.position, waves[i].SpawnPoints[j].SpawnPosition);
                Gizmos.DrawSphere(waves[i].SpawnPoints[j].SpawnPosition, .1f);
            }

        }
    }
    #endregion

    #endregion
}
