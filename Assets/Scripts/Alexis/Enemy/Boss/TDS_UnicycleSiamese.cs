using System;
using System.Collections;
using System.Collections.Generic;
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
            if (isFacingRight && agent.Velocity.x > 0 || !isFacingRight && agent.Velocity.x < 0)
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
            if (AttackCanBeCasted())
            {
                enemyState = EnemyState.Attacking;
                yield break;
            }
        }
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
        StartCoroutine(Behaviour()); 
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #endregion
}
