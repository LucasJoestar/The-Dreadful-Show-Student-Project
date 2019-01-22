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
    protected TDS_Damageable damageableTarget; 

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
    /// <summary>
    /// /!\ The behaviour includes only the Detection, Mouvement and Attacking Sequences
    ///  >>> Still has to implement Grab and throw objects + Interactions with other enemies
    /// </summary>
    /// <returns></returns>
    IEnumerator Behaviour()
    {
        // If the enemy is dead or paralyzed, they can't behave
        if (isDead || IsParalyzed) yield break;
        if (!damageableTarget || damageableTarget.IsDead)
            enemyState = EnemyState.Searching; 
        float _distance = 0;
        switch (enemyState)
        {
            case EnemyState.Searching:
                // If there is no target, search a new target
                Debug.Log("Search");
                //DETECTION
                damageableTarget = SearchDamageable();
                yield return new WaitForEndOfFrame();
                //If a target is found -> Set the state to TakingDecision
                if (damageableTarget)
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
            case EnemyState.MakingDecision:
                //Take decisions
                // If the target can't be targeted, search for another target
                if (!damageableTarget || damageableTarget.IsDead)
                {
                    enemyState = EnemyState.Searching;
                    goto case EnemyState.Searching; 
                }
                _distance = _distance = Vector3.Distance(transform.position, damageableTarget.transform.position);
                yield return new WaitForEndOfFrame(); 
                //If there is an attack that can be cast, go to attack case
                if (/*attacks.Any(a => _distance < a.range)*/ _distance < 2)
                {
                    enemyState = EnemyState.Attacking;
                    goto case EnemyState.Attacking; 
                }
                //else try to reach the target
                else
                {
                    enemyState = EnemyState.ComputingPath;
                    goto case EnemyState.ComputingPath; 
                }
            case EnemyState.ComputingPath:
                //Compute the path
                if (agent.CheckDestination(damageableTarget.transform.position))
                {
                    Debug.Log("Move To");
                    enemyState = EnemyState.GettingInRange;
                    goto case EnemyState.GettingInRange; 
                }
                else //If can't be reached, search another target
                {
                    enemyState = EnemyState.Searching;
                }
                break;
            case EnemyState.GettingInRange:
                // Wait some time before calling again Behaviour(); 
                yield return new WaitForSeconds(.1f);
                _distance = Vector3.Distance(agent.LastPosition, damageableTarget.transform.position);
                if (_distance > .5f)
                {
                    enemyState = EnemyState.ComputingPath; 
                    goto case EnemyState.ComputingPath;
                }
                break;
            case EnemyState.Attacking:
                //Throw attack
                //Select the best attack to cast
                Debug.Log("Attack");
                yield return new WaitForSeconds(1);
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision; 
            case EnemyState.HoldingObject:
                //Throw the object
                break;
            default:
                break;
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(Behaviour());
        yield break; 
    }

    TDS_Damageable SearchDamageable()
    {
        TDS_Damageable[] _damageables = Physics.OverlapSphere(transform.position, detectionRange).Where(c => c.GetComponent<TDS_Damageable>() != null && c.gameObject != this.gameObject).Select(d => d.GetComponent<TDS_Damageable>()).ToArray();
        if (_damageables.Length == 0) return null; 
        return _damageables.OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).FirstOrDefault(); 
    }
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
