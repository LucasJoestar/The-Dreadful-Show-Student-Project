using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random  = UnityEngine.Random;
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
     *  Date :          [22/05/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Refactoring of the class and its inheritances]
     *	    - Removing all methods in double in the following scripts: TDS_Minion, TDS_Punk, TDS_Boss, TDS_Siamese
     *	    - Adding the calling of the Method "ApplyBehaviour" when the attack is activated
     *	    
     *	-----------------------------------
     *	
     *	
     *	Date :          [13/02/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Modify the State Machine to get rid of the goto]
     *	    - Call methods to change the enemy state.
     *	    - Call Behaviour Recursively at the end of the method and go through another state
     *	    
     *	-----------------------------------
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
    /// Bool that allows the enemy to pick up and throw throwable objects
    /// </summary>
    [SerializeField] protected bool canThrow = true;

    /// <summary>
    /// When this bool is set to true, the enemy has to wait until it turn to false again
    /// </summary>
    private bool isWaiting = false; 

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
    /// Recoil Distance: When the enemy is hit and dies, he is pushed in a direction with a distance equal of the recoilDistance 
    /// </summary>
    [SerializeField] protected float recoilDistanceDeath = .1f;

    /// <summary>
    /// Recoil Time: When the enemy is hit, he is pushed in a direction during this time (in seconds)
    /// </summary>
    [SerializeField] protected float recoilTimeDeath = .1f;

    /// <summary>
    /// The max distance an enemy can throw an object
    /// </summary>
    [SerializeField] protected float throwRange = 1;

    /// <summary>
    /// Attacks of the enemy
    /// </summary>
    [SerializeField] protected TDS_EnemyAttack[] attacks = new TDS_EnemyAttack[] { }; 
    public TDS_EnemyAttack[] Attacks { get { return attacks;  } }

    /// <summary>
    /// Return the name of the enemy
    /// </summary>
    public string EnemyName { get { return gameObject.name; } }

    /// <summary>
    /// The target of the enemy
    /// </summary>
    protected TDS_Player playerTarget = null;

    /// <summary>
    /// Accessor of the playerTarget property
    /// </summary>
    public TDS_Player PlayerTarget { get { return playerTarget; } }

    /// <summary>
    /// Spawner Area from which the enemy comes
    /// </summary>
    public TDS_SpawnerArea Area { get; set; }

    /// <summary>
    /// Accessor of the handsTransform Property
    /// </summary>
    public Transform HandsTransform { get { return handsTransform;  } }
    #endregion

    #region Components and References
    /// <summary>
    /// CustomNavMeshAgent of the enemy
    /// Used when the enemy has to move
    /// </summary>
    [SerializeField] protected CustomNavMeshAgent agent;

    /// <summary>
    /// Accessor of the agent propertym
    /// </summary>
    public CustomNavMeshAgent Agent { get { return agent; } }

    /// <summary>
    /// Throwable to reach and grab
    /// </summary>
    private TDS_Throwable targetedThrowable = null;

    /// <summary>
    /// Behaviour Coroutine, used when the Behaviour is called and stopped
    /// </summary>
    protected Coroutine behaviourCoroutine = null;
    /// <summary>
    /// Additional Coroutine, used when the CastAttack, CastDetection, CastGrab and CastThrow are called or stopped
    /// </summary>
    protected Coroutine additionalCoroutine = null; 
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region TDS_EnemyAttack
    /// <summary>
    /// Select the attack to cast
    /// If there is no attack return null
    /// Selection is based on the Range and on the Probability of an attack
    /// </summary>
    /// <param name="_distance">Distance between the agent and its target</param>
    /// <returns>Attack to cast</returns>
    protected virtual TDS_EnemyAttack GetAttack(float _distance = 0)
    {
        //If the enemy has no attack, return null
        if (attacks == null || attacks.Length == 0) return null;
        // Get all attacks that can hit the target
        TDS_EnemyAttack[] _availableAttacks = attacks.Where(a => a.MaxRange > _distance).ToArray();
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

    #region bool 
    /// <summary>
    /// Return true if the distance is less than the minimum predicted range of the Punk attack
    /// </summary>
    /// <param name="_distance">distance between player and target</param>
    /// <returns>does the attack can be cast</returns>
    protected virtual bool AttackCanBeCasted()
    {
        return Mathf.Abs(transform.position.x - playerTarget.transform.position.x) <= attacks.Min(a => a.MaxRange);
    }

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
    /// Cast an attack: Add a use to the attack and activate the enemy hitbox with this attack
    /// Set the animation to the animation state linked to the AnimationID of the attack 
    /// Reset to 0 consecutive uses of the other attacks
    /// Return the cooldown of the attack if it can find one
    /// </summary>
    /// <param name="_attack">Attack to cast</param>
    /// <returns>cooldown of the attack</returns>
    protected virtual float StartAttack()
    {
        TDS_EnemyAttack _attack = GetAttack(Vector3.Distance(transform.position, playerTarget.transform.position));
        if (_attack == null)
        {
            return 0;
        }
        IsAttacking = true;
        _attack.ConsecutiveUses++;
        attacks.ToList().Where(a => a != _attack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState(_attack.AnimationID);
        return _attack.Cooldown;
    }

    /// <summary>
    /// Return the maximum Attack Range
    /// </summary>
    /// <returns></returns>
    protected float GetMaxRange()
    {
        return attacks.Select(a => a.MaxRange).Min();
    }

    /// <summary>
    /// Return the minimum AttackRange
    /// </summary>
    /// <returns></returns>
    protected float GetMinRange()
    {
        return attacks.Select(a => a.MaxRange).Max();
    }
    #endregion

    #region IEnumerator
    /// <summary>
    /// Apply the recoil force on the enemy
    /// </summary>
    /// <param name="_recoilDistance">Distance of the recoil</param>
    /// <returns></returns>
    protected IEnumerator ApplyRecoil(Vector3 _position)
    {
        if (!PhotonNetwork.isMasterClient) yield break ; 
        Vector3 _direction = new Vector3(transform.position.x - _position.x, 0, 0).normalized;
        Vector3 _pos = Vector3.zero; 
        if (isDead)
        {
            _pos = transform.position + (_direction * recoilDistanceDeath);
            while (Vector3.Distance(transform.position, _pos) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _pos, recoilDistanceDeath / recoilTimeDeath * Time.deltaTime);
                yield return null;
            }
            yield break;
        }
        
        while (Vector3.Distance(transform.position, _pos) > .1f)
        {
            _pos = transform.position + (_direction * recoilDistance);
            transform.position = Vector3.MoveTowards(transform.position, _pos, Time.deltaTime * 10);
            yield return null;
        }
    }

    /// <summary>
    /// Wait a certain amount of seconds before starting Behaviour Method 
    /// Called after getting hit to apply a recovery time
    /// </summary>
    /// <param name="_recoveryTime">Seconds to wait</param>
    /// <returns></returns>
    protected IEnumerator ApplyRecoveryTime(float _recoveryTime)
    {
        if (!PhotonNetwork.isMasterClient) yield break; 
        yield return new WaitForSeconds(_recoveryTime);
        behaviourCoroutine = StartCoroutine(Behaviour());
        yield break;
    }

    /// <summary>
    /// <see cref="TDS_Minion.Behaviour"/> or <see cref="TDS_Punk.Behaviour"/>
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Behaviour()
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
                additionalCoroutine = StartCoroutine(CastGrab(targetedThrowable));
                yield return additionalCoroutine; 
                targetedThrowable = null;
                break; 
            #endregion
            #region Throwing Object
            case EnemyState.ThrowingObject:
                additionalCoroutine = StartCoroutine(CastThrow());
                yield return additionalCoroutine;
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
    /// Stop the agent and orientate it
    /// Cast an attack that can touch the target
    /// Wait for the end of the attack
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator CastAttack()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        if(IsPacific)
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
        enemyState = EnemyState.MakingDecision;
    }

    /// <summary>
    /// Cast the detection during the agent movements
    /// Check if there is objects to grab or throw
    /// Check if there is player to attack  
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator CastDetection()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        SetAnimationState((int)EnemyAnimationState.Run);
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
                yield return null;
            }
            else yield return new WaitForSeconds(.1f);
            //Check if the area allow to grab object
            // If the enemy hasn't a throwable, check if he can grab one
            if (throwable == null && canThrow)
            {
                if (targetedThrowable)
                {
                    //If the targeted throwable is close enough, grab it
                    _distance = collider.size.x + targetedThrowable.transform.localScale.x;
                    if (Vector3.Distance(transform.position, targetedThrowable.transform.position) <= _distance)
                    {
                        enemyState = EnemyState.PickingUpObject;
                        yield break; 
                    }
                }
                else
                {
                    //Check if there is object around the enemy
                    _colliders = Physics.OverlapSphere(transform.position, detectionRange);
                    if (_colliders.Length > 0)
                    {
                        _colliders = _colliders.Where(c => c.GetComponent<TDS_Throwable>()).OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
                        if (_colliders.Length > 0)
                        {
                            if (Vector3.Distance(transform.position, _colliders.First().transform.position) < Vector3.Distance(transform.position, playerTarget.transform.position))
                            {
                                //Get the closest throwable
                                targetedThrowable = _colliders.Select(c => c.GetComponent<TDS_Throwable>()).First();
                                //Set a new path to the throwable 
                                enemyState = EnemyState.ComputingPath;
                                yield break; 
                            }
                        }
                    }
                }
            }
            // if any attack can be casted 
            if (AttackCanBeCasted())
            {
                enemyState = EnemyState.Attacking;
                yield break; 
            }
            //if the target is too far from the destination, recalculate the path
            if (Vector3.Distance(agent.LastPosition, playerTarget.transform.position) > GetMaxRange())
            {
                yield return new WaitForSeconds(.1f); 
                enemyState = EnemyState.ComputingPath;
                yield break; 
            }
        }
        //Debug.Log("OUT"); 
        // At the end of the path, is the agent has to throw an object, throw it
        if (throwable)
        {
            enemyState = EnemyState.ThrowingObject;
            yield break; 
        }
        enemyState = EnemyState.MakingDecision;
    }

    /// <summary>
    /// Cast the throwing Method
    /// Stop the agent, Orientate the agent
    /// Start and wait for the end of the animation
    /// <param name="_throwable">Object to throw</param>
    /// <returns></returns>
    protected IEnumerator CastGrab(TDS_Throwable _throwable)
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        //Pick up an object
        if (agent.IsMoving)
        {
            //Stop the agent if it is moving
            agent.StopAgent();
            speedCurrent = 0;
        }
        // Idle
        SetAnimationState((int)EnemyAnimationState.Idle);
        yield return null;
        //Grab the object
        if (canThrow)
        {
            //Grab the object 
            isWaiting = GrabObject(_throwable);
            //Wait until the end of the animation 
            while (isWaiting)
            {
                yield return null;
            }
        }
        yield return null;
        enemyState = EnemyState.MakingDecision;
    }

    /// <summary>
    /// Cast the throwing Method
    /// Stop the agent, Orientate the agent
    /// Start and wait for the end of the animation
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CastThrow()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        //Throw the held object
        if (agent.IsMoving)
        {
            //Stop agent
            agent.StopAgent();
            speedCurrent = 0;
        }
        //Idle
        SetAnimationState((int)EnemyAnimationState.Idle);
        yield return null;
        if (canThrow)
        {
            // Orientate the enemy
            if (CheckOrientation()) Flip();
            // Set the animation state to throw
            SetAnimationState((int)EnemyAnimationState.ThrowObject);
            isWaiting = true;
            //Wait until the end of the animation
            while (isWaiting)
            {
                yield return null;
            }
            throwable = null; 
            canThrow = false;
            SetAnimationState((int)EnemyAnimationState.Idle);
        }
        enemyState = EnemyState.MakingDecision;
    }
    #endregion

    #region TDS_Player
    /// <summary>
    /// Search the best player to target
    /// </summary>
    /// <returns>Best player to target</returns>
    protected virtual TDS_Player SearchTarget()
    {
        TDS_Player[] _targets = Physics.OverlapSphere(transform.position, detectionRange).Where(d => d.gameObject.HasTag("Player")).Select(t => t.GetComponent<TDS_Player>()).ToArray();
        if (_targets.Length == 0) return null;
        //Set constraints here (Distance, type, etc...)
        return _targets.Where(t => !t.IsDead).OrderBy(d => Vector3.Distance(transform.position, d.transform.position)).FirstOrDefault();
    }
    #endregion

    #region Overridden Methods
    /// <summary>
    /// Called when the enemy has to be brought closer
    /// </summary>
    /// <param name="_distance"></param>
    public override void BringCloser(float _distance)
    {
        StopAll();
        SetAnimationState((int)EnemyAnimationState.Brought); 
        base.BringCloser(_distance);
    }

    /// <summary>
    /// Burn the character during a certain amount of time
    /// </summary>
    /// <param name="_damagesMin">Min burn damages</param>
    /// <param name="_damagesMax">Max Burn damages</param>
    /// <param name="_duration">Duration of the burning effect</param>
    public override void Burn(int _damagesMin, int _damagesMax, float _duration)
    {
        base.Burn(_damagesMin, _damagesMax, _duration);
    }

    /// <summary>
    /// When the enemy dies, set its animation state to Death and remove it from the Area
    /// </summary>
    protected override void Die()
    {
        base.Die();
        if(PhotonNetwork.isMasterClient)
        {
            SetAnimationState((int)EnemyAnimationState.Death);
            if (Area) Area.RemoveEnemy(this);
        }
    }

    /// <summary>
    /// Call the base of the method flip
    /// if this client is the master, call the method online to flip the enemy in the other clients
    /// </summary>
    public override void Flip()
    {
        base.Flip();
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
    }

    /// <summary>
    /// Overridden Grab Object Method
    /// </summary>
    /// <param name="_throwable"></param>
    /// <returns></returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        bool _grabobject = base.GrabObject(_throwable);
        if(_grabobject && canThrow) SetAnimationState((int)EnemyAnimationState.GrabObject);
        return _grabobject;
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
    /// Put the character on the ground
    /// </summary>
    public override void PutOnTheGround()
    {
        if (!canBeDown || isDead || IsDown || !PhotonNetwork.isMasterClient) return;
        base.PutOnTheGround();
        if (IsDown)
        {
            StopAll();
            SetAnimationState((int)EnemyAnimationState.Grounded);
        }
    }

    /// <summary>
    /// Stop the current attack of the agent
    /// Desactivate the hitbox
    /// Set the bool IsAttacking to false
    /// Set the animation state to idle
    /// </summary>
    public override void StopAttack()
    {
        if (!PhotonNetwork.isMasterClient) return; 
        SetAnimationState((int)EnemyAnimationState.Idle);
        base.StopAttack();
    }

    /// <summary>
    /// Call this method as an animation event to throw the object
    /// </summary>
    public override void ThrowObject()
    {
        if (isDead) return; 
        float _range = Vector3.Distance(transform.position, playerTarget.transform.position) <  throwRange ? Vector3.Distance(transform.position, playerTarget.transform.position) : throwRange;
        Vector3 _pos = (transform.position - transform.right * _range);
        ThrowObject(_pos);
    }

    /// <summary>
    /// Overridden Throw Object Method
    /// 
    /// </summary>
    /// <param name="_targetPosition"></param>
    public override void ThrowObject(Vector3 _targetPosition)
    {
        base.ThrowObject(_targetPosition);
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
            if (!isDead && !IsDown)
            {
                SetAnimationState((int)EnemyAnimationState.Hit);
            }
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
            ApplyDamagesBehaviour(_damage, _position); 
        }
        return _isTakingDamages;
    }

    /// <summary>
    /// Overriden method of the UpdateLifeBarMethod
    /// If the client is master, call the method on every other clients
    /// </summary>
    /// <param name="_health"></param>
    public override void UpdateLifeBar(int _health)
    {
        //if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdateLifeBar"), new object[] { _health });
        base.UpdateLifeBar(_health);
    }
    #endregion

    #region Vector3
    /// <summary>
    /// Get an attacking position within a certain range to cast attacks
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetAttackingPosition()
    {
        Vector3 _offset = Vector3.zero;
        int _coeff = playerTarget.transform.position.x > transform.position.x ? -1 : 1;  
        _offset.z = Random.Range(-.2f, .2f); 
        if (throwable)
        {
            //Check if the agent is near enough to throw immediatly, or if he is to far away to throw
            float _distance = Mathf.Abs(playerTarget.transform.position.x - transform.position.x); 
            _offset.x = _distance < throwRange / 2 ? _distance : _distance < throwRange ? Random.Range(throwRange /2, _distance) : Random.Range(throwRange / 2, throwRange);
        }
        else
        {
            _offset.x = .25f; 
        }
        _offset.x *= _coeff; 
        return playerTarget.transform.position + _offset; 
    }
    #endregion

    #region Void
    /// <summary>
    /// USED IN ANIMATION
    /// Activate the hitbox with the settings of the currently casted attack
    /// Get the attack with its AnimationID
    /// </summary>
    /// <param name="_animationID">Animation ID entered in the animation window</param>
    protected void ActivateAttack(int _animationID)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        TDS_EnemyAttack _attack = attacks.Where(a => a.AnimationID == _animationID).FirstOrDefault();
        if (_attack == null) return;
        hitBox.Activate(_attack);
        _attack.ApplyAttackBehaviour(this); 
    }

    /// <summary>
    /// Compute the path
    /// If the path can be computed, set the state to Getting in Range
    /// else set the state to MakingDecision
    /// </summary>
    protected virtual void ComputePath()
    {
        if (isDead || !PhotonNetwork.isMasterClient) return; 
        if(!playerTarget || playerTarget.IsDead)
        {
            enemyState = EnemyState.Searching;
            return; 
        }
        if(IsParalyzed)
        {
            enemyState = EnemyState.MakingDecision;
            return; 
        }
        //Compute the path
        // Select a position
        bool _pathComputed = false;
        Vector3 _position;
        if (targetedThrowable && canThrow)
        {
            _position = targetedThrowable.transform.position;
        }
        else
        {
            _position = GetAttackingPosition();
        }
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
    /// Init the life bar of the enemy
    /// </summary>
    protected virtual void InitLifeBar()
    {
        //INIT LIFEBAR
        if (TDS_UIManager.Instance?.CanvasWorld)
        {
            TDS_UIManager.Instance.SetEnemyLifebar(this);
        }
    }

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

    /// <summary>
    /// Set a trigger in the animator
    /// </summary>
    /// <param name="_triggerName">Name of the trigger</param>
    public void SetAnimationTrigger(string _triggerName)
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnimationTrigger"), new object[] { _triggerName });
        }
        if (animator) animator.SetTrigger(_triggerName);
    }

    /// <summary>
    /// Stop the agent, drop the held object and stop the coroutines
    /// </summary>
    protected void StopAll()
    {
        agent.StopAgent();
        base.StopAttack(); 
        DropObject();

        //StopAllCoroutines();
        if (behaviourCoroutine != null)
        {
            StopCoroutine(behaviourCoroutine);
            behaviourCoroutine = null;
        }
        if (additionalCoroutine != null)
        {
            StopCoroutine(additionalCoroutine);
            additionalCoroutine = null;
        }
    }

    /// <summary>
    /// Set the bool isWaiting to false
    /// </summary>
    protected void StopWaiting() => isWaiting = false;

    /// <summary>
    /// Take a decision using the distance between the agent and its target
    /// If an attack can be casted, set the state to attack
    /// Else if the target is too far away, set the state to compute path
    /// </summary>
    protected virtual void TakeDecision()
    {
        //Take decisions
        if (isDead) return;
        if (isAttacking || hitBox.IsActive) StopAttack();
        if (agent.IsMoving) agent.StopAgent(); 
        // If the target can't be targeted, search for another target
        if (!playerTarget || playerTarget.IsDead)
        {
            enemyState = EnemyState.Searching;
            return; 
        }
        // Check if the agent can attack
        if (AttackCanBeCasted() && !IsPacific)
        {
            enemyState = EnemyState.Attacking;
        }
        // Else getting in range
        else if (!IsParalyzed)
        {
            enemyState = EnemyState.ComputingPath;
        }
    }

    /// <summary>
    /// Set IsPacific and IsParalyzed to false 
    /// And start the Behaviour Method
    /// </summary>
    protected void ResetBehaviour()
    {
        IsPacific = false;
        IsParalyzed = false;
        behaviourCoroutine = StartCoroutine(Behaviour()); 
    }

    /// <summary>
    /// This Method is called when the enemy has to be activated by an event
    /// </summary>
    public virtual void ActivateEnemy()
    {
        if (!PhotonNetwork.isMasterClient) return;
        IsPacific = false;
        IsParalyzed = false;
        behaviourCoroutine = StartCoroutine(Behaviour());
    }

    /// <summary>
    /// Called when the enemy takes damages
    /// Stop the agent
    /// Stop all the current Coroutines
    /// set its state to making decisions
    /// Apply the recoil and set the animation to hit if he's not dead
    /// </summary>
    /// <param name="_damage">Dealt damages</param>
    /// <param name="_position">Position of the attacker</param>
    protected virtual void ApplyDamagesBehaviour(int _damage, Vector3 _position)
    {
        StopAll();
        StartCoroutine(ApplyRecoil(_position));
        enemyState = EnemyState.MakingDecision; 
        if (!isDead && !IsDown)
        {
            SetAnimationState((int)EnemyAnimationState.Hit);
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        if (!agent) agent = GetComponent<CustomNavMeshAgent>();
        if(PhotonNetwork.isMasterClient)
        {
            OnDie += () => StopAllCoroutines();
            OnStopBringingCloser += () => SetAnimationState((int)EnemyAnimationState.Idle); 
            OnStopBringingCloser += () => behaviourCoroutine = StartCoroutine(Behaviour());
        }
        InitLifeBar(); 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (PhotonNetwork.isMasterClient)
        {
            behaviourCoroutine = StartCoroutine(Behaviour());
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
