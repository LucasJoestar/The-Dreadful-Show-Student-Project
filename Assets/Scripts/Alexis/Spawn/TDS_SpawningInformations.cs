using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TDS_SpawningInformations
{
    /* TDS_SpawningInformations :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Contains the kind and number of enemies to spawn
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Spawning Information class]
     *	    - Création des variables spawningEnemies et number of enemies
	 *
	 *	-----------------------------------
	*/
    /// <summary>
    /// Kind of enemies to spawn
    /// </summary>
    [SerializeField] protected string enemyResourceName;
    /// <summary>
    /// Property of the enemyResourceName field
    /// </summary>
    public string EnemyResourceName { get { return enemyResourceName; } }

    /// <summary>
    /// Number of enemies to spawn 
    /// </summary>
    [SerializeField] protected int[] enemyCount; 
    /// <summary>
    /// Property of the enemyCount Field
    /// </summary>
    public int[] EnemyCount
    {
        get
        {
            return enemyCount;
        }
    }

    [SerializeField] protected bool isFoldOut = true;

    /// <summary>
    /// Constructor of the TDS_SpawningInformation  class
    /// Set the enemy resources name as the name of the enemy in argument
    /// </summary>
    /// <param name="_e">enemy</param>
    public TDS_SpawningInformations(TDS_Enemy _e)
    {
        enemyResourceName = _e.EnemyName;
        enemyCount = new int[4] { 0, 0, 1, 1 };
    }
}

[Serializable]
public class TDS_RandomSpawningInformations : TDS_SpawningInformations
{
    /* TDS_RandomSpawningInformations :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Contains the kind and number of enemies to spawn.
     *	Also contains the chances to spawn this kind of enemies
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Spawning Information class]
     *	    - Création des variables spawningEnemies et number of enemies
     *	    - Création de la variable spawnChance
	 *
	 *	-----------------------------------
	*/

    /// <summary>
    /// Chance of spawning for the random enemy
    /// </summary>
    [SerializeField] private int spawnChance = 100;
    /// <summary>
    /// Property of the spawnChance field
    /// </summary>
    public int SpawnChance { get { return spawnChance; } }

    /// <summary>
    /// Constructor of TDS_RandomSpawningInformations based on the construcor of TDS_SpawningInformations
    /// </summary>
    /// <param name="_e">selected enemy</param>
    public TDS_RandomSpawningInformations(TDS_Enemy _e) : base(_e)
    {

    }
}
