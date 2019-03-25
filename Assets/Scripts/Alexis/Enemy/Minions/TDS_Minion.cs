using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] protected TDS_MinionAttack[] attacks = new TDS_MinionAttack[] {};
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Called to apply the effect of an attack 
    /// This effect can be a movement, an instanciation of an object
    /// Write effects in each specific minion
    /// </summary>
    /// <param name="_type">Type of the attack</param>
    protected abstract void ApplyAttackEffect(MinionAttackType _type);
    
    /// <summary>
    /// Set the boolean has Evolved to true
    /// </summary>
    protected virtual void Evolve()
    {
        hasEvolved = true; 
    }

    #region TDS_MinionAttack
    /// <summary>
    /// Select the attack to cast
    /// If there is no attack return null
    /// Selection is based on the Range and on the Probability of an attack
    /// </summary>
    /// <param name="_distance">Distance between the agent and its target</param>
    /// <returns>Attack to cast</returns>
    protected TDS_MinionAttack GetAttack(float _distance)
    {
        //If the enemy has no attack, return null
        if (attacks == null || attacks.Length == 0) return null;
        // Get all attacks that can hit the target
        TDS_MinionAttack[] _availableAttacks = attacks.Where(a => a.IsDistanceAttack || a.PredictedRange > _distance).ToArray();
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

    #endregion

    #region Overridden Methods
    /// <summary>
    /// Activate the hitbox with the settings of the currently casted attack
    /// Get the attack with its AnimationID
    /// </summary>
    /// <param name="_animationID">Animation ID entered in the animation window</param>
    protected override void ActivateAttack(int _animationID)
    {
        TDS_MinionAttack _attack = attacks.Where(a => a.AnimationID == _animationID).FirstOrDefault();
        if (_attack == null) return;
        hitBox.Activate(_attack);
        ApplyAttackEffect(_attack.AttackType);
    }

    /// <summary>
    /// Return true if the distance is less than the minimum predicted range of the Minion attack
    /// </summary>
    /// <param name="_distance">distance between player and target</param>
    /// <returns>does the attack can be cast</returns>
    protected override bool AttackCanBeCasted(float _distance)
    {
        return _distance <= attacks.Min(a => a.PredictedRange);
    }

    /// <summary>
    /// Return the minimum attack range 
    /// </summary>
    /// <returns></returns>
    protected override float GetMaxRange()
    {
        return attacks.Select(a => a.PredictedRange).Min();
    }

    /// <summary>
    /// Return the maximal attack range 
    /// </summary>
    /// <returns></returns>   
    protected override float GetMinRange()
    {
        return attacks.Select(a => a.PredictedRange).Max();
    }

    /// <summary>
    /// Cast an attack: Add a use to the attack and activate the enemy hitbox with this attack
    /// Set the animation to the animation state linked to the AnimationID of the attack 
    /// Reset to 0 consecutive uses of the other attacks
    /// Return the cooldown of the attack if it can find one
    /// </summary>
    /// <param name="_attack">Attack to cast</param>
    /// <returns>cooldown of the attack</returns>
    protected override float StartAttack(float _distance)
    {
        TDS_MinionAttack _attack = GetAttack(_distance);
        if (_attack == null)
        {
            return 0;
        }
        IsAttacking = true;
        _attack.ConsecutiveUses++;
        attacks.ToList().Where(a => a != _attack).ToList().ForEach(a => a.ConsecutiveUses = 0);
        SetAnimationState(_attack.AnimationID);
        //hitBox.Activate(_attack); THE HIT BOX IS NOW ACTIVATED INTO THE ANIMATION BY CALLING THE METHOD "ActivateAttack" with the AnimationID of the attack
        //ApplyAttackEffect(_attack.AttackType); 
        return _attack.Cooldown;
    }
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
