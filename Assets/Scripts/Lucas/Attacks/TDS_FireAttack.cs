using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Fire Attack", menuName = "Attacks/Fire Attack", order = 1), Serializable]
public class TDS_FireAttack : TDS_Attack
{
    /* TDS_Attack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="BurnPercentageHighest"/>.</summary>
    [Header("Fire")]
    [SerializeField] protected int burnPercentageHighest = 100;

    /// <summary>
    /// Highest percentage for the burn effect to be applied.
    /// </summary>
    public int BurnPercentageHighest
    {
        get { return burnPercentageHighest; }
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            burnPercentageHighest = value;
        }
    }

    /// <summary>Backing field for <see cref="BurnPercentageLowest"/>.</summary>
    [SerializeField] private int burnPercentageLowest = 100;

    /// <summary>
    /// Lowest percentage for the burn effect to be applied.
    /// </summary>
    public int BurnPercentageLowest
    {
        get { return burnPercentageLowest; }
        set
        {
            value = Mathf.Clamp(value, 0, burnPercentageHighest);
            burnPercentageLowest = value;
        }
    }

    /// <summary>Backing field for <see cref="BurnDuration"/>.</summary>
    [SerializeField] protected float burnDuration = 5;

    /// <summary>
    /// Duration of the effect.
    /// </summary>
    public float BurnDuration
    {
        get { return burnDuration; }
        set
        {
            if (value < 0) value = 0;
            burnDuration = value;
        }
    }

    /// <summary>Backing field for <see cref="DamagesMax"/>.</summary>
    [SerializeField,] private int burnDamagesMax = 5;

    /// <summary>
    /// Maximum damages amount dealed when burning.
    /// </summary>
    public int BurnDamagesMax
    {
        get { return burnDamagesMax; }
        set
        {
            if (value < 0) value = 0;
            burnDamagesMax = value;
        }
    }

    /// <summary>Backing field for <see cref="BurnDamagesMin"/>.</summary>
    [SerializeField] private int burnDamagesMin = 5;

    /// <summary>
    /// Minimum damages amount dealed when burning.
    /// </summary>
    public int BurnDamagesMin
    {
        get { return burnDamagesMin; }
        set
        {
            value = Mathf.Clamp(value, 0, burnDamagesMax);
            burnDamagesMin = value;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Attacks the target, by inflicting damages and applying effects.
    /// </summary>
    /// <param name="_attacker">The HitBox attacking the target.</param>
    /// <param name="_target">Target to attack.</param>
    /// <returns>Returns -1 if target didn't take any damages, 0 if no effect could be applied, and 1 if everything went good.</returns>
    public override int Attack(TDS_HitBox _attacker, TDS_Damageable _target)
    {
        // If should not apply burn effect, return
        int _result = base.Attack(_attacker, _target);
        if (_result < 0) return _result;

        // Roll the dices to get if burn effect should be applied
        int _percent = Random.Range(1, 101);
        if ((_percent > burnPercentageLowest) && ((_percent > burnPercentageHighest) || (_percent > Random.Range(burnPercentageLowest, burnPercentageHighest + 1)))) return 0;

        // Burn it
        _target.Burn(burnDamagesMin, burnDamagesMax, burnDuration);
        return 1;
    }
    #endregion
}
