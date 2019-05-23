using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Attack", menuName = "Attacks/Attack", order = 1)]
public class TDS_Attack : ScriptableObject
{
    /* TDS_Attack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class used to reference an attack and everything needed about it.
     *	    
     *	    The classes inheriting from TDS_Player use this, but the ones inheriting from TDS_Enemy use TDS_EnemyAttack, which inherit from this class.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[08 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the GetDamages property.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Attack class.
     *	
     *	    - Added the AnimationID, Description & Name fields ; and the damagesMax & damagesMin fields and properties.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="Name"/>.</summary>
    [SerializeField] protected string name = "New Attack"; 

    /// <summary>
    /// Name of this attack, used to reference it.
    /// </summary>
    public string Name { get { return name; } }


    /// <summary>
    /// Effect associated with this attack.
    /// </summary>
    public TDS_AttackEffect Effect = new TDS_AttackEffect();


    /// <summary>Backing field for <see cref="DamagesMax"/>.</summary>
    [SerializeField] protected int damagesMax = 1;

    /// <summary>
    /// The maximum amount of damages this attack can inflict.
    /// It must always be a positive value.
    /// </summary>
    public int DamagesMax
    {
        get { return damagesMax; }
        set
        {
            if (value < 0) value = 0;
            damagesMax = value;

            if (damagesMin > value) DamagesMin = value;
        }
    }

    /// <summary>Backing field for <see cref="DamagesMin"/>.</summary>
    [SerializeField] protected int damagesMin = 1;

    /// <summary>
    /// The minimum amount of damages this attack can inflict.
    /// Always superior to zero, and never superior to <see cref="DamagesMax"/>
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

    /// <summary>
    /// Get random damages from this attack.
    /// </summary>
    public int GetDamages
    {
        get { return Random.Range(damagesMin, damagesMax + 1); }
    }

    [SerializeField] protected string description = string.Empty;  
    /// <summary>
    /// Short (or long) description of this attack, and what it does.
    /// </summary>
    public string Description { get { return description; } }
    #endregion
}
