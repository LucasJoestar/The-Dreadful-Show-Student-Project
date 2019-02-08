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
     *	 Date :          [06/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Setting the attack method]
     *	
     *	- The Attack Method Activate the hit box and set the animation state of the animator to the animationId value of the attack
     *	- Reset the consecutive uses of other attacks
     *	- The Get Attack Methods search a attack that can be throw within a distance
     *	
     * 	-----------------------------------
     * 	
	 *  Date :          [05/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Implementation of the SetAnimationState Method]
     *	
     *	- The SetAnimationState Method set the int "animation State" in the enemy animator to play a linked animation
     *	
     * 	-----------------------------------
     * 	
     *	Date :          [24/01/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Implementation of the Behaviour Method]
     *	
     *	- Adding the Method Behaviour 
     *	- This Method change the enemy state and call the necessary methods to make the agent act
     *	
     *	-----------------------------------
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
	*/

    #region Events

    #endregion

    #region Fields / Properties

    /* THINGS TO ADD IN THE FUTURE
     * --> Add a spawner Owner 
     */

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

    /// <summary>
    /// Array of attacks that can be called
    /// </summary>
    [SerializeField] protected TDS_EnemyAttack[] attacks; 
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
        // If there is no target, the agent has to get one
        if (!playerTarget || playerTarget.IsDead)
            enemyState = EnemyState.Searching; 
        float _distance = 0;
        switch (enemyState)
        {
            #region Searching
            case EnemyState.Searching:
                // If there is no target, search a new target
                playerTarget = SearchTarget();
                //If a target is found -> Set the state to TakingDecision
                if (playerTarget)
                {
                    enemyState = EnemyState.MakingDecision;
                    goto case EnemyState.MakingDecision; 
                }
                //ELSE BREAK -> Set the state to Search
                else
                {
                    enemyState = EnemyState.Searching;
                    yield return new WaitForSeconds(1); 
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
                _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                /* If there is an attack that can be cast, go to attack case
                 * Check if the agent can grab an object, 
                 * if so goto case GrabObject if it can be grab 
                 * if it can't be grabbed directly, getting in range
                */
                if (attacks.Any(a => _distance < a.PredictedRange))
                {
                    yield return new WaitForSeconds(.2f); 
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

                /*
                if(Throwable && Check if the position can be reached )
                {
                    // enemyState = EnemyState.GettingInRange;
                    // goto case EnemyState.GettingInRange;
                }
                */

                if (agent.CheckDestination(playerTarget.transform.position))
                {
                    enemyState = EnemyState.GettingInRange;
                    goto case EnemyState.GettingInRange; 
                }
                yield return new WaitForSeconds(1); 
                break;
            #endregion
            #region Getting In Range
            case EnemyState.GettingInRange:
                SetAnimationState(EnemyAnimationState.Run);
                // Wait some time before calling again Behaviour(); 
                // Still has to increase speed of the agent
                _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                while(!attacks.Any(a => _distance < a.PredictedRange))
                {
                    _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                    if (isFacingRight && agent.Velocity.x > 0 || !isFacingRight && agent.Velocity.x < 0)
                        Flip(); 
                    if(Vector3.Distance(playerTarget.transform.position, agent.LastPosition) >  1)
                    {
                        if (agent.CheckDestination(playerTarget.transform.position))
                        {
                            yield return new WaitForSeconds(.1f);
                            continue;
                        }
                        else
                        {
                            agent.StopAgent(); 
                            enemyState = EnemyState.MakingDecision;
                            goto case EnemyState.MakingDecision;
                        }
                    }
                    //yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(.1f); 
                }
                agent.StopAgent();
                SetAnimationState(EnemyAnimationState.Idle); 
                break; 
            #endregion
            #region Attacking
            case EnemyState.Attacking:
                //Throw attack
                //Select the best attack to cast
                if (Throwable)
                {
                    enemyState = EnemyState.ThrowingObject;
                    goto case EnemyState.ThrowingObject;
                }
                else
                {
                    _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                    TDS_EnemyAttack _attack =  GetAttack(_distance);
                    if(_attack == null)
                    {
                        enemyState = EnemyState.MakingDecision;
                        goto case EnemyState.MakingDecision;
                    }
                    //Cast Attack
                    StartAttack(_attack); 
                    while (IsAttacking)
                    {
                        yield return new WaitForSeconds(.1f);
                    }
                    yield return new WaitForSeconds(_attack.Cooldown); 
                }
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
            #endregion
            #region Grabbing Object
            case EnemyState.PickingUpObject:
                //Pick up an object
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
        yield return new WaitForSeconds(.1f);
        StartCoroutine(Behaviour());
        yield break; 
    }
    #endregion

    #region TDS_EnemyAttack
    /// <summary>
    /// Select the attack to cast
    /// If there is no attack return null
    /// Selection is based on the Range and on the Probability of an attack
    /// </summary>
    /// <param name="_distance">Distance between the agent and its target</param>
    /// <returns>Attack to cast</returns>
    protected TDS_EnemyAttack GetAttack(float _distance)
    {
        //If the enemy has no attack, return null
        if (attacks == null || attacks.Length == 0) return null;
        // Get all attacks that can hit the target
        TDS_EnemyAttack[] _availableAttacks = attacks.Where(a => a.IsDistanceAttack || a.PredictedRange > _distance).ToArray();
        // If there is no attack in Range, return null
        if (_availableAttacks.Length == 0) return null;
        // Set a random to compare with the probabilities of the attackes
        float _random = UnityEngine.Random.Range(0, _availableAttacks.Max(a => a.Probability));
        // If a probability is less than the random, this attack can be selected
        _availableAttacks = _availableAttacks.Where(a => a.Probability >= _random).ToArray();
        // If there is no attack, return null
        if (_availableAttacks.Length == 0) return null;
        // Get a random Index to cast a random attack
        int _randomIndex = UnityEngine.Random.Range(0, _availableAttacks.Length);   
        return _availableAttacks[_randomIndex]; 
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
        return _targets.Where(t => !t.IsDead).OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).FirstOrDefault(); 
    }
    #endregion

    #region Overridden Methods
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

    /// <summary>
    /// Overriden method to take damages
    /// If the agent take damages
    /// Stop the movments of the agent
    /// Stop the Behaviour Method
    /// Set the state to making decision 
    /// Change the animation state of the agent
    /// </summary>
    /// <param name="_damage">amount of damages</param>
    /// <returns>if the agent take damages</returns>
    public override bool TakeDamage(int _damage)
    {
        bool _isTakingDamages = base.TakeDamage(_damage);
        if(_isTakingDamages)
        {
            agent.StopAgent();
            StopCoroutine(Behaviour());
            enemyState = EnemyState.MakingDecision;
            if (isDead)
                SetAnimationState(EnemyAnimationState.Death);
            else
                SetAnimationState(EnemyAnimationState.Hit);
        }
        return _isTakingDamages; 
    }

    /// <summary>
    /// Stop the current attack of the agent
    /// Desactivate the hitbox
    /// Set the bool IsAttacking to false
    /// Set the animation state to idle
    /// </summary>
    public override void StopAttack()
    {
        SetAnimationState(EnemyAnimationState.Idle); 
        base.StopAttack();
    }
    #endregion

    #region Void
    /// <summary>
    /// Set the animation of the enemy to the animationID
    /// </summary>
    /// <param name="_animationID"></param>
    protected void SetAnimationState(EnemyAnimationState _animationID)
    {
        if (!animator) return;
        animator.SetInteger("animationState", (int)_animationID); 
    }

    /// <summary>
    /// Cast an attack: Add a use to the attack and activate the enemy hitbox with this attack
    /// Set the animation to the animation state linked to the AnimationID of the attack 
    /// Reset to 0 consecutive uses of the other attacks
    /// </summary>
    /// <param name="_attack">Attack to cast</param>
    protected virtual void StartAttack(TDS_EnemyAttack _attack)
    {
        IsAttacking = true;
        _attack.ConsecutiveUses++;
        attacks.ToList().Where(a => a != _attack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState((EnemyAnimationState)_attack.AnimationID);
        hitBox.Activate(_attack); 
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        agent.OnDestinationReached += () => enemyState = EnemyState.MakingDecision;
        OnDie += () => StopAllCoroutines();
        OnDie += () => agent.StopAgent(); 
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
