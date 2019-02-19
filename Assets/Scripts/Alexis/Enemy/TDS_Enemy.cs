using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

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
    protected abstract IEnumerator Behaviour();
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
            StopAllCoroutines();
            enemyState = EnemyState.MakingDecision;
            if (isDead)
            {
                SetAnimationState(EnemyAnimationState.Death);
                Area.RemoveEnemy(this); 
            }
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
    /// Increase the speed and set the agent speed to the currentSpeed; 
    /// </summary>
    protected override void IncreaseSpeed()
    {
        base.IncreaseSpeed();
        agent.Speed = speedCurrent;
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
        agent.OnAgentStopped += () => speedCurrent = 0; 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //StartCoroutine(Behaviour());
        TDS_RPCManager.Instance.CallMethodOnline(TDS_RPCManager.GetInfo(photonView, this.GetType(), "null"));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
