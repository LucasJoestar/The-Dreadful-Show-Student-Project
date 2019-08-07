using System;
using UnityEngine;

[Serializable]
public class TDS_AttackEffect 
{
    /* TDS_AttackEffect :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Link a specific effect to an attack, with its percentage, damages, etc...
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[22 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
	 *  Creation of the TDS_AttackEffect class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// This attack effect type.
    /// </summary>
    public AttackEffectType EffectType = AttackEffectType.None;

    /// <summary>Backing field for <see cref="PercentageHighest"/>.</summary>
    [SerializeField] private int percentageHighest = 100;

    /// <summary>
    /// Highest percentage for this effect to be applied.
    /// </summary>
    public int PercentageHighest
    {
        get { return percentageHighest; }
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            percentageHighest = value;
        }
    }

    /// <summary>Backing field for <see cref="PercentageLowest"/>.</summary>
    [SerializeField] private int percentageLowest = 100;

    /// <summary>
    /// Lowest percentage for this effect to be applied.
    /// </summary>
    public int PercentageLowest
    {
        get { return percentageLowest; }
        set
        {
            value = Mathf.Clamp(value, 0, percentageHighest);
            percentageLowest = value;
        }
    }


    /// <summary>Backing field for <see cref="DamagesMax"/>.</summary>
    [SerializeField, ] private int damagesMax = 5;

    /// <summary>
    /// Maximum damages amount of this effect.
    /// </summary>
    public int DamagesMax
    {
        get { return damagesMax; }
        set
        {
            if (value < 0) value = 0;
            damagesMax = value;
        }
    }

    /// <summary>Backing field for <see cref="DamagesMin"/>.</summary>
    [SerializeField] private int damagesMin = 5;

    /// <summary>
    /// Minimum damages amount of this effect.
    /// </summary>
    public int DamagesMin
    {
        get { return damagesMin; }
        set
        {
            value = Mathf.Clamp(value, 0, damagesMax);
            damagesMin = value;
        }
    }
    #endregion
}
