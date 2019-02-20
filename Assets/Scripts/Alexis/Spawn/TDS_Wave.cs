using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TDS_Wave 
{
    /* TDS_Wave :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Contains all spawn Points to activate for the current wave
     *	Contain a debugColor to display informations on the scene
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     * 	Date :			[18/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the GetWavesEnemies method]
	 *      - Get all enemies that spawn in this wave
     *  
	 *	-----------------------------------
     *	
	 *	Date :			[14/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the TDS_Wave class]
	 *      - Set the Fields and properties spawnPoints, debugColor and isWaveFoldOut
     *  
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Editor 
    /// <summary>
    /// Is the wave foldout in the Editor
    /// </summary>
    [SerializeField] private bool isWaveFoldOut = false;

    /// <summary>
    /// The color of the wave in the editor
    /// </summary>
    [SerializeField] protected Color debugColor = Color.red;
    /// <summary>
    /// Property of the field debugColor
    /// </summary>
    public Color DebugColor { get { return debugColor; } }
    #endregion 

    /// <summary>
    /// List of all spawn points called by this area
    /// </summary>
    [SerializeField] protected List<TDS_SpawnPoint> spawnPoints = new List<TDS_SpawnPoint>();
    /// <summary>
    /// Property of the field spawnPoints
    /// </summary>
    public List<TDS_SpawnPoint> SpawnPoints { get { return spawnPoints; } }

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// For each point, spawn enemies and return the list of all spawned enemies
    /// </summary>
    /// <returns>List of all spawned enemies</returns>
    public List<TDS_Enemy> GetWaveEnemies(TDS_SpawnerArea _owner)
    {
        List<TDS_Enemy> _enemies = new List<TDS_Enemy>();
        foreach (TDS_SpawnPoint _point in spawnPoints)
        {
            _enemies.AddRange(_point.GetSpawningEnemies(_owner)); 
        }
        return _enemies; 
    }
    #endregion

	#endregion
}
