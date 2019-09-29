using System.Collections;
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
    private IEnumerator ResetAttackCoolDown(float _cooldown = 2f)
    {
        IsPacific = true;
        yield return new WaitForSeconds(_cooldown);
        IsPacific = false; 
    }

    #region OverridenMethods
    /// <summary>
    /// Check if the distance between the player and its target on the x axis is smaller than the minimum attack range
    /// Then check if the distance between the player and its target on the z axis is smaller than the agent's radius  
    /// </summary>
    /// <returns></returns>
    protected override bool AttackCanBeCasted()
    {
        float _distance = Mathf.Abs(transform.position.x - playerTarget.transform.position.x);
        bool _canAttack = Attacks.Any(a => a != null && a.MaxRange >= _distance && a.MinRange <= _distance);
        if(!_canAttack) return false;
        _canAttack = Attacks.Any(a => a.MinRange > 0) || Mathf.Abs(transform.position.z - playerTarget.transform.position.z) <= collider.size.z;

        ///
        if (_canAttack)
            _canAttack = IsFacingRight ? transform.position.x < playerTarget.transform.position.x : transform.position.x > playerTarget.transform.position.x; 
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
            _v.z = UnityEngine.Random.Range(bounds.ZMin + agent.Radius, bounds.ZMax - agent.Radius);
            _v.x = hasReachedRightBound ? bounds.XMin + agent.Radius : bounds.XMax - agent.Radius; 
        }
        return _v; 
    }

    public override IEnumerator CastAttack()
    {
        if (isDead || !PhotonNetwork.isMasterClient) yield break;
        if (IsPacific)
        {
            SetEnemyState(EnemyState.MakingDecision);
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
        StartCoroutine(ResetAttackCoolDown(_cooldown)); 
        SetEnemyState(EnemyState.MakingDecision);
    }

    /// <summary>
    /// Flip, Increase the speed and detect the closest player.
    /// Check if an attack can be casted on this closest player
    /// </summary>
    /// <returns></returns>
    public override IEnumerator CastDetection()
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
            SearchTarget(); 
            // if any attack can be casted 
            if (!playerTarget) continue; 
            if (AttackCanBeCasted())
            {
                SetEnemyState(EnemyState.Attacking);
                yield break;
            }
        }
        SetEnemyState(EnemyState.ComputingPath);
    }

    /// <summary>
    /// Compute the path to the bounds
    /// </summary>
    public override void ComputePath()
    {
        if (isDead || !PhotonNetwork.isMasterClient) return;
        if (!playerTarget) SearchTarget(); 
        bool _pathComputed = false;
        Vector3 _targetedPosition = GetAttackingPosition();
        if (agent.IsMoving) agent.StopAgent(); 
        _pathComputed = agent.CheckDestination(_targetedPosition);
        //If the path is computed, reach the end of the path
        if (_pathComputed)
        {
            SetEnemyState(EnemyState.GettingInRange);
            hasReachedRightBound = !hasReachedRightBound;
        }
        else
        {
            SetEnemyState(EnemyState.MakingDecision);
        }
    }

    /// <summary>
    /// Take a Decision
    /// </summary>
    public override void TakeDecision()
    {
        if (IsDead || !PhotonNetwork.isMasterClient) return;
        if (isAttacking || hitBox.IsActive) StopAttack();
        // If the target can't be targeted, search for another target
        SetEnemyState(EnemyState.ComputingPath);
    }

    protected override void Die()
    {
        base.Die();
        if (Area) Area.RemoveEnemy(this);
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
        SetEnemyState(EnemyState.MakingDecision);
        StartCoroutine(ResetAttackCoolDown()); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
