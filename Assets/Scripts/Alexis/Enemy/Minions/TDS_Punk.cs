using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random;

public class TDS_Punk : TDS_Enemy 
{
    /* TDS_Punk :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Specific Behaviour of the Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[13/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Setting the Punk class as a inherited class from TDS_Enemy]
     *	    - Implementing the classes Behaviour and StartAttack
     *	    - Creating a array of enemy attack to stock all the punk attacks
     *	    - Creating a Method to get the best attack to cast for a given distance
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the Punk Class]
     *	    
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    /// <summary>
    /// Array of attacks that can be called
    /// </summary>
    [SerializeField] private TDS_EnemyAttack[] attacks;

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Select the attack to cast
    /// If there is no attack return null
    /// Selection is based on the Range and on the Probability of an attack
    /// </summary>
    /// <param name="_distance">Distance between the agent and its target</param>
    /// <returns>Attack to cast</returns>
    private TDS_EnemyAttack GetAttack(float _distance)
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

    #region Overriden Methods
    protected override IEnumerator Behaviour()
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
                if (attacks.Any(a => _distance <= a.PredictedRange))
                {
                    enemyState = EnemyState.Attacking;
                    goto case EnemyState.Attacking;
                }
                else if (Throwable /*and target can be touched by thrown object*/)
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
                _distance = Vector3.Distance(transform.position, agent.LastPosition);
                while (!attacks.Any(a => _distance < a.PredictedRange) /*|| check if the throw distance*/)
                {
                    _distance = Vector3.Distance(transform.position, agent.LastPosition);
                    if (isFacingRight && agent.Velocity.x > 0 || !isFacingRight && agent.Velocity.x < 0)
                        Flip();
                    if (speedCurrent < speedMax)
                    {
                        IncreaseSpeed();
                    }
                    if (Vector3.Distance(playerTarget.transform.position, agent.LastPosition) > 1)
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
                    //Cast Attack
                    float _cooldown = StartAttack(_distance);
                    while (IsAttacking)
                    {
                        yield return new WaitForSeconds(.1f);
                    }
                    yield return new WaitForSeconds(_cooldown);
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

    /// <summary>
    /// Cast an attack: Add a use to the attack and activate the enemy hitbox with this attack
    /// Set the animation to the animation state linked to the AnimationID of the attack 
    /// Reset to 0 consecutive uses of the other attacks
    /// Return the cooldown of the attack if it can find one
    /// </summary>
    /// <param name="_attack">Attack to cast</param>
    /// <returns>cooldown of the attack</returns>
    protected override float StartAttack(float _distance)
    {
        TDS_EnemyAttack _attack = GetAttack(_distance);
        if (_attack == null)
        {
            return 0;
        }
        IsAttacking = true;
        _attack.ConsecutiveUses++;
        attacks.ToList().Where(a => a != _attack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState((EnemyAnimationState)_attack.AnimationID);
        hitBox.Activate(_attack);
        return _attack.Cooldown;
    }
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
