using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public abstract class TDS_Boss : TDS_Enemy
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
 *  Date :          [22/05/2019]
 *	Author:         [THIEBAUT Alexis]
 *	
 *	[Refactoring of the class and its inheritances]
 *	    - Removing all methods that can be virtual in the TDS_EnemyClass
 *	    - Implementing all methods that had to be overriden
 *	    - Removing the Interface
 *	    
 *	-----------------------------------
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
    [SerializeField] protected int damagesThreshold = 10; 
    protected TDS_EnemyAttack castedAttack = null;

    #endregion

    #region Methods

    #region Original Methods

    #region Overidden Methods
    /// <summary>
    /// Get the next attack to cast independently of the distance
    /// </summary>
    /// <returns></returns>
    protected override TDS_EnemyAttack GetAttack()
    {
        if (attacks.Length == 0) return null;
        TDS_EnemyAttack[] _availableAttacks = attacks.Where(a => a != null).ToArray();

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

    /// <summary>
    /// Check if the casted attack can be casted 
    /// </summary>
    /// <param name="_distance">Distance between the enemy and its target</param>
    /// <returns></returns>
    protected override bool AttackCanBeCasted()
    {
        if (!PhotonNetwork.isMasterClient || castedAttack == null) return false;
        if (castedAttack.GetType() == typeof(TDS_SpinningAttackBehaviour)) return true; 
        if (Mathf.Abs(transform.position.z - playerTarget.transform.position.z) >= .25f)
        {
            //Debug.Log(Mathf.Abs(transform.position.z - playerTarget.transform.position.z)); 
            return false;
        }
        float _distance = Mathf.Abs(transform.position.x - playerTarget.transform.position.x); 
        return castedAttack.MaxRange > _distance && _distance > castedAttack.MinRange; 
    }

    /// <summary>
    /// Cast the casted attack: Add a use to the attack
    /// Set the animation to the animation state linked to the AnimationID of the attack 
    /// Reset to 0 consecutive uses of the other attacks
    /// Return the cooldown of the attack if it can find one
    /// </summary>
    /// <returns>cooldown of the attack</returns>
    protected override float StartAttack()
    {
        if(castedAttack == null)
        {
            return 0; 
        }
        isAttacking = true;
        castedAttack.ConsecutiveUses++;
        attacks.ToList().Where(a => a != castedAttack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState(castedAttack.AnimationID);
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
                SetAnimationState((int)EnemyAnimationState.Idle); 
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
                additionalCoroutine = StartCoroutine(CastDetection());
                yield return additionalCoroutine; 
                break;
            #endregion
            #region Attacking
            case EnemyState.Attacking:
                additionalCoroutine = StartCoroutine(CastAttack());
                yield return additionalCoroutine;
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
        additionalCoroutine = null; 
        yield return new WaitForSeconds(.1f);
        behaviourCoroutine = StartCoroutine(Behaviour());
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
        yield return base.CastAttack();
        castedAttack = null;
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
            if (isFacingRight && agent.Velocity.x < 0 || !isFacingRight && agent.Velocity.x > 0)
                Flip();

            //Increase the speed if necessary
            if (speedCurrent < speedMax)
            {
                IncreaseSpeed();
                yield return null;
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
            if (Mathf.Abs(agent.LastPosition.z - playerTarget.transform.position.z) > .6f ||  Mathf.Abs(transform.position.x - playerTarget.transform.position.x) > castedAttack.MaxRange)
            {
                yield return new WaitForSeconds(.1f);
                enemyState = EnemyState.ComputingPath;
                yield break;
            }
        } 
        enemyState = EnemyState.Attacking;
    }

    /// <summary>
    /// Called when the enemy takes damages greater than his damages threshold
    /// Stop the agent
    /// Stop all the current Coroutines
    /// set its state to making decisions
    /// Apply the recoil and set the animation to hit if he's not dead
    /// </summary>
    /// <param name="_damage">Dealt damages</param>
    /// <param name="_position">Position of the attacker</param>
    protected override void ApplyDamagesBehaviour(int _damage, Vector3 _position)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        if (!isDead && _damage >= damagesThreshold)
        {
            SetAnimationState((int)EnemyAnimationState.Hit);
            agent.StopAgent();
            hitBox.Desactivate();
            StartCoroutine(ApplyRecoil(_position));
            enemyState = EnemyState.MakingDecision;
        }
        else if (isDead)
        {
            agent.StopAgent();
            hitBox.Desactivate();
        }
        else
        {
            SetAnimationState((int)EnemyAnimationState.LightHit);
        }
    }

    /// <summary>
    /// Compute the path to the atacking Position
    /// </summary>
    protected override void ComputePath()
    {
        if (!PhotonNetwork.isMasterClient || isDead) return;
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
    /// Initialise the boss's lifebar
    /// </summary>
    protected override void InitLifeBar()
    {
        if (TDS_UIManager.Instance?.CanvasScreen)
        {
            TDS_UIManager.Instance.SetBossLifeBar(this); 
        }
    }

    /// <summary>
    /// Get the casted attack and check if it can be casted, if so, cast it
    /// Else compute path until reaching a attacking position
    /// </summary>
    protected override void TakeDecision()
    {
        if (!PhotonNetwork.isMasterClient) return; 
        if(!castedAttack) castedAttack = GetAttack();
        base.TakeDecision(); 
    }

    /// <summary>
    /// Get a random player to target
    /// </summary>
    /// <returns>Return a random player within the detection Range</returns>
    protected override TDS_Player SearchTarget()
    {
        TDS_Player[] _targets = null;
        if (!TDS_LevelManager.Instance)
            _targets = Physics.OverlapSphere(transform.position, detectionRange).Where(d => d.gameObject.HasTag("Player")).Select(t => t.GetComponent<TDS_Player>()).ToArray();
        else
            _targets = TDS_LevelManager.Instance.AllPlayers.Where(p => Vector3.Distance(p.transform.position, transform.position) <= detectionRange).ToArray(); if (_targets.Length == 0) return null;
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
        //return playerTarget.transform.position; 
        Vector3 _attackingPosition = transform.position;
        if (playerTarget)
        {
            int _coeff = playerTarget.transform.position.x > transform.position.x ? -1 : 1;
            float _distance = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
            if(_distance < castedAttack.MinRange)
            {
                castedAttack = GetAttack(); 
            }
            _attackingPosition.x = _distance < castedAttack.MaxRange ? transform.position.x : playerTarget.transform.position.x + ((castedAttack.MaxRange - agent.Radius) * _coeff);
            _attackingPosition.z = playerTarget.transform.position.z; 
        }
        return _attackingPosition;
    }
    #endregion

    #endregion

    #region Unity Methods
    #endregion

    #endregion
}
