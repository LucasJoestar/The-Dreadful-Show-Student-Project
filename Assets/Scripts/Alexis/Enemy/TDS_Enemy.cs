using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Enemy : TDS_Character 
{
    /* TDS_Enemy :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Class to inherit from for all kind of enemies ( Minion and Boss)
     *	
     *	- Contains the common parts between Minions and bosses
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Creation of the enemy class]
	 *
     *  - Adding a custom NavMesh agent, EnemyState, bool canBeDown and canThrow, detection Range and EnemyName
     * 
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Variables
    /// <summary>
    /// Bool that allows the enemy to be take down  
    /// </summary>
    [SerializeField] protected bool canBeDown = true;

    /// <summary>
    /// Bool that allows the enemy to throw throwable objects
    /// </summary>
    [SerializeField] protected bool canThrow = true;

    /// <summary>
    /// State of the enemy 
    /// Check this state to know what to do
    /// </summary>
    [SerializeField] protected EnemyState enemyState = EnemyState.Searching;

    /// <summary>
    /// Detection Range of the enemy 
    /// If a player is within this range, it can be detected
    /// </summary>
    [SerializeField] protected float detectionRange = 2;

    /// <summary>
    /// Return the name of the enemy
    /// </summary>
    public string EnemyName { get { return gameObject.name; } }

    /* THINGS TO ADD IN THE FUTURE
     * --> Add a spawner Owner 
     * --> Add a Damageable Target
     */
    #endregion

    #region Components and References
    /// <summary>
    /// CustomNavMeshAgent of the enemy
    /// Used when the enemy has to move
    /// </summary>
    [SerializeField] protected CustomNavMeshAgent agent;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake(); 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); 
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
