using System.Collections;
using System.Linq; 
using UnityEngine;

public abstract class TDS_Minion : TDS_Enemy
{
    /* TDS_Minion :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Class that implement the global behaviour of a minion.
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
     *	    - Removing the Interface

     *	    
     *	-----------------------------------
     * 
	 *	Date :			[22/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Creating the Minion class]
     *	    - Change Unity Methods into override Methods.
     *	    - Adding a bool hasEvolved.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] protected bool hasEvolved = false;

    protected int ragingCount = 0;
    [SerializeField] protected int ragingThreshold = 3;

    [SerializeField] protected float resetRageDelay = 1; 

    protected Coroutine resetRagingThreshold = null;

    protected Coroutine trembleAnimation = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Set the boolean has Evolved to true
    /// </summary>
    protected virtual void Evolve()
    {
        hasEvolved = true; 
    }

    protected IEnumerator ResetRage()
    {
        yield return new WaitForSeconds(resetRageDelay);
        resetRagingThreshold = null; 
        ragingCount = 0; 
    }

    protected virtual void ResetRageOnAttack()
    {
        if (!PhotonNetwork.isMasterClient) return;

        // Reset treshold if needed
        if (resetRagingThreshold != null)
        {
            StopCoroutine(resetRagingThreshold);
            resetRagingThreshold = null;
        }
        ragingCount = 0;
    }

    protected IEnumerator TrembleAnimation(TDS_EnemyAttack _attack)
    {
        transform.position = new Vector3(transform.position.x - .05f, transform.position.y, transform.position.z);
        yield return new WaitForSeconds(.03f);

        for (int _i = 0; _i < 5; _i++)
        {
            transform.position = new Vector3(transform.position.x + .1f, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(.03f);
            transform.position = new Vector3(transform.position.x - .1f, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(.03f);
        }

        transform.position = new Vector3(transform.position.x + .05f, transform.position.y, transform.position.z);

        SetAnimationState(_attack.AnimationID);
    }
    #endregion

    #region Overridden Methods
    /// <summary>
    /// Override the ApplyDamagesBehaviour Method
    /// </summary>
    /// <param name="_damage"></param>
    /// <param name="_position"></param>
    protected override void ApplyDamagesBehaviour(int _damage, Vector3 _position)
    {
        if ((ragingCount > ragingThreshold) || IsDown)
        {
            SetAnimationState((int)EnemyAnimationState.LightHit);
            return;
        }
        else
        {
            base.ApplyDamagesBehaviour(_damage, _position);
            ragingCount++;

            if (trembleAnimation != null)
            {
                StopCoroutine(trembleAnimation);
                trembleAnimation = null;
            }
            if (resetRagingThreshold != null) StopCoroutine(resetRagingThreshold);
            resetRagingThreshold = StartCoroutine(ResetRage());
        }
    }

    /// <summary>
    /// Override the Die Method
    /// Remove the enemy from the Area
    /// </summary>
    protected override void Die()
    {
        base.Die();
        if (Area) Area.RemoveEnemy(this);
    }

    public override bool PutOnTheGround()
    {
        if ((ragingCount > ragingThreshold) || !base.PutOnTheGround()) return false;
        SetAnimationState((int)EnemyAnimationState.LightHit);

        return true;
    }

    protected override float StartAttack()
    {
        SetAnimationState((int)EnemyAnimationState.Idle); 
        // If making a counter attack, make the enemy tremble a bit before attacking
        if (ragingCount > ragingThreshold)
        {
            TDS_EnemyAttack _attack = GetAttack();
            if (_attack == null)
            {
                return 0;
            }

            IsAttacking = true;
            _attack.ConsecutiveUses++;
            attacks.ToList().Where(a => a != _attack).ToList().ForEach(a => a.ConsecutiveUses = 0);

            if (resetRagingThreshold != null) StopCoroutine(resetRagingThreshold);
            trembleAnimation = StartCoroutine(TrembleAnimation(_attack));

            return _attack.Cooldown;
        }
        return base.StartAttack();
    }
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        if (TDS_LevelManager.Instance?.AllPlayers?.Length > 1) ragingThreshold = 1;
        hitBox.OnStopAttack += ResetRageOnAttack;
    }
    #endregion

    #endregion
}
