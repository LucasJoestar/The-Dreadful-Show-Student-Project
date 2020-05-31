﻿using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random  = UnityEngine.Random;

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
     *  Date :          [26/06/2019]
     *	Author:         [THIEBAUT Alexis]
     *	
     *	[Add a new behaviour state]
     *	    - Add the wandering state, when there is too mush enemies around a player, the enemies has to wander around the player
     *	    
     *	-----------------------------------
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

    #region Constants
    /// <summary>
    /// Recoil Distance: When the enemy is hit, he is pushed in a direction with a distance equal of the recoilDistance
    /// </summary>
    protected const float RECOIL_DISTANCE = .1f;

    /// <summary>
    /// Recoil Distance: When the enemy is hit and dies, he is pushed in a direction with a distance equal of the recoilDistance 
    /// </summary>
    protected const float RECOIL_DISTANCE_DEATH = 1.2f;

    /// <summary>
    /// Recoil Time: When the enemy is hit, he is pushed in a direction during this time (in seconds)
    /// </summary>
    protected const float RECOIL_TIME_DEATH = .5f;
    #endregion 

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
    /// Bool that allow the enemy to pick up and throw objects according to the number of enemies that holds object.
    /// </summary>
    public bool CanThrow
    {
        get
        {
            if (Area)
                return canThrow && Area.GetEnemyThrowingCount() <= 2;
            return canThrow; 
        }
    }

    /// <summary>
    /// Bool that indicates if enemy life scales up depending on player amount.
    /// </summary>
    [SerializeField] protected bool doScaleOnPlayerAmount = false;

    /// <summary>
    /// When this bool is set to true, the enemy has to wait until it turn to false again
    /// </summary>
    protected bool isWaiting = false;

    /// <summary>
    /// State of the enemy 
    /// Check this state to know what to do
    /// </summary>
    [SerializeField] protected EnemyState enemyState = EnemyState.None;
    
    public EnemyState EnemyState
    {
        get { return EnemyState; }
    }

    /// <summary>
    /// Probability to taunt after wandering
    /// </summary>
    [SerializeField] protected float tauntProbability = 0; 

    /// <summary>
    /// The max distance an enemy can throw an object
    /// </summary>
    [SerializeField] protected float throwRange = 1;

    /// <summary>
    /// The minimum value of the wandering range around a player
    /// </summary>
    [SerializeField] protected float wanderingRangeMin = 5;

    /// <summary>
    /// The maximum value of the wandering range around a player
    /// </summary>
    [SerializeField] protected float wanderingRangeMax = 9;

    /// <summary>Backing field for <see cref="HealthScalePercent"/>.</summary>
    [SerializeField] protected int healthScalePercent = 50;

    /// <summary>
    /// Percentage this enemy life is scaled up by for every player in the game.
    /// </summary>
    public int HealthScalePercent
    {
        get { return healthScalePercent; }
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            healthScalePercent = value;
        }
    }

    public TDS_Damageable BringingTarget { get; set; } = null;  

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
    /// All enemies actually in game.
    /// </summary>
    public static List<TDS_Enemy> AllEnemies = new List<TDS_Enemy>();

    /// <summary>
    /// The target of the enemy
    /// </summary>
    protected TDS_Player playerTarget = null;

    /// <summary>
    /// Accessor of the playerTarget property
    /// </summary>
    public TDS_Player PlayerTarget { get { return playerTarget; } set { playerTarget = value; } }

    /// <summary>
    /// Spawner Area from which the enemy comes
    /// </summary>
    public TDS_SpawnerArea Area { get; set; }

    /// <summary>
    /// Accessor of the handsTransform Property
    /// </summary>
    public Transform HandsTransform { get { return handsTransform;  } }

    protected Vector3 targetLastPosition = Vector3.zero;
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

    #endregion

    #region Animator
    private static readonly int animationState_Hash = Animator.StringToHash("animationState");
    private static readonly int bringTargetCloser_Hash = Animator.StringToHash("BringTargetCloser");
    private static readonly int endBringingTargetCloser_Hash = Animator.StringToHash("EndBringingTargetCloser");
    private static readonly int enemyState_Hash = Animator.StringToHash("enemyState");
    private static readonly int isDead_Hash = Animator.StringToHash("isDead");
    private static readonly int isOutOfBattle_Hash = Animator.StringToHash("isOutOfBattle");
    private static readonly int hitTrigger_Hash = Animator.StringToHash("hitTrigger");
    private static readonly int lightHitTrigger_Hash = Animator.StringToHash("lightHitTrigger");
    private static readonly int ragingTrigger_Hash = Animator.StringToHash("RagingTrigger");
    private static readonly int resetBehaviour_Hash = Animator.StringToHash("resetBehaviour");
    private static readonly int stopDashing_Hash = Animator.StringToHash("StopDashing");
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
    protected virtual TDS_EnemyAttack GetAttack()
    {
        if (!playerTarget) return null; 
        float _distance = Mathf.Abs(transform.position.x - playerTarget.transform.position.x); 
        //If the enemy has no attack, return null
        if (attacks == null || attacks.Length == 0) return null;
        // Get all attacks that can hit the target
        TDS_EnemyAttack[] _availableAttacks = attacks.Where(a => a != null && a.MaxRange > _distance && a.MinRange < _distance).ToArray();
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
        if(playerTarget == null)
        {
            Debug.Log("No target");
            return false; 
        }
        if(attacks.Length == 0)
        {
            Debug.Log("No Attack");
            return false; 
        }
        if (TDS_Camera.Instance && (transform.position.x > TDS_Camera.Instance.CurrentBounds.XMax - 1 || transform.position.x < TDS_Camera.Instance.CurrentBounds.XMin + 1))
        {
            return false;
        }
        float _distance = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
        return Attacks.Any(a => a != null && a.MaxRange >= _distance &&  a.MinRange <= _distance) && Mathf.Abs(transform.position.z - playerTarget.transform.position.z) <=  collider.size.z;
    }

    /// <summary>
    /// Check if the enemy is in the right orientation to face the target
    /// </summary>
    /// <returns>true if the enemy has to flip to face the target</returns>
    protected virtual bool CheckOrientation()
    {
        if (!playerTarget) return false;
        Vector3 _dir = playerTarget.transform.position - transform.position;
        float _angle = Vector3.Angle(_dir, transform.right);
        return (_angle < 90); 
    }

    protected bool IsBetweenEnemyAndTarget(Vector3 _position)
    {
        if (!playerTarget) return false;
        if (_position.x >= TDS_Camera.Instance.CurrentBounds.XMax || _position.x <= TDS_Camera.Instance.CurrentBounds.XMin) return false; 
        return _position.x < transform.position.x && _position.x > playerTarget.transform.position.x || _position.x > transform.position.x && _position.x < playerTarget.transform.position.x;
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
        TDS_EnemyAttack _attack = GetAttack();
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
        return attacks.Where(a => a != null).Select(a => a.MaxRange).Min();
    }

    /// <summary>
    /// Return the minimum AttackRange
    /// </summary>
    /// <returns></returns>
    protected float GetMinRange()
    {
        return attacks.Where(a => a != null).Select(a => a.MaxRange).Max();
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
            _pos = transform.position + (_direction * RECOIL_DISTANCE_DEATH);
            while (Vector3.Distance(transform.position, _pos) > .1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _pos, RECOIL_DISTANCE_DEATH / RECOIL_TIME_DEATH * Time.deltaTime);
                yield return null;
            }
            yield break;
        }
        
        while (Vector3.Distance(transform.position, _pos) > .1f)
        {
            _pos = transform.position + (_direction * RECOIL_DISTANCE);
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
    public IEnumerator ApplyRecoveryTime(float _recoveryTime)
    {
        if (!PhotonNetwork.isMasterClient) yield break;
        yield return new WaitForSeconds(_recoveryTime); 
        SetEnemyState(EnemyState.MakingDecision); 
        yield break;
    }

    /// <summary>
    /// Stop the agent and orientate it
    /// Cast an attack that can touch the target
    /// Wait for the end of the attack
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CastAttack()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        if(IsPacific)
        {
            SetEnemyState(EnemyState.MakingDecision);
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
        yield return new WaitForSeconds(.15f);
        //Cast Attack
        float _cooldown = StartAttack();
        while (IsAttacking)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (IsDead) yield break; 
        yield return new WaitForSeconds(_cooldown);
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Cast the detection during the agent movements
    /// Check if there is objects to grab or throw
    /// Check if there is player to attack  
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CastDetection()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break; 
        if(animator.GetInteger(animationState_Hash) != 1) SetAnimationState((int)EnemyAnimationState.Run);
        while (agent.IsMoving)
        {
            //Orientate the agent
            if (isFacingRight && agent.Velocity.x < 0 || !isFacingRight && agent.Velocity.x > 0)
                Flip();
            
            //Increase the speed if necessary
            if (speedCurrent < speedMax)
            {
                IncreaseSpeed();
            }
            yield return null;
            if (!playerTarget) continue; 

            //if the target is too far from the destination, recalculate the path
            // OR If there is too much enemy in contact with the target, compute path to wander
            if ((Vector3.Distance(targetLastPosition, playerTarget.transform.position) > GetMaxRange()) 
                || (Area && Area.GetEnemyContactCount(playerTarget, wanderingRangeMin, this) >= 1))
            {
                yield return new WaitForSeconds(.1f);
                SetEnemyState(EnemyState.ComputingPath);
                yield break; 
            }
            // if any attack can be casted 
            if (AttackCanBeCasted() && !throwable)
            {
                SetEnemyState(EnemyState.Attacking);
                yield break;
            }
        }

        //Debug.Log("OUT"); 
        // At the end of the path, is the agent has to throw an object, throw it
        if (throwable)
        {
            SetEnemyState(EnemyState.ThrowingObject);
            yield break; 
        }
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Cast the throwing Method
    /// Stop the agent, Orientate the agent
    /// Start and wait for the end of the animation
    /// <param name="_throwable">Object to throw</param>
    /// <returns></returns>
    public IEnumerator CastGrab()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break;
        if(targetedThrowable == null || targetedThrowable.IsHeld)
        {
            if (targetedThrowable)
            {
                targetedThrowable = null;
            }
            SetEnemyState(EnemyState.MakingDecision);
            yield break; 
        }
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
        if (CanThrow)
        {
            //Grab the object 
            isWaiting = GrabObject(targetedThrowable);
            //Wait until the end of the animation 
            while (isWaiting)
            {
                yield return null;
            }
            targetedThrowable = null;
        }
        yield return null;
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Cast the throwing Method
    /// Stop the agent, Orientate the agent
    /// Start and wait for the end of the animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator CastThrow()
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
            yield return new WaitForEndOfFrame(); 
            //throwable = null; 
            canThrow = false;
            SetAnimationState((int)EnemyAnimationState.Idle);
        }
        yield return null; 
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Cast the wandering method 
    /// When too much enemies attack the same player, the enemy has to wander 
    /// Move until reaching a position then wait between 0 and 1 seconds before searching a new target
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Wander()
    {
        if (animator.GetInteger(animationState_Hash) != 1) SetAnimationState((int)EnemyAnimationState.Run);
        agent.AddAvoidanceLayer(new string[] { "Player" });
        Collider[] _colliders;
        Vector3 _closestPosition = targetedThrowable ? targetedThrowable.GetComponent<Collider>().ClosestPoint(transform.position) : Vector3.zero;
        _closestPosition.y = transform.position.y; 
        while (agent.IsMoving)
        {
            //Orientate the agent
            if (isFacingRight && agent.Velocity.x < 0 || !isFacingRight && agent.Velocity.x > 0)
                Flip();

            //Increase the speed if necessary
            if (speedCurrent < speedMax)
            {
                IncreaseSpeed();
            }
            yield return null;

            //Check if the area allow to grab object
            // If the enemy hasn't a throwable, check if he can grab one
            if (throwable == null && canThrow)
            {
                if (targetedThrowable)
                {
                    if(targetedThrowable.IsHeld)
                    {
                        targetedThrowable = null;
                        yield return null; 
                        continue; 
                    }
                    //If the targeted throwable is close enough, grab it

                    if (Vector3.Distance(transform.position, _closestPosition) <= (collider.size.z))
                    {
                        if (Vector3.Angle(targetedThrowable.transform.position - transform.position, transform.right) < 90) Flip();
                        SetEnemyState(EnemyState.PickingUpObject);
                        yield break;
                    }
                }
                else
                {
                    //Check if there is object around the enemy
                    _colliders = Physics.OverlapSphere(transform.position, wanderingRangeMax);
                    if (_colliders.Length > 0)
                    {
                        _colliders = _colliders.Where(c => c.gameObject.HasTag("Throwable") && c.GetComponent<TDS_Throwable>() && !c.GetComponent<TDS_Throwable>().IsHeld && c.GetComponent<TDS_Throwable>().CanBeGrabbedByEnemies).OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
                        if (_colliders.Length > 0)
                        {
                                //Get the closest throwable
                                targetedThrowable = _colliders.Select(c => c.GetComponent<TDS_Throwable>()).First();
                                //Set a new path to the throwable 
                                SetEnemyState(EnemyState.ComputingPath);
                                yield break; 
                        }
                    }
                }
            }

            //Search a new target and check if an attack can be casted, if so, break and attack
            playerTarget = GetPlayerTarget();
            if (AttackCanBeCasted())
            {
                agent.StopAgent();
                agent.RemoveAvoidanceLayer(new string[] { "Player" });
                SetAnimationState((int)EnemyAnimationState.Idle);
                SetEnemyState(EnemyState.Attacking);
                yield break; 
            }
        }
        agent.RemoveAvoidanceLayer(new string[] { "Player" });
        if ((isFacingRight && playerTarget.transform.position.x < transform.position.x) || (!isFacingRight && playerTarget.transform.position.x > transform.position.x))
        {
            Flip();
        }
        SetAnimationState((int)EnemyAnimationState.Idle);
        SetEnemyState(EnemyState.Waiting);
    }

    /// <summary>
    /// Waiting Method
    /// </summary>
    /// <returns></returns>
    public IEnumerator Waiting()
    {
        agent.StopAgent();
        SetAnimationState(0);
        targetLastPosition = playerTarget.transform.position; 
        Vector3 _offset = playerTarget.transform.position - targetLastPosition;
        TDS_Bounds _currentBounds = TDS_Camera.Instance.CurrentBounds; 

        while (Area && Area.GetEnemyContactCount(playerTarget, wanderingRangeMin, this) >= 1)
        {
            Vector3 _targetedPosition = transform.position + _offset;
            _targetedPosition.x = Mathf.Clamp(_targetedPosition.x, _currentBounds.XMin + agent.Radius, _currentBounds.XMax - agent.Radius);
            _targetedPosition.z = Mathf.Clamp(_targetedPosition.z, _currentBounds.ZMin + agent.Radius, _currentBounds.ZMax - agent.Radius);
            if (Vector3.Distance(targetLastPosition, playerTarget.transform.position) > agent.Radius && _targetedPosition.x < TDS_Camera.Instance.CurrentBounds.XMax && _targetedPosition.x > TDS_Camera.Instance.CurrentBounds.XMin)
            {
                agent.SetDestination(_targetedPosition);
                SetEnemyState(EnemyState.Wandering);
                yield break; 
            }
            else
            {
                isWaiting = (Random.value * 100) < tauntProbability;
                if (isWaiting)
                {
                    if (agent.IsMoving) agent.StopAgent(); 
                    SetAnimationState((int)EnemyAnimationState.Taunt);
                    while (isWaiting)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(Random.Range(.1f, 2)); 
                }
            }
            //SearchTarget();  
            yield return null;
        }
        yield return null; 
        SetEnemyState(EnemyState.MakingDecision);
    }
    #endregion

    #region TDS_Player
    /// <summary>
    /// Search the best player to target
    /// </summary>
    /// <returns>Best player to target</returns>
    protected virtual TDS_Player GetPlayerTarget()
    {
        TDS_Player[] _targets = null; 
        
        if(TDS_LevelManager.Instance)
            _targets = TDS_LevelManager.Instance.AllPlayers.Where(t => !t.IsDead).ToArray();
        else
            _targets = Physics.OverlapSphere(transform.position, 10).Where(d => d.gameObject.HasTag("Player")).Select(t => t.GetComponent<TDS_Player>()).ToArray();


        if (_targets.Length == 0) return null;
        //Set constraints here (Distance, type, etc...)
        _targets = _targets.Where(t => !t.IsDead).OrderBy(p => Vector3.Distance(transform.position, p.transform.position)).ToArray();
        if (Area) _targets.OrderBy(t => Area.GetPlayerTargetCount(t.PlayerType)); 
        return _targets.FirstOrDefault();
    }
    #endregion

    #region Overridden Methods
    /// <summary>
    /// Called when the enemy has to be brought closer
    /// </summary>
    /// <param name="_distance"></param>
    public override bool BringCloser(float _distance)
    {
        _distance -= agent.Radius * Mathf.Sign(_distance);
        if (!base.BringCloser(_distance)) return false;

        //StopAll();
        SetEnemyState(EnemyState.None); 
        SetAnimationState((int)EnemyAnimationState.Brought);

        return true;
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
        if (PhotonNetwork.isMasterClient)
        {
            //StopAll();
            SetEnemyState(EnemyState.None);
            SetAnimationState((int)EnemyAnimationState.Death);
        }

        if (AllEnemies.Contains(this)) AllEnemies.Remove(this);
        if (hitBox.IsActive) hitBox.Desactivate();
        base.Die();
    }

    /// <summary>
    /// Tells the Character that he's getting up.
    /// </summary>
    public override void GetUp()
    {
        base.GetUp();
 
        SetAnimationState((int)EnemyAnimationState.Idle);
        StartCoroutine(ApplyRecoveryTime(.25f));
    }

    /// <summary>
    /// Overridden Grab Object Method
    /// </summary>
    /// <param name="_throwable"></param>
    /// <returns></returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        bool _grabobject = base.GrabObject(_throwable);
        if(_grabobject) SetAnimationState((int)EnemyAnimationState.GrabObject);
        return _grabobject;
        // Does the agent has a different behaviour from the players? 
    }

    /// <summary>
    /// Increase the speed and set the agent speed to the currentSpeed; 
    /// </summary>
    protected override void IncreaseSpeed()
    {
        base.IncreaseSpeed();
        agent.Speed = speedCurrent * speedCoef;
    }

    /// <summary>
    /// Put the character on the ground
    /// </summary>
    public override bool PutOnTheGround()
    {
        if (!canBeDown || isDead || IsDown || !PhotonNetwork.isMasterClient) return false;
        if (!base.PutOnTheGround()) return false;

        StopAll();
        SetEnemyState(EnemyState.None);
        SetAnimationState((int)EnemyAnimationState.Grounded);

        return true;
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
    /// Method called when stopped being bringed closer.
    /// </summary>
    public override void StopBringingCloser()
    {
        base.StopBringingCloser();

        SetAnimationState((int)EnemyAnimationState.Idle);

        //behaviourCoroutine = StartCoroutine(Behaviour());
        SetEnemyState(EnemyState.MakingDecision); 
    }

    /// <summary>
    /// Call this method as an animation event to throw the object
    /// </summary>
    public override bool ThrowObject_A()
    {
        if (isDead || !playerTarget) return false; 
        float _range = Vector3.Distance(transform.position, playerTarget.transform.position) <  throwRange ? Vector3.Distance(transform.position, playerTarget.transform.position) : throwRange;
        Vector3 _pos = (transform.position - transform.right * _range);
        StopWaiting(); 
        return ThrowObject(_pos);
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
    /// Overriden method for the knockBack behaviour
    /// Stop the agent and make it go to the "Brought" Animation State
    /// </summary>
    /// <param name="_toRight"></param>
    /// <returns></returns>
    public override bool Knockback(bool _toRight)
    {
        bool _isKnockedBack = base.Knockback(_toRight);
        if(_isKnockedBack)
        {
            SetEnemyState(EnemyState.None);
            SetAnimationState((int)EnemyAnimationState.Brought);
        }
        return _isKnockedBack; 
    }

    /// <summary>
    /// The Overriden method of the KnockBackCoroutine
    /// At the end of the Coroutine, set the animation state of the agent to idle, then after .1 second, reset its behaviour
    /// </summary>
    /// <param name="_toRight"></param>
    /// <returns></returns>
    protected override IEnumerator KnockbackCoroutine(bool _toRight)
    {
        yield return base.KnockbackCoroutine(_toRight);
        SetAnimationState((int)EnemyAnimationState.Idle);
        yield return new WaitForSeconds(.1f);
        SetEnemyState(EnemyState.MakingDecision);
    }
    #endregion

    #region Vector3
    /// <summary>
    /// Get an attacking position within a certain range to cast attacks
    /// Also if there is to much enemies close enough the player, go to a wandering position
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetAttackingPosition(out bool _hasToWander)
    {
        //Search a new target
        if (!playerTarget) SearchTarget();  

        Vector3 _offset = Vector3.zero;
        Vector3 _returnedPosition = transform.position; 
        _hasToWander = Area && Area.GetEnemyContactCount(playerTarget, wanderingRangeMin, this) > 1;

        // If the agent is currently wandering, keep the offset with the target
        if (agent.IsMoving && playerTarget && _hasToWander && !throwable)
        {
            _offset = playerTarget.transform.position - targetLastPosition;

            _offset.y = 0; 
            _returnedPosition = agent.LastPosition + _offset;
        }
        else
        {
            _offset.z = Random.Range(-agent.Radius, agent.Radius);
            //If the agent has a throwable, get in range to throw it on the target
            if (throwable)
            {
                _hasToWander = false;
                //Check if the agent is near enough to throw immediatly, or if he is to far away to throw
                float _distance = Mathf.Abs(playerTarget.transform.position.x - transform.position.x);
                _offset.x = _distance < throwRange / 2 ? _distance : _distance < throwRange ? Random.Range(throwRange / 2, _distance) : Random.Range(throwRange / 2, throwRange);
            }
            else if (!_hasToWander)
            {
                TDS_EnemyAttack _attack = GetAttack();
                if (_attack)
                {
                    _offset.x = Random.Range(_attack.MinRange, _attack.MaxRange) - (agent.Radius);
                }

                if (_offset.x < agent.Radius) _offset.x = agent.Radius;
            }
            else
            {
                _offset = new Vector3(Random.Range(wanderingRangeMin, wanderingRangeMax), 0, Random.Range(wanderingRangeMin, wanderingRangeMax));
            }

            int _coeff = _hasToWander ? Random.value > .5f ? 1 : -1 : playerTarget.transform.position.x > transform.position.x ? -1 : 1;
            _offset.x *= _coeff;
            
            _returnedPosition = playerTarget.transform.position + _offset;
        }

        targetLastPosition = playerTarget.transform.position;
        return _returnedPosition;
    }

    /// <summary>
    /// Get an attacking position within a certain range to cast attacks
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 GetAttackingPosition()
    {
        Vector3 _offset = Vector3.zero;
        int _coeff = playerTarget.transform.position.x > transform.position.x ? -1 : 1;
        _offset.z = Random.Range(-agent.Radius, agent.Radius);
        if (throwable)
        {
            //Check if the agent is near enough to throw immediatly, or if he is to far away to throw
            float _distance = Mathf.Abs(playerTarget.transform.position.x - transform.position.x);
            _offset.x = _distance < throwRange / 2 ? _distance : _distance < throwRange ? Random.Range(throwRange / 2, _distance) : Random.Range(throwRange / 2, throwRange);
        }
        else
        {
            TDS_EnemyAttack _attack = GetAttack();
            if (_attack)
            {
                _offset.x = Random.Range(_attack.MinRange, _attack.MaxRange) - (agent.Radius);
            }
            else
                _offset.x = Random.Range(GetMinRange(), GetMaxRange()) - agent.Radius;

            if (_offset.x < agent.Radius) _offset.x = agent.Radius;
        }
        _offset.x *= _coeff;
        targetLastPosition = playerTarget.transform.position;
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
        TDS_EnemyAttack _attack = attacks.Where(a => a.AnimationID == _animationID).FirstOrDefault();
        if (_attack == null)
            return;

        if (!PhotonNetwork.isMasterClient)
            return;

        hitBox.Activate(_attack);
        _attack.ApplyAttackBehaviour(this);

        // Play sound
        _attack.PlaySound(gameObject);
    }

    /// <summary>
    /// This Method is called when the enemy has to be activated by an event
    /// </summary>
    public virtual void ActivateEnemy(bool _hastoTaunt = false)
    {
        if (!PhotonNetwork.isMasterClient || isDead) return;
        IsPacific = false;
        IsParalyzed = false;
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Call this method after the taunt animation
    /// </summary>
    public void ActivateEnemyAfterTaunt()
    {
        if (!PhotonNetwork.isMasterClient || isDead) return;

        SetAnimationState((int)EnemyAnimationState.Idle);
        if (isWaiting)
        {
            isWaiting = false;
            return;
        }

        ActivateEnemy(); 
    }

    /// <summary>
    /// Compute the path
    /// If the path can be computed, set the state to Getting in Range
    /// else set the state to MakingDecision
    /// </summary>
    public virtual void ComputePath()
    {
        if (isDead || !PhotonNetwork.isMasterClient) return; 
        if(IsParalyzed)
        {
            SetEnemyState(EnemyState.MakingDecision);
            return; 
        }
        bool _pathComputed = false;
        Vector3 _position;

        if (!playerTarget || playerTarget.IsDead)
        {
            if (TDS_Camera.Instance && (transform.position.x > TDS_Camera.Instance.CurrentBounds.XMax || transform.position.x < TDS_Camera.Instance.CurrentBounds.XMin))
            {
                _position = new Vector3(transform.position.x > TDS_Camera.Instance.CurrentBounds.XMax ? TDS_Camera.Instance.CurrentBounds.XMax - 1 - agent.Radius : TDS_Camera.Instance.CurrentBounds.XMin + 1 + agent.Radius, transform.position.y, transform.position.z);
                agent.SetDestination(_position);
                SetEnemyState(EnemyState.GettingInRange);
                return;
            }
            SetEnemyState(EnemyState.MakingDecision);
            return;
        }
        //Compute the path
        bool _hasToWander = false;
        // Select a position
        if (targetedThrowable && canThrow)
        {
            _position = targetedThrowable.transform.position;
            _hasToWander = true; 
        }
        else
        {
            _position = GetAttackingPosition(out _hasToWander);
        }
        _position.x = Mathf.Clamp(_position.x, TDS_Camera.Instance.CurrentBounds.XMin + 1 + agent.Radius, TDS_Camera.Instance.CurrentBounds.XMax - 1 - agent.Radius); 
        if(!targetedThrowable && !throwable && Vector3.Distance(_position, transform.position) <= agent.Radius)
        {
            SetEnemyState(EnemyState.Waiting);
            return; 
        }
        // Debug.Log(_position); 
        _pathComputed = agent.CheckDestination(_position);
        //If the path is computed, reach the end of the path
        if (_pathComputed)
        {
            SetAnimationState((int)EnemyAnimationState.Run);
            //Orientate the agent
            if (isFacingRight && agent.Velocity.x < 0 || !isFacingRight && agent.Velocity.x > 0)
                Flip();
            SetEnemyState(_hasToWander? EnemyState.Wandering : EnemyState.GettingInRange);
        }
        else
        {
            SetAnimationState((int)EnemyAnimationState.Idle);
            SetEnemyState(EnemyState.MakingDecision);
        }
    }

    /// <summary>
    /// Init the life bar of the enemy
    /// </summary>
    protected virtual void InitLifeBar()
    {
        //INIT LIFEBAR
        if (TDS_UIManager.Instance.CanvasWorld)
        {
            TDS_UIManager.Instance.SetEnemyLifebar(this);
        }
    }

    /// <summary>
    /// Force the Dying Methods of the enemy
    /// </summary>
    protected void KillEnemy()
    {
        TakeDamage(999); 
    }

    /// <summary>
    /// Set the animation of the enemy to the animationID
    /// </summary>
    /// <param name="_animationID"></param>
    public void SetAnimationState(int _animationID)
    {
        if (!animator) return;
        switch ((EnemyAnimationState)_animationID)
        {
            case EnemyAnimationState.Hit:
                animator.SetTrigger(hitTrigger_Hash);
                break;
            case EnemyAnimationState.Death:
                animator.SetBool(isDead_Hash, true);
                break; 
            case EnemyAnimationState.BringTargetCloser:
                animator.SetTrigger(bringTargetCloser_Hash);
                break;
            case EnemyAnimationState.EndBringingTargetCloser:
                animator.SetTrigger(endBringingTargetCloser_Hash);
                break;
            case EnemyAnimationState.StopDashing:
                animator.SetTrigger(stopDashing_Hash);
                break;
            case EnemyAnimationState.LightHit:
                animator.SetTrigger(lightHitTrigger_Hash);
                break;
            case EnemyAnimationState.Rage:
                animator.SetTrigger(ragingTrigger_Hash); 
                break; 
            default:
                animator.SetInteger(animationState_Hash, _animationID);
                break;
        }
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, this.GetType(), "SetAnimationState", new object[] { _animationID });
    }

    /// <summary>
    /// Set the new enemyState and update the state machine
    /// </summary>
    /// <param name="_newState">New state</param>
    public void SetEnemyState(EnemyState _newState)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        enemyState = _newState;
        switch (enemyState) 
        {
            case EnemyState.MakingDecision:
                animator.SetTrigger(resetBehaviour_Hash);
                break;
            case EnemyState.OutOfBattle:
                animator.SetBool(isOutOfBattle_Hash, true); 
                break;
            case EnemyState.GettingInRange:
                break;
            default:
                animator.ResetTrigger(resetBehaviour_Hash); 
                break;
        }
        animator.SetInteger(enemyState_Hash, (int)enemyState); 
    }

    /// <summary>
    /// Stop the agent, drop the held object and stop the coroutines
    /// </summary>
    public void StopAll()
    {
        if (agent.IsMoving) agent.StopAgent();
        if (isAttacking) StopAttack();
        if (throwable) DropObject();
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
    public virtual void TakeDecision()
    {
        //Take decisions
        if (isDead || !PhotonNetwork.isMasterClient) return;
        if (isAttacking || hitBox.IsActive)
        {
            StopAttack();
        }
        if (agent.IsMoving) agent.StopAgent();
        speedCurrent = speedInitial; 
        agent.Speed = speedCurrent; 
        if(transform.position.x > TDS_Camera.Instance.CurrentBounds.XMax || transform.position.x < TDS_Camera.Instance.CurrentBounds.XMin)
        {
            SetEnemyState(EnemyState.ComputingPath);
            return; 
        }
        if(targetedThrowable && targetedThrowable.IsHeld)
        {
            targetedThrowable = null; 
        }
        // If the target can't be targeted, search for another target
        if (!playerTarget || playerTarget.IsDead)
        {
            SearchTarget();
        }
        // Check if the agent can attack
        if (AttackCanBeCasted() && !IsPacific)
        {
            SetEnemyState(EnemyState.Attacking);
        }
        // Else getting in range
        else if (!IsParalyzed)
        {
            SetEnemyState(EnemyState.ComputingPath);
        }
        else
        {
            SetEnemyState(EnemyState.MakingDecision); 
        }
    }

    /// <summary>
    /// Called when the bringing target has stopped
    /// </summary>
    public void TargetBrought()
    {
        if (BringingTarget) BringingTarget.OnStopBringingCloser -= TargetBrought;
        BringingTarget = null;
        SetAnimationState((int)EnemyAnimationState.EndBringingTargetCloser);
    }

    /// <summary>
    /// Called at the end of a bring target attack.
    /// </summary>
    public void NoTargetToBrought()
    {
        if (BringingTarget) return;
        SetAnimationState((int)EnemyAnimationState.EndBringingTargetCloser);
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
        if (!PhotonNetwork.isMasterClient) return;
        if (BringingTarget) BringingTarget.StopBringingCloser(); 
        StartCoroutine(ApplyRecoil(_position));
        if (!isDead && !IsDown)
        {
            SetAnimationState((int)EnemyAnimationState.Hit);
        }
    }

    /// <summary>
    /// Search the best Target
    /// </summary>
    public void SearchTarget()
    {
        playerTarget = GetPlayerTarget();
        if(playerTarget) targetLastPosition = playerTarget.transform.position; 
    }

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        if (!agent) agent = GetComponent<CustomNavMeshAgent>();
        OnDie += () => StopAllCoroutines();
        InitLifeBar();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (AllEnemies.Contains(this))
            AllEnemies.Remove(this);
    }

    // Use this for initialization
    protected override void Start()
    {
        if(PhotonNetwork.isMasterClient)
        {
            if (photonView.owner == null)
                photonView.TransferOwnership(PhotonNetwork.masterClient);
        }

        // Scales up health on player amount
        if (doScaleOnPlayerAmount)
        {
            for (int _i = 1; _i < TDS_LevelManager.Instance.AllPlayers.Length; _i++)
            {
                HealthMax += (int)(healthMax * (healthScalePercent / 100f));
            }
            HealthCurrent = healthMax;
        }

        if (!AllEnemies.Contains(this))
            AllEnemies.Add(this);

        base.Start();
    }

    private void OnDisable()
    {
        if (AllEnemies.Contains(this)) AllEnemies.Remove(this);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(playerTarget != null)
        {
            Gizmos.color = Vector3.Distance(playerTarget.transform.position, transform.position) <= GetMaxRange() ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, playerTarget.transform.position);
        }
    }

    public void OnBecameInvisibleCallBack()
    {
        if (PhotonNetwork.isMasterClient && !IsPacific && !IsParalyzed)
        {
            Invoke("KillEnemy", 20.0f);
            //Debug.Log($"Kill {gameObject.name} called in 20.0s."); 
        }
    }

    public void OnBecameVisibleCallBack()
    {
        if (PhotonNetwork.isMasterClient) 
            CancelInvoke("KillEnemy");
    }

    #region Editor
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (Time.timeSinceLevelLoad > .1f)
        {
            if (PhotonNetwork.isMasterClient && photonView.owner == null) photonView.TransferOwnership(PhotonNetwork.masterClient);
        }
    }
    #endregion
    #endregion

    #endregion
}
