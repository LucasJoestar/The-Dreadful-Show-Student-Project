using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class TDS_UnicycleSiamese : TDS_Enemy 
{
    /* TDS_UnicycleSiamse :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Behaviour of the unicycle siamese]
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
     *	Date :			[24/05/2019]
     *	Author :		[THIEBAUT Alexis]
     *
     *	Changes :
     *
     *	[Initialisation of the class]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    private bool hasReachedRightBound = false; 
    private TDS_Bounds bounds = null;
    #endregion

    #region Methods

    #region Original Methods

    #region OverridenMethods
    /// <summary>
    /// Check if the distance between the player and its target on the x axis is smaller than the minimum attack range
    /// Then check if the distance between the player and its target on the z axis is smaller than the agent's radius  
    /// </summary>
    /// <returns></returns>
    protected override bool AttackCanBeCasted()
    {
        bool _canAttack = base.AttackCanBeCasted();
        if (_canAttack)
            _canAttack = Mathf.Abs(transform.position.z - playerTarget.transform.position.z) <= agent.Radius * 2;
        if (_canAttack)
            _canAttack = IsFacingRight ? transform.position.x > playerTarget.transform.position.x : transform.position.x < playerTarget.transform.position.x; 
        return _canAttack; 
    }

    /// <summary>
    /// Wander around the bounds, so get random position on the left and right limits 
    /// </summary>
    /// <returns></returns>
    protected override Vector3 GetAttackingPosition()
    {
        Vector3 _v = transform.position; 
        if(bounds != null)
        {
            _v.z = UnityEngine.Random.Range(bounds.ZMin, bounds.ZMax);
            _v.x = hasReachedRightBound ? bounds.XMin + agent.Radius : bounds.XMax - agent.Radius; 
        }
        return _v; 
    }

    protected override IEnumerator Behaviour()
    {
        if (!PhotonNetwork.isMasterClient) yield break;
        // If the enemy is dead or paralyzed, they can't behave
        if (isDead || IsParalyzed || IsPacific) yield break;
        // If there is no target, the agent has to get one
        switch (enemyState)
        {
            #region Searching
            case EnemyState.Searching:
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
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
                break;
            #endregion
            #region Throwing Object
            case EnemyState.ThrowingObject:
                break;
            #endregion
            default:
                break;
        }
        additionalCoroutine = null;
        //yield return new WaitForSeconds(.1f);
        behaviourCoroutine = StartCoroutine(Behaviour());
        yield break;
    }

    protected override IEnumerator CastAttack()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break;
        if (IsPacific)
        {
            yield break;
        }
        //Throw attack
        // If the agent is still moving, stop him
        Vector3 _targetedPosition = Vector3.zero; 
        if (agent.IsMoving)
        {
            _targetedPosition = agent.LastPosition; 
        }
        agent.StopAgent();
        speedCurrent = 0;
        //Orientate the agent
        if (CheckOrientation()) Flip();
        //Cast Attack
        float _cooldown = StartAttack();
        while (IsAttacking)
        {
            yield return new WaitForSeconds(.1f);
        }
        playerTarget = null;
        if (_targetedPosition != Vector3.zero)
        {
            agent.SetDestination(_targetedPosition);
            enemyState = EnemyState.GettingInRange;
        }
        else
        {
            enemyState = EnemyState.ComputingPath;
            yield break; 
        }
        yield return new WaitForSeconds(_cooldown);
    }

    /// <summary>
    /// Flip, Increase the speed and detect the closest player.
    /// Check if an attack can be casted on this closest player
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator CastDetection()
    {
        if (isDead) yield break;
        SetAnimationState((int)EnemyAnimationState.Run);
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
            playerTarget = SearchTarget();
            // if any attack can be casted 
            if (!playerTarget) continue; 
            if (AttackCanBeCasted())
            {
                enemyState = EnemyState.Attacking;
                yield break;
            }
        }
        enemyState = EnemyState.ComputingPath;
    }

    /// <summary>
    /// Compute the path to the bounds
    /// </summary>
    protected override void ComputePath()
    {
        if (isDead || !PhotonNetwork.isMasterClient) return;
        bool _pathComputed = false;
        Vector3 _targetedPosition = GetAttackingPosition();
        if (agent.IsMoving) agent.StopAgent(); 
        _pathComputed = agent.CheckDestination(_targetedPosition);
        //If the path is computed, reach the end of the path
        if (_pathComputed)
        {
            enemyState = EnemyState.GettingInRange;
            hasReachedRightBound = !hasReachedRightBound;
        }
        else
        {
            enemyState = EnemyState.MakingDecision;
        }
    }

    /// <summary>
    /// Take a Decision
    /// </summary>
    protected override void TakeDecision()
    {
        if (IsDead || !PhotonNetwork.isMasterClient) return;
        if (isAttacking || hitBox.IsActive) StopAttack();
        // If the target can't be targeted, search for another target
        enemyState = EnemyState.ComputingPath;
    }

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        canThrow = false; 
    }

    // Use this for initialization
    protected override void Start()
    {
        bounds = TDS_Camera.Instance?.CurrentBounds;
        hasReachedRightBound = Mathf.Abs(transform.position.x - bounds.XMin) >= Mathf.Abs(transform.position.x - bounds.XMax) ? true : false;
        base.Start();
        behaviourCoroutine = StartCoroutine(Behaviour()); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
