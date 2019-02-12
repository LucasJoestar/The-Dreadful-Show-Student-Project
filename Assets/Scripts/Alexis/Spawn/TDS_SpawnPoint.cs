using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

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

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>
    /// Index of the wave, the wave will be called using this index
    /// </summary>
    [SerializeField] private int waveIndex = 0;

    /// <summary>
    /// Range where the enemy can be instanciated around the spawn Position
    /// </summary>
    [SerializeField] private float spawnRange;

    /// <summary>
    /// Position used when the enemy has to be instantiated
    /// </summary>
    [SerializeField] private Vector3 spawnPosition;

    /// <summary>
    /// Wave Element of the SpawnPoint
    /// </summary>
    [SerializeField] private TDS_WaveElement waveElement; 
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Add a spawning information into the wave element of this point
    /// If this info is random, add it to the randomSpawningInformations
    /// else add this info to the spawningInformations
    /// </summary>
    /// <param name="_enemy">enemy to spawn</param>
    /// <param name="_isRandom">is this info is random?</param>
    public void AddSpawningInformations(TDS_Enemy _enemy, bool _isRandom)
    {
        if (_isRandom)
        {
            waveElement.RandomSpawningInformations.Add(new TDS_RandomSpawningInformations(_enemy));
            return; 
        }
        waveElement.SpawningInformations.Add(new TDS_SpawningInformations(_enemy));
    }
    /// <summary>
    /// Check if the enemy is already in the spawning informations list 
    /// </summary>
    /// <param name="_enemy">enemy to check</param>
    /// <returns>is the enemy is already in the list spawning Enemy</returns>
    public bool ExistsEnemy(TDS_Enemy _enemy)
    {
        return waveElement.SpawningInformations.Any(i => i.SpawningEnemyName == _enemy.EnemyName); 
    }
    /// <summary>
    /// Check if the enemy is already in the random spawning informations list 
    /// </summary>
    /// <param name="_enemy">enemy to check</param>
    /// <returns>is the enemy is already in the list random spawning Enemy</returns>
    public bool ExistsRandomEnemy(TDS_Enemy _enemy)
    {
        return waveElement.RandomSpawningInformations.Any(i => i.SpawningEnemyName == _enemy.EnemyName);
    }
    #endregion

    #endregion
}
