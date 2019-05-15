using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[Serializable]
public class TDS_EnemyAttack : TDS_Attack 
{
    /* TDS_EnemyAttack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Inherited class from TDS_Attack 
     *	- Contains complementary informations for the enemies
     *	    - bool isDistanceAttack
     *	    - float predictedRange
     *	    - float probability
     *	    - int consecutiveUses
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     *	
	 *	Date :			[06/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : Adding a variable Attack Cooldown
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[24/01/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes : Initialisation of the class
	 *
	 *	- Create all variables : isDistanceRange, predictedRange, probability and consecutive uses
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    #region bool
    /// <summary>
    /// Backing field of <see cref="IsDistanceAttack"/> 
    /// </summary>
    [SerializeField]protected bool isDistanceAttack = false;
    /// <summary>
    /// Is the attack distance or not 
    /// </summary>
    public bool IsDistanceAttack
    {
        get
        {
            return isDistanceAttack;
        }
        set
        {
            isDistanceAttack = value;
        }
    }
    #endregion

    #region int
    /// <summary>
    /// ID of the animation used to play this attack.
    /// </summary>
    public int AnimationID = 0;

    /// <summary>
    /// Backing field of <see cref="ConsecutiveUses"/>
    /// </summary>
    protected int consecutiveUses = 0;
    /// <summary>
    /// Number of consecutives uses of this attack
    /// Reset to zero when the enemy use another attack
    /// </summary>
    public int ConsecutiveUses
    {
        get
        {
            return consecutiveUses; 
        }
        set
        {
            if (value < 0)
                consecutiveUses = 0;
            else consecutiveUses = value; 
        }
    }
    #endregion

    #region float 
    /// <summary>
    /// Backing field <see cref="Cooldown"/>
    /// </summary>
    [SerializeField] protected float cooldown = 3;
    /// <summary>
    /// Cooldown of the attack.
    /// The enemy has to wait this time (in seconds) before attacking again
    /// </summary>
    public float Cooldown { get { return cooldown; } }

    /// <summary>
    /// Backing field <see cref="PredictedRange"/>
    /// </summary>
    [SerializeField] protected float predictedRange = 0;
    /// <summary>
    /// Predicted Range of the attack. 
    /// Used in debug and to check if the enemy is in range to cast this attack
    /// </summary>
    public float PredictedRange
    {
        get
        {
            return predictedRange; 
        }
        set
        {
            if (value < 0) predictedRange = 0;
            else predictedRange = value; 
        }
    }

    /// <summary>
    /// Probability to cast this attack over 100.
    /// </summary>
    [SerializeField] protected float probability = 100;
    /// <summary>
    /// Probability to cast this attack divided by the number of consecutive uses +1.
    /// </summary>
    public float Probability
    {
        get
        {
            return probability / (consecutiveUses+1); 
        }
    }
    #endregion
	#endregion
}
