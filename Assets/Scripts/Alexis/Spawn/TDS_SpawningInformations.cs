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
    [SerializeField] protected string spawningEnemyName;
    public string SpawningEnemyName { get { return spawningEnemyName; } }

    /// <summary>
    /// Number of enemies to spawn 
    /// </summary>
    [SerializeField] protected int numberOfEnemies = 1;

    public TDS_SpawningInformations(TDS_Enemy _e)
    {
        spawningEnemyName = _e.EnemyName;
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
    [SerializeField] private float spawnChance = 100;

    public TDS_RandomSpawningInformations(TDS_Enemy _e) : base(_e)
    {
        spawningEnemyName = _e.EnemyName;
    }
}
