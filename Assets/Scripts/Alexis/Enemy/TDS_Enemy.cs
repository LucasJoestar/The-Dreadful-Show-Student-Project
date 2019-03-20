using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Photon;

[RequireComponent(typeof(CustomNavMeshAgent))]
public abstract class TDS_Enemy : TDS_Character
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
     *	Date :          [13/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Adding the property Area]
     *	    - Area is the TDS_SpawnerArea from which the enemy comes
     *	    
     *	-----------------------------------
     *	
	 *	Date :          [13/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Implement the Method Increase Speed with the customNavMeshAgent]
     *	    - This Method increase the speed of the agent until it reachs its maximum speed. The it set the agent speed to the current speed
     *	
     *	-----------------------------------
     *	
     *	Date :          [06/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Setting the ApplyRecoveryTime Method]
     *	    - This Method is called after getting hit to wait a certain amount of time before calling again the Behaviour Method
     *	
     *	-----------------------------------
     *	
     *	Date :          [06/02/2019]
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
    /// Recoil Distance: When the enemy is hit, he is pushed in a direction with a distance equal of the recoilDistance
    /// </summary>
    [SerializeField] protected float recoilDistance = .1f;

    /// <summary>
    /// Return the name of the enemy
    /// </summary>
    public string EnemyName { get { return gameObject.name; } }

    /// <summary>
    /// The target of the enemy
    /// </summary>
    protected TDS_Player playerTarget = null;

    /// <summary>
    /// Spawner Area from which the enemy comes
    /// </summary>
    public TDS_SpawnerArea Area { get; set; }
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

    #region bool 
    /// <summary>
    /// Return if any attack of the enemy can be casted 
    /// </summary>
    /// <param name="_distance"></param>
    /// <returns></returns>
    protected abstract bool AttackCanBeCasted(float _distance); 

    /// <summary>
    /// Check if the enemy is in the right orientation to face the target
    /// </summary>
    /// <returns>true if the enemy has to flip to face the target</returns>
    protected bool CheckOrientation()
    {
        if (!playerTarget) return false;
        Vector3 _dir = playerTarget.transform.position - transform.position;
        float _angle = Vector3.Angle(_dir, transform.right);
        return (_angle < 90); 
    }
    #endregion 

    #region float 
    /// <summary>
    /// Method Abstract
    /// <see cref="TDS_Minion.StartAttack(float)"/>
    /// </summary>
    /// <param name="_distance"></param>
    protected abstract float StartAttack(float _distance);
    #endregion

    #region IEnumerator
    /// <summary>
    /// Wait a certain amount of seconds before starting Behaviour Method 
    /// Called after getting hit to apply a recovery time
    /// </summary>
    /// <param name="_recoveryTime">Seconds to wait</param>
    /// <returns></returns>
    protected IEnumerator ApplyRecoveryTime(float _recoveryTime)
    {
        yield return new WaitForSeconds(_recoveryTime);
        StartCoroutine(Behaviour());
        yield break;
    }

    /// <summary>
    /// /!\ THE BEHAVIOUR METHOD IS NOW ABSTRACT /!\
    /// <see cref="TDS_Minion.Behaviour"/> or <see cref="TDS_Punk.Behaviour"/>
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Behaviour()
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
                if (agent.IsMoving)
                {
                    agent.StopAgent();
                    speedCurrent = 0; 
                    SetAnimationState((int)EnemyAnimationState.Idle);
                }
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
                if (AttackCanBeCasted(_distance))
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
                SetAnimationState((int)EnemyAnimationState.Run);
                // Wait some time before calling again Behaviour(); 
                while (agent.IsMoving)
                {
                    if (isFacingRight && agent.Velocity.x > 0 || !isFacingRight && agent.Velocity.x < 0)
                        Flip();
                    _distance = Vector3.Distance(transform.position, playerTarget.transform.position);
                    if (AttackCanBeCasted(_distance))
                    {
                        enemyState = EnemyState.Attacking;
                        goto case EnemyState.Attacking;
                    }
                    if (Vector3.Distance(playerTarget.transform.position, agent.LastPosition) > detectionRange)
                    {
                        if (agent.CheckDestination(playerTarget.transform.position))
                        {
                            yield return new WaitForSeconds(.1f);
                            continue;
                        }
                        else
                        {
                            enemyState = EnemyState.MakingDecision;
                            goto case EnemyState.MakingDecision;
                        }
                    }
                    if (speedCurrent < speedMax)
                    {
                        IncreaseSpeed();
                        yield return new WaitForEndOfFrame();
                    }
                    else yield return new WaitForSeconds(.1f);
                }
                enemyState = EnemyState.MakingDecision;
                goto case EnemyState.MakingDecision;
            #endregion
            #region Attacking
            case EnemyState.Attacking:
                //Throw attack
                // If the agent is still moving, stop him
                if (agent.IsMoving)
                {
                    agent.StopAgent();
                    speedCurrent = 0;
                    SetAnimationState((int)EnemyAnimationState.Idle);
                    yield return new WaitForEndOfFrame();
                }
                if (CheckOrientation()) Flip(); 
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
    /// Apply the recoil force on the enemy
    /// </summary>
    /// <param name="_recoilDistance">Distance of the recoil</param>
    /// <returns></returns>
    protected IEnumerator ApplyRecoil(Vector3 _position)
    {
        Vector3 _direction = new Vector3(transform.position.x - _position.x , 0, 0).normalized; 
        Vector3 _pos = transform.position + (_direction * recoilDistance); 
        while(Vector3.Distance(transform.position, _pos) > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _pos, Time.deltaTime * 10); 
            yield return new WaitForEndOfFrame(); 
        }
    }
    #endregion

    #region TDS_Player
    /// <summary>
    /// Search the best player to target
    /// </summary>
    /// <returns>Best player to target</returns>
    protected TDS_Player SearchTarget()
    {
        TDS_Player[] _targets = Physics.OverlapSphere(transform.position, detectionRange).Where(c => c.GetComponent<TDS_Player>() != null && c.gameObject != this.gameObject).Select(d => d.GetComponent<TDS_Player>()).ToArray();
        if (_targets.Length == 0) return null;
        //Set constraints here (Distance, type, etc...)
        return _targets.Where(t => !t.IsDead).OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).FirstOrDefault();
    }
    #endregion

    #region Overridden Methods
    /// <summary>
    /// When the enemy dies, set its animation state to Death and remove it from the Area
    /// </summary>
    protected override void Die()
    {
        base.Die();
        animator.applyRootMotion = true;
        SetAnimationState((int)EnemyAnimationState.Death);
        if (Area) Area.RemoveEnemy(this);
    }

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
    /// Increase the speed and set the agent speed to the currentSpeed; 
    /// </summary>
    protected override void IncreaseSpeed()
    {
        base.IncreaseSpeed();
        agent.Speed = speedCurrent;
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
        if (_isTakingDamages)
        {
            agent.StopAgent();
            StopAllCoroutines();
            enemyState = EnemyState.MakingDecision;
            if(!isDead) SetAnimationState((int)EnemyAnimationState.Hit);
        }
        return _isTakingDamages;
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
    public override bool TakeDamage(int _damage, Vector3 _position)
    {
        bool _isTakingDamages = base.TakeDamage(_damage);
        if (_isTakingDamages)
        {
            agent.StopAgent();
            StopAllCoroutines();
            enemyState = EnemyState.MakingDecision;
            if (!isDead)
            {
                StartCoroutine(ApplyRecoil(_position)); 
                SetAnimationState((int)EnemyAnimationState.Hit);
            }
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
        SetAnimationState((int)EnemyAnimationState.Idle);
        base.StopAttack();
    }

    /// <summary>
    /// Call the base of the method flip
    /// if this client is the master, call the method online to flip the enemy in the other clients
    /// </summary>
    public override void Flip()
    {
        base.Flip();
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnimationState"), new object[] {});
    }
    #endregion

    #region Void
    protected abstract void ActivateAttack(int _animationID);

    /// <summary>
    /// Set the animation of the enemy to the animationID
    /// </summary>
    /// <param name="_animationID"></param>
    protected void SetAnimationState(int _animationID)
    {
        if (!animator) return;
        animator.SetInteger("animationState", _animationID);
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnimationState"), new object[] { (int)_animationID });
    }

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        if (!agent) agent = GetComponent<CustomNavMeshAgent>();
        agent.OnDestinationReached += () => enemyState = EnemyState.MakingDecision;
        OnDie += () => StopAllCoroutines();
        OnDie += () => agent.StopAgent();
        //agent.OnAgentStopped += () => speedCurrent = 0;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (PhotonNetwork.isMasterClient) StartCoroutine(Behaviour());
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
