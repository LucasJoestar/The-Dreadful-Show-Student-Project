using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : 
	 *
	 *	[Initialisation of the Wave Element class]
     *	    - Création des variables min and max random spawn et de la liste spawning Informations
     *	
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Minimum number of additional random enemies to spawn
    /// </summary>
    [SerializeField] private int minRandomSpawn = 0;
    /// <summary>
    /// Maximal number of additional random enemies to spawn
    /// </summary>
    [SerializeField] private int maxRandomSpawn = 1; 

    /// <summary>
    /// List of enemies and number of enemies linked to this SpawnPoint
    /// </summary>
    [SerializeField] private List<TDS_SpawningInformations> spawningInformations = new List<TDS_SpawningInformations>();
    public List<TDS_SpawningInformations> SpawningInformations { get { return spawningInformations; } }

    /// <summary>
    /// List of enemies and number of enemies linked to this SpawnPoint
    /// </summary>
    [SerializeField] private List<TDS_RandomSpawningInformations> randomSpawningInformations = new List<TDS_RandomSpawningInformations>();
    public List<TDS_RandomSpawningInformations> RandomSpawningInformations { get { return randomSpawningInformations; } }
    #endregion

    #region Methods

    #region Original Methods
    #endregion

    #endregion
}