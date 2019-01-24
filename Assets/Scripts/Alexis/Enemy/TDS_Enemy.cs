using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[RequireComponent(typeof(CustomNavMeshAgent))]
public class TDS_Enemy : TDS_Character 
{
    /* TDS_Enemy :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Class to inherit from for all kind of enemies ( Minion and Boss).
     *	
     *	- Contains the common parts between Minions and bosses.
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
     *  - Adding a custom NavMesh agent, EnemyState, bool canBeDown and canThrow, detection Range and EnemyName.
     * 
	 *	----------------------------------- 
     *	
     *	Date :          [24/01/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Implementation of the Behaviour Method]
     *	
     *	- Adding the Method Behaviour 
     *	- This Method change the enemy state and call the necessary methods to make the agent act
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
    [SerializeField] protected float detectionRange = 5;

    /// <summary>
    /// Return the name of the enemy
    /// </summary>
    public string EnemyName { get { return gameObject.name; } }

    /// <summary>
    /// The target of the enemy
    /// </summary>
    protected TDS_Player playerTarget = null; 

    /* THINGS TO ADD IN THE FUTURE
     * --> Add a spawner Owner 
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

    #region IEnumerator
    /// <summary>
    /// /!\ The behaviour includes only the Detection, Mouvement and Attacking Sequences
    ///  >>> Still has to implement Grab and throw objects + Interactions with other enemies
    /// </summary>
    /// <returns></returns>
    IEnumerator Behaviour()
    {
        // If the enemy is dead or paralyzed, they can't behave
        if (isDead || IsParalyzed) yield break;
        if (!playerTarget || playerTarget.IsDead)
            enemyState = EnemyState.Searching; 
        float _distance = 0;
        switch (enemyState)
        {
            #region Searching
            case EnemyState.Searching:
                // If there is no target, search a new target
                Debug.Log("Search");
                //DETECTION
                playerTarget = SearchTarget();
                yield return new WaitForEndOfFrame();
                //If a target is found -> Set the state to TakingDecision
                if (playerTarget)
                {
                    Debug.Log("Target found");
                    enemyState = EnemyState.MakingDecision;
                    yield return new WaitForEndOfFrame();
                    goto case EnemyState.MakingDecision; 
                }
                //ELSE BREAK -> Set the state to Search
                else
                {
                    Debug.Log("Target Not found");
                    enemyState = EnemyState.Searching;
                    break;
                }
            #endregion
            #region Making Decision
            case EnemyState.MakingDecision:
                //Take decisions
                // If the target can't be targeted, search for another target
                if (!playerTarget || playerTarget.IsDead)
                {
                    enemyState = EnemyState.Searching;
                    goto case EnemyState.Searching; 
                }
                _distance = _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                yield return new WaitForEndOfFrame();
                //If there is an attack that can be cast, go to attack case
                //Check if the agent can grab an object, 
                //if so goto case GrabObject if it can be grab 
                // if it can't be grabbed directly, getting in range
                if (/*attacks.Any(a => _distance < a.range)*/ _distance < 2)
                {
                    enemyState = EnemyState.Attacking;
                    goto case EnemyState.Attacking; 
                }
                else if(Throwable /*and target can be touched by thrown object*/)
                {
                    enemyState = EnemyState.ThrowingObject;
                    goto case EnemyState.ThrowingObject; 
                }
                //else try to reach the target
                else
                {
                    enemyState = EnemyState.ComputingPath;
                    goto case EnemyState.ComputingPath; 
                }
            #endregion
            #region Computing Path
            case EnemyState.ComputingPath:
                //Compute the path
                // If there is something to throw, Move until reaching a position from where the player can be touched
                // Be careful, the agent don't have to recalculate path when they have a Throwable
                if(Throwable /*&& Check if the position can be reached*/ )
                {
                    // enemyState = EnemyState.GettingInRange;
                    // goto case EnemyState.GettingInRange;
                }
                if (agent.CheckDestination(playerTarget.transform.position))
                {
                    enemyState = EnemyState.GettingInRange;
                    goto case EnemyState.GettingInRange; 
                }
                else //If can't be reached, search another target
                {
                    enemyState = EnemyState.Searching;
                }
                break;
            #endregion
            #region Getting In Range
            case EnemyState.GettingInRange:
                // Wait some time before calling again Behaviour(); 
                yield return new WaitForSeconds(.1f);
                _distance = Vector3.Distance(agent.LastPosition, playerTarget.transform.position);
                if (_distance > .5f)
                {
                    enemyState = EnemyState.ComputingPath; 
                    goto case EnemyState.ComputingPath;
                }
                break;
            #endregion
            #region Attacking
            case EnemyState.Attacking:
                //Throw attack
                //Select the best attack to cast
                if(Throwable)
                {
                    enemyState = EnemyState.ThrowingObject;
                    goto case EnemyState.ThrowingObject;
                }
                Debug.Log("Attack");
                while(IsAttacking)
                {
                    Debug.Log("Wait end of attack"); 
                    yield return new WaitForEndOfFrame();
                }
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
            #endregion
            #region Grabbing Object
            case EnemyState.GrabingObject:
                //Grab an object
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
            #endregion
            #region Throwing Object
            case EnemyState.ThrowingObject:
                //Throw the held object
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
            #endregion
            default:
                break;
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(Behaviour());
        yield break; 
    }
    #endregion

    #region TDS_Player
    /// <summary>
    /// Search the best player to target
    /// </summary>
    /// <returns>Best player to target</returns>
    TDS_Player SearchTarget()
    {
        TDS_Player[] _targets = Physics.OverlapSphere(transform.position, detectionRange).Where(c => c.GetComponent<TDS_Player>() != null && c.gameObject != this.gameObject).Select(d => d.GetComponent<TDS_Player>()).ToArray();
        if (_targets.Length == 0) return null; 
        //Set constraints here (Distance, type, etc...)
        return _targets.OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).FirstOrDefault(); 
    }
    #endregion

    #region void
    /// <summary>
    /// Overridden Grab Object Method
    /// </summary>
    /// <param name="_throwable"></param>
    /// <returns></returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        return base.GrabObject(_throwable);
        // Does the agent has a different behaviour from the players? 
    }

    /// <summary>
    /// Overridden Throw Object Method
    /// 
    /// </summary>
    /// <param name="_targetPosition"></param>
    public override void ThrowObject(Vector3 _targetPosition)
    {
        base.ThrowObject(_targetPosition);
        // Does the agent has a different behaviour from the players? 
    }
    #endregion 

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        agent.OnDestinationReached += () => enemyState = EnemyState.MakingDecision; 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Behaviour());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
