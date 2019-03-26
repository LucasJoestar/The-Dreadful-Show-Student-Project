using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Random = UnityEngine.Random; 

[Serializable]
public class TDS_SpawnPoint 
{
    /* TDS_SpawnPoint :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Point that contains a various number of enemies to spawn into a certain range
     *	Also contains spawning informations (spawned enemies, number of enemies)
     *  
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[18/02/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the GetSpawningEnemies]
     *	    - This method get all enemies to spawn from waveElement informations
     *	    - Make them spawn at random position around the range of the spawn point
     *	    - return spawned enemies
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[11/02/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the SpawnPoint Class]
     *	    - Creation des variables waveIndex, spawnRange et spawnPosition
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    /// <summary>
    /// Range where the enemy can be instanciated around the spawn Position
    /// </summary>
    [SerializeField] private float spawnRange = 1;
    /// <summary>
    /// Property of the field spawnRange 
    /// <see cref="SpawnRange"/>
    /// </summary>
    public float SpawnRange { get { return spawnRange; } }

    /// <summary>
    /// Position used when the enemy has to be instantiated
    /// </summary>
    [SerializeField] private Vector3 spawnPosition;
    /// <summary>
    /// Property of the field spawnPosition 
    /// <see cref="spawnPosition"/>
    /// </summary>
    public Vector3 SpawnPosition { get { return spawnPosition; } set { spawnPosition = value; } }

    /// <summary>
    /// Wave Element of the SpawnPoint
    /// </summary>
    [SerializeField] private TDS_WaveElement waveElement; 

    /// <summary>
    /// Return a random position around the spawnpoint within the spawnRange
    /// </summary>
    public Vector3 GetRandomSpawnPosition
    {
        get { return spawnPosition + new Vector3(Random.Range(-spawnRange, spawnRange), 0, Random.Range(-spawnRange, spawnRange)); }
    }
    #endregion

    #region Methods

    #region Original Methods

    /// <summary>
    /// Get all enemies resources Names
    /// Instantiate them and set their owner as the spawner area that owns this SpawnPoint
    /// return the list of all spawned enemies 
    /// </summary>
    /// <param name="_owner">TDS_SpawnerArea that owns the spawn point</param>
    /// <returns>List of spawned enemies</returns>
    public List<TDS_Enemy> GetSpawningEnemies(TDS_SpawnerArea _owner)
    {
        // Get enemies from the wave Element
        List<string> _enemiesNames = waveElement.GetInformations();
        //Create the list of spawned Enemies for this spawn point
        List<TDS_Enemy> _spawnedEnemies = new List<TDS_Enemy>(); 
        // Spawn every enemies from thoses informations 
        TDS_Enemy _e; 
        for (int i = 0; i < _enemiesNames.Count; i++)
        {
            _e = PhotonNetwork.Instantiate(_enemiesNames[i], GetRandomSpawnPosition, Quaternion.identity, 0).GetComponent<TDS_Enemy>();
            _e.Area = _owner;
            //INIT LIFEBAR
            if(TDS_UIManager.Instance.CanvasWorld)
            {
                
            }
            _spawnedEnemies.Add(_e); 
        }
        return _spawnedEnemies; 
    }
    #endregion

    #endregion
}
