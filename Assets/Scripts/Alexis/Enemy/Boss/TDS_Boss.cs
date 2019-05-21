using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public abstract class TDS_Boss : TDS_Enemy, TDS_ISpecialAttacker
{
    /* TDS_Boss :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	[General Behaviour of a boss]
 *
 *	#####################
 *	####### TO DO #######
 *	#####################
 *
 *	[TO DO]
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *
 *	Date :			[13/05/2019]
 *	Author :		[Thiebaut Alexis]
 *
 *	Changes :
 *
 *	[Initialisation of the Boss Class]
 *	    - Implementing the Interface TDS_ISpecialAttack
 *	    - Implementing the Attacks array
 *	    - Overriding the abstract and the virtual methods
 *	        GetAttack, ApplyAttackEffect, CastFirstEffect, CastSecondEffect, CastThirdEffect
 *	        
 *
 *	-----------------------------------
*/

    #region Events

    #endregion

    #region Fields / Properties
    protected TDS_EffectiveEnemyAttack castedAttack = null;

    [SerializeField] protected TDS_EffectiveEnemyAttack[] attacks; 
    public TDS_EffectiveEnemyAttack[] Attacks
    {
        get
        {
            return attacks;
        }
    }
    #endregion

    #region Methods

    #region Original Methods

    #region Overidden Methods

    #region Interface
    /// <summary>
    /// Get the next attack to cast independently of the distance
    /// </summary>
    /// <returns></returns>
    public TDS_EffectiveEnemyAttack GetAttack(float _distance = 0)
    {
        if (Attacks.Length == 0) return null;
        // Set a random to compare with the probabilities of the attackes
        float _random = UnityEngine.Random.Range(0, Attacks.Max(a => a.Probability));
        // If a probability is less than the random, this attack can be selected
        TDS_EffectiveEnemyAttack[] _availableAttacks = Attacks.Where(a => a.Probability >= _random).ToArray();
        // If there is no attack, return null
        if (_availableAttacks.Length == 0) return null;
        // Get a random Index to cast a random attack
        int _randomIndex = UnityEngine.Random.Range(0, _availableAttacks.Length);
        return _availableAttacks[_randomIndex];
    }

    /// <summary>
    /// Call the cast of the type of the Attack
    /// </summary>
    /// <param name="_type"></param>
    public void ApplyAttackEffect(EnemyEffectiveAttackType _type)
    {
        if (!PhotonNetwork.isMasterClient) return;
        switch (_type)
        {
            case EnemyEffectiveAttackType.TypeOne:
                CastFirstEffect();
                break;
            case EnemyEffectiveAttackType.TypeTwo:
                CastSecondEffect(); 
                break;
            case EnemyEffectiveAttackType.TypeThree:
                CastThirdEffect(); 
                break;
            case EnemyEffectiveAttackType.TypeSpecial:
                CastSpecialEffect(); 
                break;
            default:
                break;
        }
    }

    public abstract void CastFirstEffect();

    public abstract void CastSecondEffect();

    public abstract void CastThirdEffect();

    public abstract void CastSpecialEffect();
    
    #endregion

    /// <summary>
    /// Check if the casted attack can be casted 
    /// </summary>
    /// <param name="_distance">Distance between the enemy and its target</param>
    /// <returns></returns>
    protected override bool AttackCanBeCasted()
    {
        if (castedAttack == null) return false;
        if (Mathf.Abs(transform.position.z - playerTarget.transform.position.z) >= .6f)
        {
            return false;
        }
        return castedAttack.PredictedRange >= Mathf.Abs(transform.position.x - playerTarget.transform.position.x); 
    }

    /// <summary>
    /// Get the maximal range of the enemy's attacks
    /// </summary>
    /// <returns></returns>
    protected override float GetMaxRange()
    {
        return Attacks.Max(a => a.PredictedRange);
    }

    /// <summary>
    /// Get the Minimal Range of the enemy's attacks
    /// </summary>
    /// <returns></returns>
    protected override float GetMinRange()
    {
        return Attacks.Min(a => a.PredictedRange);
    }

    /// <summary>
    /// Attack and increase the uses of the current attack
    /// Set the animation and return the cooldown of the attack
    /// </summary>
    /// <param name="_distance"></param>
    /// <returns>The Cooldown of the casted attack</returns>
    protected override float StartAttack(float _distance = 0)
    {
        if (castedAttack == null) return 0;
        IsAttacking = true;
        castedAttack.ConsecutiveUses++;
        Attacks.ToList().Where(a => a != castedAttack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState(castedAttack.AnimationID);
        ApplyAttackEffect(castedAttack.AttackType); 
        return castedAttack.Cooldown;
    }

    /// <summary>
    /// General Behaviour of a Boss
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Behaviour()
    {
        if (!PhotonNetwork.isMasterClient) yield break; 
        // If the enemy is dead or paralyzed, they can't behave
        if (isDead || IsParalyzed || IsPacific) yield break;
        // If there is no target, the agent has to get one
        if (!playerTarget || playerTarget.IsDead)
            enemyState = EnemyState.Searching;
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
                //ELSE -> Set the state to Search
                else
                {
                    enemyState = EnemyState.Searching;
                    yield return new WaitForSeconds(1);
                    break;
                }
            #endregion
            #region Making Decision
            case EnemyState.MakingDecision:
                TakeDecision();
                break;
            #endregion
            #region Computing Path
            case EnemyState.ComputingPath:
                ComputePath();
                break;
            #endregion
            #region Getting In Range
            case EnemyState.GettingInRange:
                yield return StartCoroutine(CastDetection());
                break;
            #endregion
            #region Attacking
            case EnemyState.Attacking:
                yield return StartCoroutine(CastAttack());
                break;
            #endregion
            #region Grabbing Object
            case EnemyState.PickingUpObject:
                enemyState = EnemyState.MakingDecision; 
                break;
            #endregion
            #region Throwing Object
            case EnemyState.ThrowingObject:
                enemyState = EnemyState.MakingDecision;
                break;
            #endregion
            default:
                break;
        }
        yield return new WaitForSeconds(.1f);
        StartCoroutine(Behaviour());
        yield break;
    }

    /// <summary>
    /// Cast an the casted Attack
    /// Stop the agent movements
    /// Set its orientation
    /// Start the attack and wait until its end, then wait for the cooldown of the attack
    /// Set the casted as null then set its state to search
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator CastAttack()
    {
        if (isDead) yield break;
        if (IsPacific)
        {
            enemyState = EnemyState.MakingDecision;
            yield break;
        }
        //Throw attack
        // If the agent is still moving, stop him
        if (agent.IsMoving)
        {
            agent.StopAgent();
            speedCurrent = 0;
        }
        SetAnimationState((int)EnemyAnimationState.Idle);
        //Orientate the agent
        if (CheckOrientation()) Flip();
        yield return new WaitForSeconds(.5f);
        //Cast Attack
        float _cooldown = StartAttack();
        while (IsAttacking)
        {
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(_cooldown);
        castedAttack = null; 
        enemyState = EnemyState.Searching;
    }

    /// <summary>
    /// Check around the boss to see if the attack can be casted with anticipation
    /// Also check if the path has to be recalculated
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator CastDetection()
    {
        if (isDead) yield break;
        SetAnimationState((int)EnemyAnimationState.Run);
        // Wait some time before calling again Behaviour(); 
        Collider[] _colliders;
        float _distance;
        while (agent.IsMoving)
        {
            //Orientate the agent
            if (isFacingRight && agent.Velocity.x > 0 || !isFacingRight && agent.Velocity.x < 0)
                Flip();

            //Increase the speed if necessary
            if (speedCurrent < speedMax)
            {
                IncreaseSpeed();
                yield return new WaitForEndOfFrame();
            }
            else yield return new WaitForSeconds(.1f);
            //Check if the area allow to grab object
            // if any attack can be casted
            //Check if a player is close enough to cast the attack on the player (not the targeted one but the closer one)
            _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
            _colliders = Physics.OverlapSphere(transform.position, detectionRange);
            if (_colliders.Length > 0)
            {
                _colliders = _colliders.Where(t => t.gameObject.HasTag("Player")).OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
                if (_colliders.Length > 0)
                {
                    for (int i = 0; i < _colliders.Length; i++)
                    {
                        if(transform.position.z - _colliders[i].transform.position.z <= .5f)
                        {
                            _distance = Vector3.Distance(transform.position, _colliders[i].transform.position);
                            break; 
                        }
                    }
                }
            }
            if (AttackCanBeCasted())
            {
                enemyState = EnemyState.Attacking;
                yield break;
            }
            //if the target is too far from the destination, recalculate the path
            if (Vector3.Distance(agent.LastPosition, playerTarget.transform.position) > castedAttack.PredictedRange)
            {
                yield return new WaitForSeconds(.1f);
                enemyState = EnemyState.ComputingPath;
                yield break;
            }
        }
        enemyState = EnemyState.MakingDecision;
    }

    /// <summary>
    /// Activate the enemy
    /// </summary>
    public override void ActivateEnemy()
    {
        if (!PhotonNetwork.isMasterClient) return;
        IsPacific = false;
        IsParalyzed = false;
        StartCoroutine(Behaviour()); 
    }

    /// <summary>
    /// USED IN ANIMATION
    /// Activate the hitbox and Apply the effect of the attack
    /// </summary>
    /// <param name="_animationID"></param>
    protected override void ActivateAttack(int _animationID)
    {
        if (!PhotonNetwork.isMasterClient || castedAttack == null) return;
        hitBox.Activate(castedAttack);
        //ApplyAttackEffect(castedAttack.AttackType);
    }

    /// <summary>
    /// Compute the path to the atacking Position
    /// </summary>
    protected override void ComputePath()
    {
        if (isDead) return;
        if (IsParalyzed)
        {
            enemyState = EnemyState.MakingDecision;
            return;
        }
        //Compute the path
        // Select a position
        bool _pathComputed = false;
        Vector3 _position = GetAttackingPosition();
        // Debug.Log(_position); 
        _pathComputed = agent.CheckDestination(_position);
        //If the path is computed, reach the end of the path
        if (_pathComputed)
        {
            enemyState = EnemyState.GettingInRange;
        }
        else
        {
            SetAnimationState((int)EnemyAnimationState.Idle);
            enemyState = EnemyState.MakingDecision;
        }
    }

    /// <summary>
    /// Called when the boss dies
    /// </summary>
    protected override void Die()
    {
        base.Die();
    }

    /// <summary>
    /// Get the casted attack and check if it can be casted, if so, cast it
    /// Else compute path until reaching a attacking position
    /// </summary>
    protected override void TakeDecision()
    {
        castedAttack = GetAttack();
        float _distance = Vector3.Distance(transform.position, playerTarget.transform.position); 
        if(AttackCanBeCasted())
        {
            enemyState = EnemyState.Attacking; 
        }
        else
        {
            enemyState = EnemyState.ComputingPath; 
        }
    }

    /// <summary>
    /// Get a random player to target
    /// </summary>
    /// <returns>Return a random player within the detection Range</returns>
    protected override TDS_Player SearchTarget()
    {
        TDS_Player[] _targets = Physics.OverlapSphere(transform.position, detectionRange).Where(d => d.gameObject.HasTag("Player")).Select(t => t.GetComponent<TDS_Player>()).ToArray();
        if (_targets.Length == 0) return null;
        _targets = _targets.Where(t => !t.IsDead).ToArray();
        if (_targets.Length == 0) return null;
        int _randomIndex = UnityEngine.Random.Range(0, _targets.Length - 1);
        return _targets[_randomIndex];
    }

    /// <summary>
    /// Get an attacking position for the casted attack
    /// </summary>
    /// <returns>Return an attacking position</returns>
    protected override Vector3 GetAttackingPosition()
    {
        Vector3 _attackingPosition = transform.position;
        if (playerTarget)
        {
            int _coeff = playerTarget.transform.position.x > transform.position.x ? -1 : 1;
            _attackingPosition.x = Mathf.Abs(transform.position.x - playerTarget.transform.position.x) < castedAttack.PredictedRange ? transform.position.x : playerTarget.transform.position.x + (castedAttack.PredictedRange * _coeff); 
            _attackingPosition.z = playerTarget.transform.position.z + UnityEngine.Random.Range(-.4f, .4f);
        }
        return _attackingPosition;
    }
    #endregion

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
