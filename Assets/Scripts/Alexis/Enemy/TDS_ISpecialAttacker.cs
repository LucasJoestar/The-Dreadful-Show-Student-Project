using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TDS_ISpecialAttacker 
{
    /* TDS_ISpecialAttacker :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Interface for the enemies that apply effects on their attacks]
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
	 *	Date :			[13/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the interface]
     *	    - Array of TDS_EnemyEffectiveAttacks
     *	    - Method to get an attack
     *	    - Method to cast effect of the attack
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    TDS_EffectiveEnemyAttack[] Attacks { get; }
    #endregion

    #region Methods

    #region Original Methods
    TDS_EffectiveEnemyAttack GetAttack(float _distance = 0);
    void ApplyAttackEffect(EnemyEffectiveAttackType _type);
    void CastFirstEffect();
    void CastSecondEffect();
    void CastThirdEffect();
    void CastSpecialEffect();
    #endregion

    #endregion
}
