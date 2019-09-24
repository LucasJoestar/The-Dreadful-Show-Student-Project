using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random; 

[Serializable]
public class TDS_WaveElement 
{
    /* TDS_WaveElement :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Contains informations of enemies to spawn
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     * Date :			[18/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : 
	 *
	 *	[Creation of the method Get informations]
     *	    - Get all enemies to spawn ans return them
     *	    
	 *	-----------------------------------
     *	
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : 
	 *
	 *	[Initialisation of the Wave Element class]
     *	    - Création des variables min and max random spawn et de la liste spawning Informations
     *	    
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Minimum number of additional random enemies to spawn
    /// </summary>
    [SerializeField] private int[] minRandomSpawn = new int[4] { 1,1,1,1 };
    /// <summary>
    /// Maximal number of additional random enemies to spawn
    /// </summary>
    [SerializeField] private int[] maxRandomSpawn = new int[4] { 0, 0, 0, 0 }; 

    /// <summary>
    /// List of enemies and number of enemies linked to this SpawnPoint
    /// </summary>
    [SerializeField] private List<TDS_SpawningInformations> spawningInformations = new List<TDS_SpawningInformations>();

    /// <summary>
    /// List of enemies and number of enemies linked to this SpawnPoint
    /// </summary>
    [SerializeField] private List<TDS_RandomSpawningInformations> randomSpawningInformations = new List<TDS_RandomSpawningInformations>();

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get the name of all enemies to spawn at this point
    /// Get all the static enemies
    /// Also get additional random enemies
    /// </summary>
    /// <returns>list of all enemies to spawn</returns>
    public List<string> GetInformations()
    {
        List<string> _enemies = new List<string>();
        // Get enemies
        foreach (TDS_SpawningInformations _info in spawningInformations)
        {
            for (int i = 0; i < _info.EnemyCount[TDS_GameManager.PlayerCount - 1]; i++)
            {
                _enemies.Add(_info.EnemyResourceName);
            }
        }

        // GET RANDOM ENEMIES
        if (randomSpawningInformations.Count == 0) return _enemies; 
        //Get the count of additional enemies to spawn
        int _randomCount = Random.Range(minRandomSpawn[TDS_GameManager.PlayerCount - 1], maxRandomSpawn[TDS_GameManager.PlayerCount - 1] + 1);
        // if no more enemies to spawn, return the enemies list
        if (_randomCount == 0) return _enemies;
        int _max = 0;
        int _value = 0;
        int _referenceValue = 0; 
        for (int i = 0; i < _randomCount; i++)
        {
            //Get the total of all spawn chances 
            _max = randomSpawningInformations.Where(a => a.EnemyCount[TDS_GameManager.PlayerCount - 1] > 0).Sum(a => a.SpawnChance);
            if (_max == 0) break;
            //Get a random value
            _value = Random.Range(0, _max);
            for (int j = 0; j < randomSpawningInformations.Count; j++)
            {
                //Reference value is the sum of every previous spawnchance added to the current spawn chance
                _referenceValue += randomSpawningInformations[j].SpawnChance;
                //If the value is less than the reference value
                if (_value <= _referenceValue && randomSpawningInformations[j].EnemyCount[TDS_GameManager.PlayerCount - 1] > 0)
                {
                    //Add the enemy name to the list and decrease the enemy count for the selected enemy
                    _enemies.Add(randomSpawningInformations[j].EnemyResourceName);
                    randomSpawningInformations[j].EnemyCount[TDS_GameManager.PlayerCount - 1]--; 
                    break; 
                }
            }
            //Reset the reference value to 0
            _referenceValue = 0; 
        }
        return _enemies; 
    }
    #endregion

    #endregion
}